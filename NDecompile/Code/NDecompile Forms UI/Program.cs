using System;
using System.Windows.Forms;
using LittleNet.NDecompile.FormsUI.Models;
using LittleNet.NDecompile.FormsUI.Views;

namespace LittleNet.NDecompile.FormsUI
{
	internal static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

			// Set the compatibility stuff
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			// Create a model and a view and link them
            using (MainForm mainForm = new MainForm())
            {
                mainForm.CreateControl();
                mainForm.Visible = true;
                using (MainModel mainModel = new MainModel())
                {
                    mainForm.MainModel = mainModel;

                    // Run
                    Application.Run(mainForm);
                }
            }

            AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledException;
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception exception = e.ExceptionObject as Exception;
            if (exception == null)
                return;

            using (ExceptionForm exceptionForm = new ExceptionForm())
                exceptionForm.ShowDialog(exception);
        }
	}
}