using System;
using System.Collections.Specialized;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;

namespace SC2TV.RTFControl {



	#region Public Enums

	// Enum for possible RTF colors
	public enum RtfColor {
		Black, Maroon, Green, Olive, Navy, Purple, Teal, Gray, Silver,
		Red, Lime, Yellow, Blue, Fuchsia, Aqua, White
	}

	#endregion

	public class ExRichTextBox : System.Windows.Forms.RichTextBox {
        #region Constants
        private const int WM_NCHITTEST = 0x84;
        private const int HTTRANSPARENT = -1;
        #endregion

		#region My Enums

		// Specifies the flags/options for the unmanaged call to the GDI+ method
		// Metafile.EmfToWmfBits().
		private enum EmfToWmfBitsFlags {

			// Use the default conversion
			EmfToWmfBitsFlagsDefault = 0x00000000,

			// Embedded the source of the EMF metafiel within the resulting WMF
			// metafile
			EmfToWmfBitsFlagsEmbedEmf = 0x00000001,

			// Place a 22-byte header in the resulting WMF file.  The header is
			// required for the metafile to be considered placeable.
			EmfToWmfBitsFlagsIncludePlaceable = 0x00000002,

			// Don't simulate clipping by using the XOR operator.
			EmfToWmfBitsFlagsNoXORClip = 0x00000004
		};

		#endregion

		#region My Structs

		// Definitions for colors in an RTF document
		private struct RtfColorDef {
			public const string Black = @"\red0\green0\blue0";
			public const string Maroon = @"\red128\green0\blue0";
			public const string Green = @"\red0\green128\blue0";
			public const string Olive = @"\red128\green128\blue0";
			public const string Navy = @"\red0\green0\blue128";
			public const string Purple = @"\red128\green0\blue128";
			public const string Teal = @"\red0\green128\blue128";
			public const string Gray = @"\red128\green128\blue128";
			public const string Silver = @"\red192\green192\blue192";
			public const string Red = @"\red255\green0\blue0";
			public const string Lime = @"\red0\green255\blue0";
			public const string Yellow = @"\red255\green255\blue0";
			public const string Blue = @"\red0\green0\blue255";
			public const string Fuchsia = @"\red255\green0\blue255";
			public const string Aqua = @"\red0\green255\blue255";
			public const string White = @"\red255\green255\blue255";
		}

		// Control words for RTF font families
		private struct RtfFontFamilyDef {
			public const string Unknown = @"\fnil";
			public const string Roman = @"\froman";
			public const string Swiss = @"\fswiss";
			public const string Modern = @"\fmodern";
			public const string Script = @"\fscript";
			public const string Decor = @"\fdecor";
			public const string Technical = @"\ftech";
			public const string BiDirect = @"\fbidi";
		}

		#endregion

		#region My Constants

		// Not used in this application.  Descriptions can be found with documentation
		// of Windows GDI function SetMapMode
		private const int MM_TEXT = 1;
		private const int MM_LOMETRIC = 2;
		private const int MM_HIMETRIC = 3;
		private const int MM_LOENGLISH = 4;
		private const int MM_HIENGLISH = 5;
		private const int MM_TWIPS = 6;
        private const int WM_VSCROLL = 277;
        private const int SB_LINEUP = 0;
        private const int SB_LINEDOWN = 1;
        private const int SB_THUMBPOSITION = 4;
        private const int SB_THUMBTRACK = 5;
        private const int SB_TOP = 6;
        private const int SB_BOTTOM = 7;
        private const int SB_ENDSCROLL = 8;
        private const int WM_USER = 0x400;
        private const int SB_VERT = 1;
        private const int EM_GETSCROLLPOS = WM_USER + 221;

        private System.Windows.Forms.Timer t = new System.Windows.Forms.Timer();
        
        delegate void ScrollCB( object o);

		// Ensures that the metafile maintains a 1:1 aspect ratio
		private const int MM_ISOTROPIC = 7;

		// Allows the x-coordinates and y-coordinates of the metafile to be adjusted
		// independently
		private const int MM_ANISOTROPIC = 8;

		// Represents an unknown font family
		private const string FF_UNKNOWN = "UNKNOWN";

		// The number of hundredths of millimeters (0.01 mm) in an inch
		// For more information, see GetImagePrefix() method.
		private const int HMM_PER_INCH = 2540;

		// The number of twips in an inch
		// For more information, see GetImagePrefix() method.
		private const int TWIPS_PER_INCH = 1440;

		#endregion

		#region My Privates
        private object scrollLock = new object();
		// The default text color
		private Color textColor;

		// The default text background color
		private Color highlightColor;

		// Dictionary that maps color enums to RTF color codes
		private HybridDictionary rtfColor;

		// Dictionary that mapas Framework font families to RTF font families
		private HybridDictionary rtfFontFamily;

		// The horizontal resolution at which the control is being displayed
		private float xDpi;

		// The vertical resolution at which the control is being displayed
		private float yDpi;

        private System.Threading.Timer scrollTimer;
        private System.Threading.Timer toimageTimer;

        #endregion

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetScrollInfo(IntPtr hwnd, int fnBar, ref SCROLLINFO lpsi);


        [DllImport("user32.dll")]
        private static extern bool GetScrollRange(IntPtr hWnd, int nBar, out int lpMinPos, out int lpMaxPos);

        [DllImport("user32.dll")]
        static extern int SetScrollInfo(IntPtr hwnd, int fnBar, [In] ref SCROLLINFO lpsi, bool fRedraw);

        [DllImport("User32.dll", CharSet = CharSet.Auto, EntryPoint = "SendMessage")]
        static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, Int32 wMsg, Int32 wParam, ref Point lParam);

        [DllImport("user32.dll", EntryPoint = "HideCaret")]
        public static extern long HideCaret(IntPtr hwnd);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr LoadLibrary(string lpFileName);


        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr CreateFontIndirect(ref LOGFONT lplf);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public class LOGFONT
        {
            public int lfHeight = 0;
            public int lfWidth = 0;
            public int lfEscapement = 0;
            public int lfOrientation = 0;
            public int lfWeight = 0;
            public byte lfItalic = 0;
            public byte lfUnderline = 0;
            public byte lfStrikeOut = 0;
            public byte lfCharSet = 0;
            public byte lfOutPrecision = 0;
            public byte lfClipPrecision = 0;
            public byte lfQuality = 0;
            public byte lfPitchAndFamily = 0;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string lfFaceName = string.Empty;
        }

