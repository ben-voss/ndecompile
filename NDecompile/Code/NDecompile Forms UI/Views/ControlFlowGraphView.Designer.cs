namespace LittleNet.NDecompile.FormsUI.Views
{
	partial class ControlFlowGraphView
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
			this._vScrollBar = new System.Windows.Forms.VScrollBar();
			this._hScrollBar = new System.Windows.Forms.HScrollBar();
			this.SuspendLayout();
			// 
			// _vScrollBar
			// 
			this._vScrollBar.LargeChange = 50;
			this._vScrollBar.Location = new System.Drawing.Point(267, 0);
			this._vScrollBar.Name = "_vScrollBar";
			this._vScrollBar.Size = new System.Drawing.Size(17, 200);
			this._vScrollBar.TabIndex = 0;
			this._vScrollBar.ValueChanged += new System.EventHandler(this._vScrollBar_ValueChanged);
			// 
			// _hScrollBar
			// 
			this._hScrollBar.Location = new System.Drawing.Point(0, 220);
			this._hScrollBar.Name = "_hScrollBar";
			this._hScrollBar.Size = new System.Drawing.Size(80, 17);
			this._hScrollBar.TabIndex = 1;
			this._hScrollBar.ValueChanged += new System.EventHandler(this._hScrollBar_ValueChanged);
			// 
			// ControlFlowGraphView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._hScrollBar);
			this.Controls.Add(this._vScrollBar);
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Name = "ControlFlowGraphView";
			this.Size = new System.Drawing.Size(348, 274);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.VScrollBar _vScrollBar;
		private System.Windows.Forms.HScrollBar _hScrollBar;
	}
}