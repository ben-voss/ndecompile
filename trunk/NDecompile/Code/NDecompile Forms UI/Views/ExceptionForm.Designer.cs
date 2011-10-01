namespace LittleNet.NDecompile.FormsUI.Views
{
    partial class ExceptionForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this._exceptionMessageLabel = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this._exceptionDetailText = new System.Windows.Forms.TextBox();
            this._okButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // _exceptionMessageLabel
            // 
            this._exceptionMessageLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._exceptionMessageLabel.AutoEllipsis = true;
            this._exceptionMessageLabel.Location = new System.Drawing.Point(51, 13);
            this._exceptionMessageLabel.Name = "_exceptionMessageLabel";
            this._exceptionMessageLabel.Size = new System.Drawing.Size(418, 32);
            this._exceptionMessageLabel.TabIndex = 0;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(13, 13);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(32, 32);
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // _exceptionDetailText
            // 
            this._exceptionDetailText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._exceptionDetailText.Enabled = false;
            this._exceptionDetailText.Location = new System.Drawing.Point(13, 52);
            this._exceptionDetailText.Multiline = true;
            this._exceptionDetailText.Name = "_exceptionDetailText";
            this._exceptionDetailText.Size = new System.Drawing.Size(457, 202);
            this._exceptionDetailText.TabIndex = 2;
            // 
            // _okButton
            // 
            this._okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._okButton.Location = new System.Drawing.Point(394, 260);
            this._okButton.Name = "_okButton";
            this._okButton.Size = new System.Drawing.Size(75, 23);
            this._okButton.TabIndex = 3;
            this._okButton.Text = "OK";
            this._okButton.UseVisualStyleBackColor = true;
            this._okButton.Click += new System.EventHandler(this._okButton_Click);
            // 
            // ExceptionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(482, 295);
            this.Controls.Add(this._okButton);
            this.Controls.Add(this._exceptionDetailText);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this._exceptionMessageLabel);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.Name = "ExceptionForm";
            this.Text = "Unhandled Exception";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label _exceptionMessageLabel;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.TextBox _exceptionDetailText;
        private System.Windows.Forms.Button _okButton;
    }
}