        struct SCROLLINFO
        {
            public uint cbSize;
            public uint fMask;
            public int nMin;
            public int nMax;
            public uint nPage;
            public int nPos;
            public int nTrackPos;
        }

        enum ScrollBarDirection
        {
            SB_HORZ = 0,
            SB_VERT = 1,
            SB_CTL = 2,
            SB_BOTH = 3
        }

        enum ScrollInfoMask
        {
            SIF_RANGE = 0x1,
            SIF_PAGE = 0x2,
            SIF_POS = 0x4,
            SIF_DISABLENOSCROLL = 0x8,
            SIF_TRACKPOS = 0x10,
            SIF_ALL = SIF_RANGE + SIF_PAGE + SIF_POS + SIF_TRACKPOS
        }

        // Scrolls a given textbox. handle: an handle to our textbox. pixels: number of pixels to scroll.
        long scroll(int pixels)
        {
            long pixelsToEnd = 0;
            IntPtr ptrLparam = new IntPtr(0);
            IntPtr ptrWparam;
            // Get current scroller posion
            
            SCROLLINFO si = new SCROLLINFO();
            si.cbSize = (uint)Marshal.SizeOf(si);
            si.fMask = (uint)ScrollInfoMask.SIF_ALL;
            GetScrollInfo(Handle, (int)ScrollBarDirection.SB_VERT, ref si);           

            if( pixels == 0 )
            {
                endscroll(Handle);
                return si.nPos;
            }
            // Increase posion by pixles
            pixelsToEnd = (si.nMax - si.nPage) - (si.nPos + pixels);
            if (si.nPos < (si.nMax - si.nPage))
                si.nPos += pixels;
            else
            {
                endscroll(Handle);
                return pixelsToEnd;
            }
                        
            // Reposition scroller
            SetScrollInfo(Handle, (int)ScrollBarDirection.SB_VERT, ref si, true);

            // Send a WM_VSCROLL scroll message using SB_THUMBTRACK as wParam
            // SB_THUMBTRACK: low-order word of wParam, si.nPos high-order word of wParam
            ptrWparam = new IntPtr(SB_THUMBTRACK + 0x10000 * si.nPos);            
            SendMessage(Handle, WM_VSCROLL, ptrWparam, ptrLparam);
            return pixelsToEnd;
        }
        void endscroll(IntPtr handle)
        {
            IntPtr ptrLparam = new IntPtr(0);
            IntPtr ptrWparam;
            ptrWparam = new IntPtr(SB_ENDSCROLL);
            //t.Enabled = false;
            SendMessage(handle, WM_VSCROLL, ptrWparam, ptrLparam);
        }
        
        void t_Tick(object sender, EventArgs e)
        {
            var pixelsToEnd = scroll(1);
            
            if (pixelsToEnd <= 0)
            {
                //t.Enabled = false;
                //t.Stop();
            }

        }
        
        public UInt32 MaxLines
        {
            get;
            set;
        }
        public bool MouseTransparent
        {
            get;
            set;
        }
/*        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            if (m.Msg == (int)WM_NCHITTEST && MouseTransparent && ModifierKeys != Keys.Control)
                m.Result = (IntPtr)HTTRANSPARENT;
            else
                base.WndProc(ref m);
        }*/
        public Font TimestampFont
        {
            get;
            set;
        }
        public Color TimestampColor
        {
            get;
            set;
        }
        public string RTF
        {
            get { return Rtf; }
            set { Rtf = value; }
        }
        public void ReplaceSmileCode(String code, Bitmap bmp)
        {

        }
        public bool SaveToImage
        {
            get;
            set;
        }
        public string SaveToImageFileName
        {
            get;
            set;
        }
        public bool IsAtMaxScroll()
        {
            int minScroll;
            int maxScroll;
            GetScrollRange(Handle, SB_VERT, out minScroll, out maxScroll);
            Point rtfPoint = Point.Empty;
            SendMessage(Handle, EM_GETSCROLLPOS, 0, ref rtfPoint);


            return (rtfPoint.Y + this.ClientSize.Height >= maxScroll);
        }
        public int ScrollPos()
        {
            int minScroll;
            int maxScroll;
            GetScrollRange(Handle, SB_VERT, out minScroll, out maxScroll);
            Point rtfPoint = Point.Empty;
            SendMessage(Handle, EM_GETSCROLLPOS, 0, ref rtfPoint);


            return rtfPoint.Y + this.ClientSize.Height;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
              Rectangle r = this.ClientRectangle;
              r.Width -= 5;
              r.Height -= 5;
              e.Graphics.DrawRectangle(Pens.DeepSkyBlue, r);
        }
        public bool SlowScroll
        {
            get;
            set;
        }
        public bool Caret
        {
            get;
            set;
        }
     
        private void scrollTimer_Tick(object o)
        {
            if (InvokeRequired)
            {
                ScrollCB d = new ScrollCB(scrollTimer_Tick);
                try
                {
                    Parent.Invoke(d, new object[] { this });
                }
                catch { }
            }
            else
            {
                lock (scrollLock)
                {
                    if (String.IsNullOrEmpty(Text))
                        return;


                   if (!IsAtMaxScroll())
                   {
                        var linesToEnd = scroll(1);
                        if (linesToEnd > 15)
                        {
                            linesToEnd = scroll((int)(linesToEnd - (SlowScroll ? 15 : 0)));
                        }

                        while (linesToEnd > 0)
                        {
                            linesToEnd = scroll(1);
                            Thread.Sleep(20);
                        }
                    }
//                    if (!Caret)
                        //HideCaret(this.Handle);

                    ToImage();
                }
            }

        }
        private object lockScroll = new object();
        public void ScrollToEnd()
        {
            lock (lockScroll)
            {
                scrollTimer.Change(350, System.Threading.Timeout.Infinite);
            }
        }


