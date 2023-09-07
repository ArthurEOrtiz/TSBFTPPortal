using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Windows;
using TSBFTPPortal.Models;
using TSBFTPPortal.Views;


namespace TSBFTPPortal
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private IConfiguration? Configuration;
		private readonly string ReportsFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "TSBFTPPortal", "Reports");
		private readonly string ScriptsFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "TSBFTPPortal", "Scripts");


		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			ConfigureLogger();
			Log.Information($"\nProgram Started\n***************************");
			InitializeLocalAppDataFolder();
			InitializeCountyDataBase();
			ConfigureAppSettings();
			

			if (Configuration != null)
			{
				
				Window selectCountyView = new SelectCountyView(Configuration);
				selectCountyView.Show();
			}
			else
			{
				Log.Error($"Configuration is null");
			}


			Application.Current.Exit += Current_Exit;
		}

		private void InitializeLocalAppDataFolder()
		{

			Log.Information("Initializing Local App Folder");

			string commonAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
			string dataFolderPath = Path.Combine(commonAppDataPath, "TSBFTPPortal");

			if (!Directory.Exists(dataFolderPath))
			{
				try
				{
					Directory.CreateDirectory(dataFolderPath);

					// Create the root directory "FTPDashboard"
					Directory.CreateDirectory(dataFolderPath);

					// Create the reports folder 
					Directory.CreateDirectory(ReportsFolderPath);

					// Create the scripts folder 
					Directory.CreateDirectory(ScriptsFolderPath);

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

		private static void InitializeCountyDataBase()
		{
			Log.Information("Initializing County Data Base");
			string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "TSBFTPPortal", "Counties.db");

			if (!File.Exists(dbPath))
			{
				using (var connection = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
				{
					connection.Open();

					// Create the table
					string createTableQuery = @"
									CREATE TABLE IF NOT EXISTS Counties (
											Name TEXT,
											AdminSystem TEXT,
											CAMASystem TEXT
									);";

					using var command = new SQLiteCommand(createTableQuery, connection);
					command.ExecuteNonQuery();
				}
				// Populate the table with data from the JSON file
				PopulateDataBase(dbPath);
			}
		}

		private static void PopulateDataBase(string dbPath)
		{
			string jsonFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "countiesInitialData.json");
			string jsonData = File.ReadAllText(jsonFilePath);

			var counties = Newtonsoft.Json.JsonConvert.DeserializeObject<List<County>>(jsonData);

			using var connection = new SQLiteConnection($"Data Source={dbPath};Version=3;");
			connection.Open();

			if (counties != null)
			{
				foreach (var county in counties)
				{
					string insertQuery = @"
											INSERT INTO Counties (Name, AdminSystem, CAMASystem)
											VALUES (@Name, @AdminSystem, @CAMASystem);";

					using var command = new SQLiteCommand(insertQuery, connection);
					command.Parameters.AddWithValue("@Name", county.Name);
					command.Parameters.AddWithValue("@AdminSystem", county.AdminSystem);
					command.Parameters.AddWithValue("@CAMASystem", county.CAMASystem);

					command.ExecuteNonQuery();
				}
			}
			else
			{
				Log.Error("Counties failed to de-serialize from JSON file.");
			}
		}

		private void ConfigureAppSettings()
		{
			Log.Information("Configuring App settings.");
			Configuration = new ConfigurationBuilder()
					.SetBasePath(Directory.GetCurrentDirectory())
					.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
					.Build();
			Log.Information($"Configuration: {Configuration}");
		}

		private static void ConfigureLogger()
		{
			Log.Information("Configuring logger.");
			string loggerFilePath = Path.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "TSBFTPPortal/Log", "log.txt");

			Log.Logger = new LoggerConfiguration()
				.MinimumLevel.Debug()
				.WriteTo.File(loggerFilePath, rollingInterval: RollingInterval.Day)
				.CreateLogger();
		}

		private void Current_Exit(object sender, ExitEventArgs e)
		{
			Log.Information("Exiting App.");

			DeleteDirectoryContents(ReportsFolderPath);
			DeleteDirectoryContents(ScriptsFolderPath);

			// Close and flush the logger
			Log.Information($"\nProgram Closed\n***************************");
			Log.CloseAndFlush();

			Application.Current.Shutdown();
		}

		private static void DeleteDirectoryContents(string directoryPath)
		{
			Log.Information($"Deleting files at : {directoryPath}");
			if (Directory.Exists(directoryPath))
			{
				// Delete all files inside the directory
				string[] files = Directory.GetFiles(directoryPath);
				foreach (string file in files)
				{
					File.Delete(file);
					Log.Information($"Deleted: {file}");
				}

				// Delete all directories inside the directory (recursively)
				string[] directories = Directory.GetDirectories(directoryPath);
				foreach (string directory in directories)
				{
					Directory.Delete(directory, true);
					Log.Information($"Deleted: {directory}");
				}
			}
		}

		



	
	}
}
