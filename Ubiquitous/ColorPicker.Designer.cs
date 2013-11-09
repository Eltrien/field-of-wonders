namespace Ubiquitous
{
    partial class ColorPicker
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
            this.buttonColor = new System.Windows.Forms.Button();
            this.label33 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // buttonColor
            // 
            this.buttonColor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonColor.BackColor = System.Drawing.Color.Black;
            this.buttonColor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonColor.Location = new System.Drawing.Point(71, 5);
            this.buttonColor.Name = "buttonColor";
            this.buttonColor.Size = new System.Drawing.Size(20, 20);
            this.buttonColor.TabIndex = 13;
            this.buttonColor.UseVisualStyleBackColor = false;
            this.buttonColor.Click += new System.EventHandler(this.buttonColor_Click);
            // 
            // label33
            // 
            this.label33.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label33.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label33.Location = new System.Drawing.Point(3, 9);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(62, 13);
            this.label33.TabIndex = 12;
            this.label33.Text = "Background:";
            // 
            // ColorPicker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.buttonColor);
            this.Controls.Add(this.label33);
            this.Name = "ColorPicker";
            this.Size = new System.Drawing.Size(96, 30);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonColor;
        private System.Windows.Forms.Label label33;

    }
}
