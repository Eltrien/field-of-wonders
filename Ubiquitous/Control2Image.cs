using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Ubiquitous
{
	public static class Control2Image
	{
		[DllImport("USER32.dll")]
        private static extern Int32 SendMessage(IntPtr hWnd, int msg, int wParam, IntPtr lParam);
        private const int WM_USER = 0x400;
        private const int EM_FORMATRANGE = WM_USER + 57;
        [StructLayout(LayoutKind.Sequential)]
        private struct RECT {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }
        [StructLayout(LayoutKind.Sequential)]
        private struct CHARRANGE {
            public int cpMin;
            public int cpMax;
        }
        [StructLayout(LayoutKind.Sequential)]
        private struct FORMATRANGE {
            public IntPtr hdc;
            public IntPtr hdcTarget;
            public RECT rc;
            public RECT rcPage;
            public CHARRANGE chrg;
        }
        private const double inch = 14.4;

        public static void RtbToBitmap(SC2TV.RTFControl.ExRichTextBox rtb, string fileName) 
        {
            //Rectangle rectangle = rtb.;            
            Bitmap bmp = new Bitmap(rtb.ClientRectangle.Width,rtb.ClientRectangle.Height+20);
            using (Graphics gr = Graphics.FromImage(bmp)) 
            {
                IntPtr hDC = gr.GetHdc();
                FORMATRANGE fmtRange;
                RECT rect;
                int fromAPI;

                rect.Top = 0; rect.Left = 0;
                rect.Bottom = (int)(bmp.Height + (bmp.Height * (bmp.HorizontalResolution/100)) * inch);
                rect.Right = (int)(bmp.Width + (bmp.Width * (bmp.VerticalResolution / 100)) * inch);
                fmtRange.chrg.cpMin = rtb.GetCharIndexFromPosition(new Point(0,0));
                fmtRange.chrg.cpMax = rtb.Text.Length;
                //Debug.Print(String.Format("x:{0}, y:{1}, len:{2}",fmtRange.chrg.cpMin,fmtRange.chrg.cpMax,rtb.Text.Length));

                fmtRange.hdc = hDC;
                fmtRange.hdcTarget = hDC;
                fmtRange.rc = rect;
                fmtRange.rcPage = rect;
                int wParam = 1;
                IntPtr lParam = Marshal.AllocCoTaskMem(Marshal.SizeOf(fmtRange));
                Marshal.StructureToPtr(fmtRange, lParam, false);
                fromAPI = SendMessage(rtb.Handle, EM_FORMATRANGE, wParam, lParam);
                Marshal.FreeCoTaskMem(lParam);
                fromAPI = SendMessage(rtb.Handle, EM_FORMATRANGE, wParam, new IntPtr(0));
                gr.ReleaseHdc(hDC);
            }
            bmp.Save(fileName, System.Drawing.Imaging.ImageFormat.Jpeg);
            bmp.Dispose();
        }
    }
}
