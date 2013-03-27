namespace Ubiquitous
{
    partial class CounterPanel
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.labelViewers = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // labelViewers
            // 
            this.labelViewers.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.labelViewers.AutoEllipsis = true;
            this.labelViewers.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.labelViewers.Cursor = System.Windows.Forms.Cursors.SizeAll;
            this.labelViewers.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.labelViewers.Font = new System.Drawing.Font("Arial Narrow", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelViewers.ForeColor = System.Drawing.Color.White;
            this.labelViewers.Location = new System.Drawing.Point(18, 0);
            this.labelViewers.Margin = new System.Windows.Forms.Padding(0);
            this.labelViewers.Name = "labelViewers";
            this.labelViewers.Size = new System.Drawing.Size(24, 16);
            this.labelViewers.TabIndex = 0;
            this.labelViewers.Text = "0";
            this.labelViewers.MouseDown += new System.Windows.Forms.MouseEventHandler(this.labelViewers_MouseDown);
            this.labelViewers.MouseMove += new System.Windows.Forms.MouseEventHandler(this.labelViewers_MouseMove);
            this.labelViewers.MouseUp += new System.Windows.Forms.MouseEventHandler(this.labelViewers_MouseUp);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.pictureBox1.Cursor = System.Windows.Forms.Cursors.SizeAll;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(16, 16);
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.labelViewers_MouseDown);
            this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.labelViewers_MouseMove);
            this.pictureBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.labelViewers_MouseUp);
            // 
            // CounterPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.labelViewers);
            this.ForeColor = System.Drawing.Color.DimGray;
            this.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.Name = "CounterPanel";
            this.Size = new System.Drawing.Size(43, 17);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelViewers;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}
