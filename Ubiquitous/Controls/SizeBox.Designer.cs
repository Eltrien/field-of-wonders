namespace Ubiquitous
{
    partial class SizeBox
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
            this.textHeight = new Ubiquitous.IntTextBox();
            this.textWidth = new Ubiquitous.IntTextBox();
            this.label35 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // textHeight
            // 
            this.textHeight.IntValue = 0;
            this.textHeight.Location = new System.Drawing.Point(71, 3);
            this.textHeight.Name = "textHeight";
            this.textHeight.Size = new System.Drawing.Size(45, 20);
            this.textHeight.TabIndex = 4;
            this.textHeight.Text = "0";
            this.textHeight.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textWidth
            // 
            this.textWidth.IntValue = 0;
            this.textWidth.Location = new System.Drawing.Point(2, 3);
            this.textWidth.Name = "textWidth";
            this.textWidth.Size = new System.Drawing.Size(45, 20);
            this.textWidth.TabIndex = 2;
            this.textWidth.Text = "0";
            this.textWidth.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label35
            // 
            this.label35.AutoSize = true;
            this.label35.Location = new System.Drawing.Point(53, 6);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(12, 13);
            this.label35.TabIndex = 3;
            this.label35.Text = "x";
            // 
            // SizeBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.textHeight);
            this.Controls.Add(this.textWidth);
            this.Controls.Add(this.label35);
            this.Name = "SizeBox";
            this.Size = new System.Drawing.Size(119, 25);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private IntTextBox textHeight;
        private IntTextBox textWidth;
        private System.Windows.Forms.Label label35;
    }
}