        private void toimageTimer_Tick(object o)
        {
            if (SaveToImage && !String.IsNullOrEmpty(SaveToImageFileName))
                global::Ubiquitous.Control2Image.RtbToBitmap(this, SaveToImageFileName);
        }
        private void ToImage()
        {
            toimageTimer.Change(1000, System.Threading.Timeout.Infinite);
        }

		#region Elements required to create an RTF document
		
		/* RTF HEADER
		 * ----------
		 * 
		 * \rtf[N]		- For text to be considered to be RTF, it must be enclosed in this tag.
		 *				  rtf1 is used because the RichTextBox conforms to RTF Specification
		 *				  version 1.
		 * \ansi		- The character set.
		 * \ansicpg[N]	- Specifies that unicode characters might be embedded. ansicpg1252
		 *				  is the default used by Windows.
		 * \deff[N]		- The default font. \deff0 means the default font is the first font
		 *				  found.
		 * \deflang[N]	- The default language. \deflang1033 specifies US English.
		 * */
        private const string RTF_HEADER = @"{\rtf1\ansi\ansicpg1252\deff0\deflang1033";

		/* RTF DOCUMENT AREA
		 * -----------------
		 * 
		 * \viewkind[N]	- The type of view or zoom level.  \viewkind4 specifies normal view.
		 * \uc[N]		- The number of bytes corresponding to a Unicode character.
		 * \pard		- Resets to default paragraph properties
		 * \cf[N]		- Foreground color.  \cf1 refers to the color at index 1 in
		 *				  the color table
		 * \f[N]		- Font number. \f0 refers to the font at index 0 in the font
		 *				  table.
		 * \fs[N]		- Font size in half-points.
		 * */
		private const string RTF_DOCUMENT_PRE = @"\viewkind4\uc1\pard\cf1\f0\fs20";
		private const string RTF_DOCUMENT_POST = @"\cf0\fs17}";
		private string RTF_IMAGE_POST = @"}";

		#endregion

		#region Accessors

		// Overrides the default implementation of RTF.  This is done because the control
		// was originally developed to run in an instant messenger that uses the
		// Jabber XML-based protocol.  The framework would throw an exception when the
		// XML contained the null character, so I filtered out.
		public new string Rtf {
			get {return RemoveBadChars(base.Rtf);}
			set {base.Rtf = value;}
		}

        public bool TimeStamp
        {
            get;
            set;
        }

        public Color TimeColor
        {
            get;
            set;
        }
		// The color of the text
		public Color TextColor {
			get {return textColor;}
			set {textColor = value;}
		}

        public String RawTextColor
        {
            get;
            set;
        }

        public Color PersonalMessageColor
        {
            get;
            set;

        }
        public Color PersonalMessageBack
        {
            get;
            set;
        }
        public Font PersonalMessageFont
        {
            get;
            set;
        }
		// The color of the highlight
		public Color HighlightColor {
			get {return highlightColor;}
			set {highlightColor = value;}
		}

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes the text colors, creates dictionaries for RTF colors and
		/// font families, and stores the horizontal and vertical resolution of
		/// the RichTextBox's graphics context.
		/// </summary>
		public ExRichTextBox() : base() {

			// Initialize default text and background colors
            TextColor =  this.TextColor;
            HighlightColor = this.BackColor;

			// Initialize the dictionary mapping color codes to definitions
			rtfColor = new HybridDictionary();
			rtfColor.Add(RtfColor.Aqua, RtfColorDef.Aqua);
			rtfColor.Add(RtfColor.Black, RtfColorDef.Black);
			rtfColor.Add(RtfColor.Blue, RtfColorDef.Blue);
			rtfColor.Add(RtfColor.Fuchsia, RtfColorDef.Fuchsia);
			rtfColor.Add(RtfColor.Gray, RtfColorDef.Gray);
			rtfColor.Add(RtfColor.Green, RtfColorDef.Green);
			rtfColor.Add(RtfColor.Lime, RtfColorDef.Lime);
			rtfColor.Add(RtfColor.Maroon, RtfColorDef.Maroon);
			rtfColor.Add(RtfColor.Navy, RtfColorDef.Navy);
			rtfColor.Add(RtfColor.Olive, RtfColorDef.Olive);
			rtfColor.Add(RtfColor.Purple, RtfColorDef.Purple);
			rtfColor.Add(RtfColor.Red, RtfColorDef.Red);
			rtfColor.Add(RtfColor.Silver, RtfColorDef.Silver);
			rtfColor.Add(RtfColor.Teal, RtfColorDef.Teal);
			rtfColor.Add(RtfColor.White, RtfColorDef.White);
			rtfColor.Add(RtfColor.Yellow, RtfColorDef.Yellow);

			// Initialize the dictionary mapping default Framework font families to
			// RTF font families
			rtfFontFamily = new HybridDictionary();
			rtfFontFamily.Add(FontFamily.GenericMonospace.Name, RtfFontFamilyDef.Modern);
			rtfFontFamily.Add(FontFamily.GenericSansSerif, RtfFontFamilyDef.Swiss);
			rtfFontFamily.Add(FontFamily.GenericSerif, RtfFontFamilyDef.Roman);
			rtfFontFamily.Add(FF_UNKNOWN, RtfFontFamilyDef.Unknown);

			// Get the horizontal and vertical resolutions at which the object is
			// being displayed
			using(Graphics _graphics = this.CreateGraphics()) {
				xDpi = _graphics.DpiX;
				yDpi = _graphics.DpiY;
			}

            t.Tick += new EventHandler(t_Tick);

            scrollTimer = new System.Threading.Timer(new TimerCallback(scrollTimer_Tick), null, Timeout.Infinite, Timeout.Infinite);
            toimageTimer = new System.Threading.Timer(new TimerCallback(toimageTimer_Tick), null, Timeout.Infinite, Timeout.Infinite);

		}

		/// <summary>
		/// Calls the default constructor then sets the text color.
		/// </summary>
		/// <param name="_textColor"></param>
		public ExRichTextBox(Color _textColor) : this() {
			textColor = _textColor;
		}

