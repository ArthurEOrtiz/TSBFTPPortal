using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using TSBFTPPortal.Views;
using Serilog;

namespace TSBFTPPortal
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			Window selectCountyView = new SelectCountyView();
			selectCountyView.Show();

			ConfigureLogger();
			InitializeLocalAppDataFolder();
			
		}

		private void ConfigureLogger()
		{
			string loggerFilePath = Path.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),"TSBFTPPortal/Log", "log.txt");

			Log.Logger = new LoggerConfiguration()
				.MinimumLevel.Debug()
				.WriteTo.File(loggerFilePath, rollingInterval: RollingInterval.Day)
				.CreateLogger();
		}

		private void InitializeLocalAppDataFolder()
		{
			string commonAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
			string dataFolderPath = System.IO.Path.Combine(commonAppDataPath, "TSBFTPPortal");

			if (!Directory.Exists(dataFolderPath))
			{
				try
				{
					Directory.CreateDirectory(dataFolderPath);

					// Create the root directory "FTPDashboard"
					Directory.CreateDirectory(dataFolderPath);

					// Create the reports folder 
					string reportFolderPath = Path.Combine(dataFolderPath, "Reports");
					Directory.CreateDirectory(reportFolderPath);

					// Create the scripts folder 
					string scriptsFolderPath = Path.Combine(dataFolderPath, "Scripts");
					Directory.CreateDirectory(scriptsFolderPath);

					// Create the Log Folder 
					string reportsFolderPath = Path.Combine(dataFolderPath, "Log");
					Directory.CreateDirectory(reportsFolderPath);

					Log.Information("App folders created");

				}
				catch (Exception ex) 
				{
					Log.Error($"Failed to create directory: {ex.Message}");
				}
			}

		}
	}
}
