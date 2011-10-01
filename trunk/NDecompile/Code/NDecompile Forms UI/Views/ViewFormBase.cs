using System;
using System.Windows.Forms;
using LittleNet.NDecompile.FormsUI.Interfaces;

namespace LittleNet.NDecompile.FormsUI.Views
{
	internal delegate void ModelChangeEventHandler(Object hint);

	internal class ViewFormBase : Form, IObserver
	{
		private IObservable _model;

		protected event ModelChangeEventHandler ModelChange;

		/// <summary>
		/// Invokes the model change event
		/// </summary>
		/// <param name="hint"></param>
		protected virtual void OnModelChange(Object hint)
		{
			if (ModelChange != null)
				ModelChange(hint);
		}

		#region IObserver Members

		/// <summary>
		/// Handle notfications from the model.
		/// </summary>
		/// <remarks>
		/// Handles the thread marshalling onto the GUI thread and invokes the change event
		/// </remarks>
		/// <param name="hint"></param>
		void IObserver.Notify(object hint)
		{
			if (InvokeRequired)
				BeginInvoke(new ModelChangeEventHandler(OnModelChange), hint);
			else
				OnModelChange(hint);
		}

		#endregion

		#region Model

		public IObservable Model
		{
			get
			{
				return _model;
			}
			set
			{
				if (_model != null)
					_model.RemoveObserver(this);

				_model = value;

				if (_model != null)
					_model.AddObserver(this);
			}
		}

		#endregion

		protected override void Dispose(bool disposing)
		{
			Model = null;

			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			this.SuspendLayout();
			// 
			// ViewFormBase
			// 
			this.ClientSize = new System.Drawing.Size(292, 266);
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Name = "ViewFormBase";
			this.ResumeLayout(false);

		}

	}
}