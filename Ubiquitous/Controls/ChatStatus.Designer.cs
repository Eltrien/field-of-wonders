namespace Ubiquitous
{
    partial class ChatStatus
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
            this.label = new System.Windows.Forms.Label();
            this.picture = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.picture)).BeginInit();
            this.SuspendLayout();
            // 
            // label
            // 
            this.label.AutoSize = true;
            this.label.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label.Location = new System.Drawing.Point(13, 0);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(24, 13);
            this.label.TabIndex = 6;
            this.label.Text = "text";
            // 
            // picture
            // 
            this.picture.Image = global::Ubiquitous.Properties.Resources.checkMarkRed;
            this.picture.InitialImage = global::Ubiquitous.Properties.Resources.checkMarkRed;
            this.picture.Location = new System.Drawing.Point(1, 2);
            this.picture.Name = "picture";
            this.picture.Size = new System.Drawing.Size(10, 10);
            this.picture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picture.TabIndex = 5;
            this.picture.TabStop = false;
            // 
            // ChatStatus
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.picture);
            this.Controls.Add(this.label);
            this.Name = "ChatStatus";
            this.Size = new System.Drawing.Size(88, 15);
            this.VisibleChanged += new System.EventHandler(this.ChatStatus_VisibleChanged);
            ((System.ComponentModel.ISupportInitialize)(this.picture)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox picture;
        private System.Windows.Forms.Label label;
    }
}
