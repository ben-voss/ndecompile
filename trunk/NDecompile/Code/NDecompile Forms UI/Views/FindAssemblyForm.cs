using System;
using System.IO;
using System.Windows.Forms;

namespace LittleNet.NDecompile.FormsUI.Views
{
	public partial class FindAssemblyForm : Form
	{
		private FileInfo _fileInfo;

		public FindAssemblyForm()
		{
			InitializeComponent();
		}

		public String AssemblyName
		{
			set
			{
				_assemblyNameLabel.Text = value;
			}
		}

		public FileInfo FileInfo
		{
			get
			{
				return _fileInfo;
			}
		}

		private void _cancelButton_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void _okButton_Click(object sender, EventArgs e)
		{
			_fileInfo = new FileInfo(_pathText.Text);

			Close();
		}

		private void _findFileButton_Click(object sender, EventArgs e)
		{
			if (_openFileDialog.ShowDialog(this) == DialogResult.OK)
				_pathText.Text = _openFileDialog.FileName;
			
		}
	}
}