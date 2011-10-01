namespace LittleNet.NDecompile.FormsUI.Views
{
	partial class FindAssemblyForm
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
			this._okButton = new System.Windows.Forms.Button();
			this._cancelButton = new System.Windows.Forms.Button();
			this._assemblyNameLabel = new System.Windows.Forms.Label();
			this._openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this._pathText = new System.Windows.Forms.TextBox();
			this._findFileButton = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// _okButton
			// 
			this._okButton.Location = new System.Drawing.Point(326, 123);
			this._okButton.Name = "_okButton";
			this._okButton.Size = new System.Drawing.Size(75, 23);
			this._okButton.TabIndex = 0;
			this._okButton.Text = "OK";
			this._okButton.UseVisualStyleBackColor = true;
			this._okButton.Click += new System.EventHandler(this._okButton_Click);
			// 
			// _cancelButton
			// 
			this._cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this._cancelButton.Location = new System.Drawing.Point(244, 123);
			this._cancelButton.Name = "_cancelButton";
			this._cancelButton.Size = new System.Drawing.Size(75, 23);
			this._cancelButton.TabIndex = 1;
			this._cancelButton.Text = "Cancel";
			this._cancelButton.UseVisualStyleBackColor = true;
			this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
			// 
			// _assemblyNameLabel
			// 
			this._assemblyNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._assemblyNameLabel.Location = new System.Drawing.Point(12, 32);
			this._assemblyNameLabel.Name = "_assemblyNameLabel";
			this._assemblyNameLabel.Size = new System.Drawing.Size(389, 38);
			this._assemblyNameLabel.TabIndex = 2;
			this._assemblyNameLabel.Text = "Assembly Name";
			// 
			// _openFileDialog
			// 
			this._openFileDialog.DefaultExt = "*.dll";
			this._openFileDialog.Filter = "Assemblies|*.dll|All files|*.*";
			this._openFileDialog.Title = "Find Assembly";
			// 
			// _pathText
			// 
			this._pathText.Location = new System.Drawing.Point(25, 84);
			this._pathText.Name = "_pathText";
			this._pathText.Size = new System.Drawing.Size(336, 21);
			this._pathText.TabIndex = 3;
			// 
			// _findFileButton
			// 
			this._findFileButton.Location = new System.Drawing.Point(376, 81);
			this._findFileButton.Name = "_findFileButton";
			this._findFileButton.Size = new System.Drawing.Size(25, 23);
			this._findFileButton.TabIndex = 4;
			this._findFileButton.Text = "...";
			this._findFileButton.UseVisualStyleBackColor = true;
			this._findFileButton.Click += new System.EventHandler(this._findFileButton_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(271, 13);
			this.label1.TabIndex = 5;
			this.label1.Text = "Unable to locate the assembly with the following name:";
			// 
			// FindAssemblyForm
			// 
			this.AcceptButton = this._okButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this._cancelButton;
			this.ClientSize = new System.Drawing.Size(413, 159);
			this.Controls.Add(this.label1);
			this.Controls.Add(this._findFileButton);
			this.Controls.Add(this._pathText);
			this.Controls.Add(this._assemblyNameLabel);
			this.Controls.Add(this._cancelButton);
			this.Controls.Add(this._okButton);
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FindAssemblyForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Find Assembly";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button _okButton;
		private System.Windows.Forms.Button _cancelButton;
		private System.Windows.Forms.Label _assemblyNameLabel;
		private System.Windows.Forms.OpenFileDialog _openFileDialog;
		private System.Windows.Forms.TextBox _pathText;
		private System.Windows.Forms.Button _findFileButton;
		private System.Windows.Forms.Label label1;
	}
}