
using System;
using System.Reflection;
using System.Threading;

namespace LittleNet.NDecompile.Console
{
	public class Program
	{

		static int Main(string[] args)
		{
			Thread.CurrentThread.Name = "Main";

			Options options;
			if (ParseArguments(args, out options))
				return -1;

			// Load all the assemblies
			foreach (String assembly in options.Assemblies)
				AssemblyManager.Load(assembly);

			

			return 0;
		}

		private static void PrintLogo()
		{
			// Get the assembly copyright
			object[] attributes = Assembly.GetEntryAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
			String copyright = String.Empty;
			if (attributes.Length > 0)
				copyright = ((AssemblyCopyrightAttribute)attributes[0]).Copyright;

			Version ver = new Version();
			attributes = Assembly.GetEntryAssembly().GetCustomAttributes(typeof(AssemblyVersionAttribute), false);
			if (attributes.Length > 0)
				ver = new Version(((AssemblyVersionAttribute)attributes[0]).Version);

			// Write a version string
			System.Console.WriteLine("NDecompile v{0}.{1}.{2}.{3}\n" + copyright + ".  All rights reserved.\n",
							  ver.Major, ver.Minor, ver.Build, ver.Revision);
		}

		private static void PrintUsage()
		{
			PrintLogo();
			System.Console.WriteLine("Syntax: NDecompile [Options] [type] [assembly list]");
			System.Console.WriteLine("Options:");
			System.Console.WriteLine("\t/language:Language\tThe language to decompile into");
			System.Console.WriteLine("\t/? or /help\t\tDisplay this usage message");
			System.Console.WriteLine("");
		}

		private static void WriteErrorMessage(String message)
		{
			System.Console.WriteLine(message);
		}

		private static void WriteErrorMessage(String message, Exception e)
		{
			System.Console.WriteLine(message + e.Message);
		}

		private static bool ParseArguments(String[] args, out Options options)
		{
			options = new Options();

			// Parse the command line into its constituent parts
			CommandLine commandLine;
			try
			{
				commandLine = new CommandLine(args, new string[] { "*language", "?", "help" });
			}
			catch (ApplicationException e)
			{
				PrintLogo();
				WriteErrorMessage(null, e);
				return false;
			}

			if ((commandLine.NumArguments + commandLine.NumOptions) < 1)
			{
				// There are no arguments or options
				PrintUsage();
				return false;
			}

			CommandLineOption option;
			while ((option = commandLine.GetNextOption()) != null)
			{
				switch (option.Name)
				{


				}
			}

			return true;
		}

	}
}