		/// <summary>
		/// Calls the default constructor then sets te text and highlight colors.
		/// </summary>
		/// <param name="_textColor"></param>
		/// <param name="_highlightColor"></param>
		public ExRichTextBox(Color _textColor, Color _highlightColor) : this() {
			textColor = _textColor;
			highlightColor = _highlightColor;
		}

		#endregion

		#region Append RTF or Text to RichTextBox Contents

		/// <summary>
		/// Assumes the string passed as a paramter is valid RTF text and attempts
		/// to append it as RTF to the content of the control.
		/// </summary>
		/// <param name="_rtf"></param>
		public void AppendRtf(string _rtf) {

            // Move caret to the end of the text
			this.Select(this.TextLength, 0);

			// Since SelectedRtf is null, this will append the string to the
			// end of the existing RTF
            this.SelectedRtf = _rtf;
		}

		/// <summary>
		/// Assumes that the string passed as a parameter is valid RTF text and
		/// attempts to insert it as RTF into the content of the control.
		/// </summary>
		/// <remarks>
		/// NOTE: The text is inserted wherever the caret is at the time of the call,
		/// and if any text is selected, that text is replaced.
		/// </remarks>
		/// <param name="_rtf"></param>
		public void InsertRtf(string _rtf) {
			this.SelectedRtf = _rtf;
		}

		/// <summary>
		/// Appends the text using the current font, text, and highlight colors.
		/// </summary>
		/// <param name="_text"></param>
		public void AppendTextAsRtf(string _text) {
			AppendTextAsRtf(_text, this.Font);
		}


		/// <summary>
		/// Appends the text using the given font, and current text and highlight
		/// colors.
		/// </summary>
		/// <param name="_text"></param>
		/// <param name="_font"></param>
		public void AppendTextAsRtf(string _text, Font _font) {
			AppendTextAsRtf(_text, _font, textColor);
		}
		
		/// <summary>
		/// Appends the text using the given font and text color, and the current
		/// highlight color.
		/// </summary>
		/// <param name="_text"></param>
		/// <param name="_font"></param>
		/// <param name="_color"></param>
		public void AppendTextAsRtf(string _text, Font _font, Color _textColor) {
			AppendTextAsRtf(_text, _font, _textColor, highlightColor);
		}

		/// <summary>
		/// Appends the text using the given font, text, and highlight colors.  Simply
		/// moves the caret to the end of the RichTextBox's text and makes a call to
		/// insert.
		/// </summary>
		/// <param name="_text"></param>
		/// <param name="_font"></param>
		/// <param name="_textColor"></param>
		/// <param name="_backColor"></param>
		public void AppendTextAsRtf(string _text, Font _font, Color _textColor, Color _backColor) {
			// Move carret to the end of the text
			this.Select(this.TextLength, 0);

			InsertTextAsRtf(_text, _font, _textColor, _backColor);
		}

		#endregion

		#region Insert Plain Text

		/// <summary>
		/// Inserts the text using the current font, text, and highlight colors.
		/// </summary>
		/// <param name="_text"></param>
		public void InsertTextAsRtf(string _text) {
			InsertTextAsRtf(_text, this.Font);
		}


		/// <summary>
		/// Inserts the text using the given font, and current text and highlight
		/// colors.
		/// </summary>
		/// <param name="_text"></param>
		/// <param name="_font"></param>
		public void InsertTextAsRtf(string _text, Font _font) {
			InsertTextAsRtf(_text, _font, textColor);
		}
		
		/// <summary>
		/// Inserts the text using the given font and text color, and the current
		/// highlight color.
		/// </summary>
		/// <param name="_text"></param>
		/// <param name="_font"></param>
		/// <param name="_color"></param>
		public void InsertTextAsRtf(string _text, Font _font, Color _textColor) {
			InsertTextAsRtf(_text, _font, _textColor, highlightColor);
		}

