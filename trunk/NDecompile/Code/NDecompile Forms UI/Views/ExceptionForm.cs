using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LittleNet.NDecompile.FormsUI.Views
{
    public partial class ExceptionForm : Form
    {
        public ExceptionForm()
        {
            InitializeComponent();
        }

        public void ShowDialog(Exception exception)
        {
            _exceptionMessageLabel.Text = exception.Message;
            _exceptionDetailText.Text = exception.ToString();
            ShowDialog();
        }

        private void _okButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