		/// <summary>
		/// Inserts the text using the given font, text, and highlight colors.  The
		/// text is wrapped in RTF codes so that the specified formatting is kept.
		/// You can only assign valid RTF to the RichTextBox.Rtf property, else
		/// an exception is thrown.  The RTF string should follow this format ...
		/// 
		/// {\rtf1\ansi\ansicpg1252\deff0\deflang1033{\fonttbl{[FONTS]}{\colortbl ;[COLORS]}}
		/// \viewkind4\uc1\pard\cf1\f0\fs20 [DOCUMENT AREA] }
		/// 
		/// </summary>
		/// <remarks>
		/// NOTE: The text is inserted wherever the caret is at the time of the call,
		/// and if any text is selected, that text is replaced.
		/// </remarks>
		/// <param name="_text"></param>
		/// <param name="_font"></param>
		/// <param name="_color"></param>
		/// <param name="_color"></param>
		public void InsertTextAsRtf(string _text, Font _font, Color _textColor, Color _backColor) {

            StringBuilder _rtf = new StringBuilder();

			// Append the RTF header
			_rtf.Append(RTF_HEADER);

			// Create the font table from the font passed in and append it to the
			// RTF string
			_rtf.Append(GetFontTable(_font));

			// Create the color table from the colors passed in and append it to the
			// RTF string
			_rtf.Append(GetColorTable(_textColor, _backColor));

			// Create the document area from the text to be added as RTF and append
			// it to the RTF string.
			_rtf.Append(GetDocumentArea(EncodeNonAsciiCharacters(_text), _font));
           
			this.SelectedRtf = _rtf.ToString();

            var prevScrollPos = ScrollPos();
            while (MaxLines > 0 && Lines.Length > MaxLines)
            {
                ReadOnly = false;
                SelectionStart = 0;
                SelectionLength = Text.IndexOf("\n", 0) + 1;
                SelectedText = "";
                ReadOnly = true;
                var newScrollPos = ScrollPos();
            }



		}
        private string EncodeNonAsciiCharacters(string value)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in value)
            {
                if (c > 127)
                {
                    // This character is too big for ASCII
                    string encodedValue = "\\u" + ((int)c).ToString("d4") + "?";
                    sb.Append(encodedValue);
                }
                else
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }
        public string TextToRtfText(string _text)
        {
            StringBuilder _rtf = new StringBuilder();

            // Append the RTF header
            _rtf.Append(RTF_HEADER);

            // Create the font table from the font passed in and append it to the
            // RTF string
            _rtf.Append(GetFontTable(this.Font));

            // Create the color table from the colors passed in and append it to the
            // RTF string
            _rtf.Append(GetColorTable(textColor, highlightColor));

            // Create the document area from the text to be added as RTF and append
            // it to the RTF string.
            _rtf.Append(GetDocumentArea(_text, this.Font));

            return _rtf.ToString();
        }

		/// <summary>
		/// Creates the Document Area of the RTF being inserted. The document area
		/// (in this case) consists of the text being added as RTF and all the
		/// formatting specified in the Font object passed in. This should have the
		/// form ...
		/// 
		/// \viewkind4\uc1\pard\cf1\f0\fs20 [DOCUMENT AREA] }
		///
		/// </summary>
		/// <param name="_text"></param>
		/// <param name="_font"></param>
		/// <returns>
		/// The document area as a string.
		/// </returns>
		private string GetDocumentArea(string _text, Font _font) {

			StringBuilder _doc = new StringBuilder();
			
			// Append the standard RTF document area control string
			_doc.Append(RTF_DOCUMENT_PRE);

			// Set the highlight color (the color behind the text) to the
			// third color in the color table.  See GetColorTable for more details.
			_doc.Append(@"\highlight2");

			// If the font is bold, attach corresponding tag
			if (_font.Bold)
				_doc.Append(@"\b");

			// If the font is italic, attach corresponding tag
			if (_font.Italic)
				_doc.Append(@"\i");

			// If the font is strikeout, attach corresponding tag
			if (_font.Strikeout)
				_doc.Append(@"\strike");

			// If the font is underlined, attach corresponding tag
			if (_font.Underline)
				_doc.Append(@"\ul");

			// Set the font to the first font in the font table.
			// See GetFontTable for more details.
			_doc.Append(@"\f0");

			// Set the size of the font.  In RTF, font size is measured in
			// half-points, so the font size is twice the value obtained from
			// Font.SizeInPoints
			_doc.Append(@"\fs");
			_doc.Append((int)Math.Round((2 * _font.SizeInPoints)));

			// Apppend a space before starting actual text (for clarity)
			_doc.Append(@" ");

			// Append actual text, however, replace newlines with RTF \par.
			// Any other special text should be handled here (e.g.) tabs, etc.
			_doc.Append(_text.Replace("\n", @"\par "));

			// RTF isn't strict when it comes to closing control words, but what the
			// heck ...

			// Remove the highlight
			_doc.Append(@"\highlight0");

			// If font is bold, close tag
			if (_font.Bold)
				_doc.Append(@"\b0");

			// If font is italic, close tag
			if (_font.Italic)
				_doc.Append(@"\i0");

			// If font is strikeout, close tag
			if (_font.Strikeout)
				_doc.Append(@"\strike0");

			// If font is underlined, cloes tag
			if (_font.Underline)
				_doc.Append(@"\ulnone");

			// Revert back to default font and size
			_doc.Append(@"\f0");
			_doc.Append(@"\fs20");

			// Close the document area control string
			_doc.Append(RTF_DOCUMENT_POST);

			return _doc.ToString();
		}

		#endregion

		#region Insert Image

		/// <summary>
		/// Inserts an image into the RichTextBox.  The image is wrapped in a Windows
		/// Format Metafile, because although Microsoft discourages the use of a WMF,
		/// the RichTextBox (and even MS Word), wraps an image in a WMF before inserting
		/// the image into a document.  The WMF is attached in HEX format (a string of
		/// HEX numbers).
		/// 
		/// The RTF Specification v1.6 says that you should be able to insert bitmaps,
		/// .jpegs, .gifs, .pngs, and Enhanced Metafiles (.emf) directly into an RTF
		/// document without the WMF wrapper. This works fine with MS Word,
		/// however, when you don't wrap images in a WMF, WordPad and
		/// RichTextBoxes simply ignore them.  Both use the riched20.dll or msfted.dll.
		/// </summary>
		/// <remarks>
		/// NOTE: The image is inserted wherever the caret is at the time of the call,
		/// and if any text is selected, that text is replaced.
		/// </remarks>
		/// <param name="_image"></param>
		public void InsertImage(Image _image) {

            StringBuilder _rtf = new StringBuilder();

            using (Image b = new Bitmap(_image.Width, _image.Height))
            {
                using (Graphics g = Graphics.FromImage(b))
                {
                    g.Clear(this.BackColor);
                    g.DrawImageUnscaled(_image, 0, 0);
                }
                _image = b;

			    // Append the RTF header
                _rtf.Append(RTF_HEADER);

                // Create the font table using the RichTextBox's current font and append
			    // it to the RTF string
			    _rtf.Append(GetFontTable(this.Font));

			    // Create the image control string and append it to the RTF string			
                _rtf.Append(GetImagePrefix(_image));
            

			    // Create the Windows Metafile and append its bytes in HEX format
			    _rtf.Append(GetRtfImage(_image));
            }
            // Close the RTF image control string
            _rtf.Append(RTF_IMAGE_POST);
            this.SelectedRtf = _rtf.ToString();
		}
        public string ImageToString(Image _image)
        {
            StringBuilder _rtf = new StringBuilder();

            // Append the RTF header
            _rtf.Append(RTF_HEADER);

            // Create the font table using the RichTextBox's current font and append
            // it to the RTF string
            _rtf.Append(GetFontTable(this.Font));

            // Create the image control string and append it to the RTF string
            _rtf.Append(GetImagePrefix(_image));

            // Create the Windows Metafile and append its bytes in HEX format
            _rtf.Append(GetRtfImage(_image));

            // Close the RTF image control string
            _rtf.Append(RTF_IMAGE_POST);

            return _rtf.ToString();
        }
		/// <summary>
		/// Creates the RTF control string that describes the image being inserted.
		/// This description (in this case) specifies that the image is an
		/// MM_ANISOTROPIC metafile, meaning that both X and Y axes can be scaled
		/// independently.  The control string also gives the images current dimensions,
		/// and its target dimensions, so if you want to control the size of the
		/// image being inserted, this would be the place to do it. The prefix should
		/// have the form ...
		/// 
		/// {\pict\wmetafile8\picw[A]\pich[B]\picwgoal[C]\pichgoal[D]
		/// 
		/// where ...
		/// 
		/// A	= current width of the metafile in hundredths of millimeters (0.01mm)
		///		= Image Width in Inches * Number of (0.01mm) per inch
		///		= (Image Width in Pixels / Graphics Context's Horizontal Resolution) * 2540
		///		= (Image Width in Pixels / Graphics.DpiX) * 2540
		/// 
		/// B	= current height of the metafile in hundredths of millimeters (0.01mm)
		///		= Image Height in Inches * Number of (0.01mm) per inch
		///		= (Image Height in Pixels / Graphics Context's Vertical Resolution) * 2540
		///		= (Image Height in Pixels / Graphics.DpiX) * 2540
		/// 
		/// C	= target width of the metafile in twips
		///		= Image Width in Inches * Number of twips per inch
		///		= (Image Width in Pixels / Graphics Context's Horizontal Resolution) * 1440
		///		= (Image Width in Pixels / Graphics.DpiX) * 1440
		/// 
		/// D	= target height of the metafile in twips
		///		= Image Height in Inches * Number of twips per inch
		///		= (Image Height in Pixels / Graphics Context's Horizontal Resolution) * 1440
		///		= (Image Height in Pixels / Graphics.DpiX) * 1440
		///	
		/// </summary>
		/// <remarks>
		/// The Graphics Context's resolution is simply the current resolution at which
		/// windows is being displayed.  Normally it's 96 dpi, but instead of assuming
		/// I just added the code.
		/// 
		/// According to Ken Howe at pbdr.com, "Twips are screen-independent units
		/// used to ensure that the placement and proportion of screen elements in
		/// your screen application are the same on all display systems."
		/// 
		/// Units Used
		/// ----------
		/// 1 Twip = 1/20 Point
		/// 1 Point = 1/72 Inch
		/// 1 Twip = 1/1440 Inch
		/// 
		/// 1 Inch = 2.54 cm
		/// 1 Inch = 25.4 mm
		/// 1 Inch = 2540 (0.01)mm
		/// </remarks>
		/// <param name="_image"></param>
		/// <returns></returns>
		private string GetImagePrefix(Image _image) {

			StringBuilder _rtf = new StringBuilder();

			// Calculate the current width of the image in (0.01)mm
			int picw = (int)Math.Round((_image.Width / xDpi) * HMM_PER_INCH);

			// Calculate the current height of the image in (0.01)mm
			int pich = (int)Math.Round((_image.Height / yDpi) * HMM_PER_INCH);

			// Calculate the target width of the image in twips
			int picwgoal = (int)Math.Round((_image.Width / xDpi) * TWIPS_PER_INCH);

			// Calculate the target height of the image in twips
			int pichgoal = (int)Math.Round((_image.Height / yDpi) * TWIPS_PER_INCH);

			// Append values to RTF string
            //_rtf.Append(@"{\*\picprop\shplid1025{\sp{\sn fLockAgainstSelect}{\sv 1}}}");
            _rtf.Append(@"{\pict{\*\picprop{\sp{\sn fLockAgainstSelect}{\sv 1}}}\wmetafile8");
            _rtf.Append(@"\picw");
			_rtf.Append(picw);
			_rtf.Append(@"\pich");
			_rtf.Append(pich);
			_rtf.Append(@"\picwgoal");
			_rtf.Append(picwgoal);
			_rtf.Append(@"\pichgoal");
			_rtf.Append(pichgoal);
			_rtf.Append(" ");

			return _rtf.ToString();
		}

		/// <summary>
		/// Use the EmfToWmfBits function in the GDI+ specification to convert a 
		/// Enhanced Metafile to a Windows Metafile
		/// </summary>
		/// <param name="_hEmf">
		/// A handle to the Enhanced Metafile to be converted
		/// </param>
		/// <param name="_bufferSize">
		/// The size of the buffer used to store the Windows Metafile bits returned
		/// </param>
		/// <param name="_buffer">
		/// An array of bytes used to hold the Windows Metafile bits returned
		/// </param>
		/// <param name="_mappingMode">
		/// The mapping mode of the image.  This control uses MM_ANISOTROPIC.
		/// </param>
		/// <param name="_flags">
		/// Flags used to specify the format of the Windows Metafile returned
		/// </param>
		[DllImportAttribute("gdiplus.dll")]
		private static extern uint GdipEmfToWmfBits (IntPtr _hEmf, uint _bufferSize,
			byte[] _buffer, int _mappingMode, EmfToWmfBitsFlags _flags);


		/// <summary>
		/// Wraps the image in an Enhanced Metafile by drawing the image onto the
		/// graphics context, then converts the Enhanced Metafile to a Windows
		/// Metafile, and finally appends the bits of the Windows Metafile in HEX
		/// to a string and returns the string.
		/// </summary>
		/// <param name="_image"></param>
		/// <returns>
		/// A string containing the bits of a Windows Metafile in HEX
		/// </returns>
		private string GetRtfImage(Image _image) {

			StringBuilder _rtf = null;

			// Used to store the enhanced metafile
			MemoryStream _stream = null;

			// Used to create the metafile and draw the image
			Graphics _graphics = null;

			// The enhanced metafile
			Metafile _metaFile = null;

			// Handle to the device context used to create the metafile
			IntPtr _hdc;

			try {
				_rtf = new StringBuilder();
				_stream = new MemoryStream();

				// Get a graphics context from the RichTextBox
				using(_graphics = this.CreateGraphics()) {
					// Get the device context from the graphics context

                    _hdc = _graphics.GetHdc();

					// Create a new Enhanced Metafile from the device context
					_metaFile = new Metafile(_stream, _hdc);
					// Release the device context
					_graphics.ReleaseHdc(_hdc);
				}
                
				// Get a graphics context from the Enhanced Metafile
				using(_graphics = Graphics.FromImage(_metaFile)) {
                        
                    Bitmap temp = new Bitmap(_image.Width, _image.Height, PixelFormat.Format24bppRgb);

                    using (Graphics g = Graphics.FromImage(temp))
                    {
                        g.Clear(Color.Black);
                        g.DrawImage(_image, Point.Empty);

                        // Draw the image on the Enhanced Metafile
                        _graphics.DrawImage(temp, new Rectangle(0, 0, _image.Width, _image.Height));
                    }
                    

				}

				// Get the handle of the Enhanced Metafile
				IntPtr _hEmf = _metaFile.GetHenhmetafile();

				// A call to EmfToWmfBits with a null buffer return the size of the
				// buffer need to store the WMF bits.  Use this to get the buffer
				// size.
				uint _bufferSize = GdipEmfToWmfBits(_hEmf, 0, null, MM_ANISOTROPIC,
                    EmfToWmfBitsFlags.EmfToWmfBitsFlagsNoXORClip);

				// Create an array to hold the bits
				byte[] _buffer = new byte[_bufferSize];

				// A call to EmfToWmfBits with a valid buffer copies the bits into the
				// buffer an returns the number of bits in the WMF.  
				uint _convertedSize = GdipEmfToWmfBits(_hEmf, _bufferSize, _buffer, MM_ANISOTROPIC,
                    EmfToWmfBitsFlags.EmfToWmfBitsFlagsNoXORClip);

				// Append the bits to the RTF string
				for(int i = 0; i < _buffer.Length; ++i) {
					_rtf.Append(String.Format("{0:X2}", _buffer[i]));
				}

				return _rtf.ToString();
			}
			finally {
				if(_graphics != null)
					_graphics.Dispose();
				if(_metaFile != null)
					_metaFile.Dispose();
				if(_stream != null)
					_stream.Close();
			}
		}
		
		#endregion

		#region RTF Helpers

		/// <summary>
		/// Creates a font table from a font object.  When an Insert or Append 
		/// operation is performed a font is either specified or the default font
		/// is used.  In any case, on any Insert or Append, only one font is used,
		/// thus the font table will always contain a single font.  The font table
		/// should have the form ...
		/// 
		/// {\fonttbl{\f0\[FAMILY]\fcharset0 [FONT_NAME];}
		/// </summary>
		/// <param name="_font"></param>
		/// <returns></returns>
        /// 
		private string GetFontTable(Font _font) {
            if (_font == null)
                return String.Empty;
			StringBuilder _fontTable = new StringBuilder();

			// Append table control string
			_fontTable.Append(@"{\fonttbl{\f0");
			_fontTable.Append(@"\");
			
			// If the font's family corresponds to an RTF family, append the
			// RTF family name, else, append the RTF for unknown font family.
			if (rtfFontFamily.Contains(_font.FontFamily.Name))
				_fontTable.Append(rtfFontFamily[_font.FontFamily.Name]);
			else
				_fontTable.Append(rtfFontFamily[FF_UNKNOWN]);

			// \fcharset specifies the character set of a font in the font table.
			// 0 is for ANSI.
			_fontTable.Append(@"\fcharset0 ");

			// Append the name of the font
			_fontTable.Append(_font.Name);

			// Close control string
			_fontTable.Append(@";}}");

			return _fontTable.ToString();
		}

		/// <summary>
		/// Creates a font table from the RtfColor structure.  When an Insert or Append
		/// operation is performed, _textColor and _backColor are either specified
		/// or the default is used.  In any case, on any Insert or Append, only three
		/// colors are used.  The default color of the RichTextBox (signified by a
		/// semicolon (;) without a definition), is always the first color (index 0) in
		/// the color table.  The second color is always the text color, and the third
		/// is always the highlight color (color behind the text).  The color table
		/// should have the form ...
		/// 
		/// {\colortbl ;[TEXT_COLOR];[HIGHLIGHT_COLOR];}
		/// 
		/// </summary>
		/// <param name="_textColor"></param>
		/// <param name="_backColor"></param>
		/// <returns></returns>
		private string GetColorTable(Color _textColor, Color _backColor) {

			StringBuilder _colorTable = new StringBuilder();

			// Append color table control string and default font (;)
			_colorTable.Append(@"{\colortbl ;");
            _colorTable.Append(GetRtfColor(_textColor));
            _colorTable.Append(@";");



			// Append the highlight color
			_colorTable.Append(GetRtfColor(_backColor));
			_colorTable.Append(@";}\n");
					
			return _colorTable.ToString();
		}
        public static String GetRtfColor(Color color)
        {
            string mask = @"\red{0}\green{1}\blue{2}";
            return String.Format(mask, color.R, color.G, color.B);
        }
		/// <summary>
		/// Called by overrided RichTextBox.Rtf accessor.
		/// Removes the null character from the RTF.  This is residue from developing
		/// the control for a specific instant messaging protocol and can be ommitted.
		/// </summary>
		/// <param name="_originalRtf"></param>
		/// <returns>RTF without null character</returns>
		private string RemoveBadChars(string _originalRtf) {			
			return _originalRtf.Replace("\0", "");
		}

		#endregion

        #region URLs actions
        public void LinkClick(string link)
        {
            try
            {
                System.Diagnostics.Process.Start(link);
            }
            catch (System.ComponentModel.Win32Exception noBrowser)
            {
                if (noBrowser.ErrorCode == -2147467259)
                    MessageBox.Show(noBrowser.Message);
            }
            catch (System.Exception other)
            {
                MessageBox.Show(other.Message);
            }
        }

        #endregion
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // ExRichTextBox
            // 
            this.Resize += new System.EventHandler(this.ExRichTextBox_Resize);
            this.ResumeLayout(false);

        }
        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            if (!Caret)
                HideCaret(this.Handle);
        }
        protected override void OnSizeChanged(EventArgs e)
        {
            //endscroll(this.Handle);
            //SelectionStart = Text.Length;
            //SelectionLength = 0;
        }
        private void ExRichTextBox_Resize(object sender, EventArgs e)
        {

            //ScrollToEnd();
        }
        protected override CreateParams CreateParams
        {
            get
            {
                var prams = base.CreateParams;
                if (LoadLibrary("msftedit.dll") != IntPtr.Zero)
                    prams.ClassName = "RICHEDIT50W";

                //prams.ExStyle |= 0x00000020;
                return prams;
            }
        }


        /// <summary>
        /// Maintains performance while updating.
        /// </summary>
        /// <remarks>
        /// <para>
        /// It is recommended to call this method before doing
        /// any major updates that you do not wish the user to
        /// see. Remember to call EndUpdate when you are finished
        /// with the update. Nested calls are supported.
        /// </para>
        /// <para>
        /// Calling this method will prevent redrawing. It will
        /// also setup the event mask of the underlying richedit
        /// control so that no events are sent.
        /// </para>
        /// </remarks>
        public void BeginUpdate()
        {
            // Deal with nested calls.
            ++updating;
        
            if ( updating > 1 )
                return;
        
            // Prevent the control from raising any events.
            oldEventMask = SendMessage( new HandleRef( this, Handle ),
                                        EM_SETEVENTMASK, 0, 0 );
        
            // Prevent the control from redrawing itself.
            SendMessage( new HandleRef( this, Handle ),
                         WM_SETREDRAW, 0, 0 );
        }
    
        /// <summary>
        /// Resumes drawing and event handling.
        /// </summary>
        /// <remarks>
        /// This method should be called every time a call is made
        /// made to BeginUpdate. It resets the event mask to it's
        /// original value and enables redrawing of the control.
        /// </remarks>
        public void EndUpdate()
        {
            // Deal with nested calls.
            --updating;
        
            if ( updating > 0 )
                return;
        
            // Allow the control to redraw itself.
            SendMessage( new HandleRef( this, Handle ),
                         WM_SETREDRAW, 1, 0 );
        
            // Allow the control to raise event messages.
            SendMessage( new HandleRef( this, Handle ),
                         EM_SETEVENTMASK, 0, oldEventMask );
        }
    
        /// <summary>
        /// Gets or sets the alignment to apply to the current
        /// selection or insertion point.
        /// </summary>
        /// <remarks>
        /// Replaces the SelectionAlignment from
        /// <see cref="RichTextBox"/>.
        /// </remarks>
        public new TextAlign SelectionAlignment
        {
            get
            {
                PARAFORMAT fmt = new PARAFORMAT();
                fmt.cbSize = Marshal.SizeOf( fmt );
            
                // Get the alignment.
                SendMessage( new HandleRef( this, Handle ),
                             EM_GETPARAFORMAT,
                             SCF_SELECTION, ref fmt );
            
                // Default to Left align.
                if ( ( fmt.dwMask & PFM_ALIGNMENT ) == 0 )
                    return TextAlign.Left;
            
                return ( TextAlign )fmt.wAlignment;
            }
        
            set
            {
                PARAFORMAT fmt = new PARAFORMAT();
                fmt.cbSize = Marshal.SizeOf( fmt );
                fmt.dwMask = PFM_ALIGNMENT;
                fmt.wAlignment = ( short )value;
            
                // Set the alignment.
                SendMessage( new HandleRef( this, Handle ),
                             EM_SETPARAFORMAT,
                             SCF_SELECTION, ref fmt );
            }
        }

        public bool Antialias
        {
            get;
            set;

        }

        /// <summary>
        /// This member overrides
        /// <see cref="Control"/>.OnHandleCreated.
        /// </summary>
        protected override void OnHandleCreated( EventArgs e )
        {
            base.OnHandleCreated( e );
        
            // Enable support for justification.
            SendMessage( new HandleRef( this, Handle ),
                         EM_SETTYPOGRAPHYOPTIONS,
                         TO_ADVANCEDTYPOGRAPHY,
                         TO_ADVANCEDTYPOGRAPHY );
        }
    
        private int updating = 0;
        private int oldEventMask = 0;
    
        // Constants from the Platform SDK.
        private const int EM_SETEVENTMASK = 1073;
        private const int EM_GETPARAFORMAT = 1085;
        private const int EM_SETPARAFORMAT = 1095;
        private const int EM_SETTYPOGRAPHYOPTIONS = 1226;
        private const int WM_SETREDRAW = 11;
        private const int TO_ADVANCEDTYPOGRAPHY = 1;
        private const int PFM_ALIGNMENT = 8;
        private const int SCF_SELECTION = 1;
    
        // It makes no difference if we use PARAFORMAT or
        // PARAFORMAT2 here, so I have opted for PARAFORMAT2.
        [StructLayout( LayoutKind.Sequential )]
        private struct PARAFORMAT
        {
            public int cbSize;
            public uint dwMask;
            public short wNumbering;
            public short wReserved;
            public int dxStartIndent;
            public int dxRightIndent;
            public int dxOffset;
            public short wAlignment;
            public short cTabCount;
            [MarshalAs( UnmanagedType.ByValArray, SizeConst = 32 )]
            public int[] rgxTabs;
        
            // PARAFORMAT2 from here onwards.
            public int dySpaceBefore;
            public int dySpaceAfter;
            public int dyLineSpacing;
            public short sStyle;
            public byte bLineSpacingRule;
            public byte bOutlineLevel;
            public short wShadingWeight;
            public short wShadingStyle;
            public short wNumberingStart;
            public short wNumberingStyle;
            public short wNumberingTab;
            public short wBorderSpace;
            public short wBorderWidth;
            public short wBorders;
        }
    
        [DllImport( "user32", CharSet = CharSet.Auto )]
        private static extern int SendMessage( HandleRef hWnd,
                                               int msg,
                                               int wParam,
                                               int lParam );
    
        [DllImport( "user32", CharSet = CharSet.Auto )]
        private static extern int SendMessage( HandleRef hWnd,
                                               int msg,
                                               int wParam,
                                               ref PARAFORMAT lp );

        /// <summary>
        /// Specifies how text in a <see cref="AdvRichTextBox"/> is
        /// horizontally aligned.
        /// </summary>
        public enum TextAlign
        {
            /// <summary>
            /// The text is aligned to the left.
            /// </summary>
            Left = 1,
    
            /// <summary>
            /// The text is aligned to the right.
            /// </summary>
            Right = 2,
    
            /// <summary>
            /// The text is aligned in the center.
            /// </summary>
            Center = 3,
    
            /// <summary>
            /// The text is justified.
            /// </summary>
            Justify = 4
        }
    }
}
