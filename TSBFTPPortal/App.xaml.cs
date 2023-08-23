using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using TSBFTPPortal.Views;
using Serilog;
using TSBFTPPortal.Models;
using System.Data.SQLite;
using Microsoft.Extensions.Configuration;


namespace TSBFTPPortal
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private IConfiguration Configuration;
		private readonly string ReportsFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "TSBFTPPortal", "Reports");
		private readonly string ScriptsFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "TSBFTPPortal", "Scripts");
		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			InitializeLocalAppDataFolder();
			InitializeCountyDataBase();
			ConfigureAppSettings();
			ConfigureLogger();
			
			Window selectCountyView = new SelectCountyView(Configuration);
			selectCountyView.Show();

			Current.Exit += Current_Exit;
		}

		private void Current_Exit(object sender, ExitEventArgs e)
		{

			DeleteDirectoryContents(ReportsFolderPath);
			DeleteDirectoryContents(ScriptsFolderPath);


			// Close and flush the logger
			Log.Information("Program Closed");
			Log.CloseAndFlush();
		}

		private void DeleteDirectoryContents(string directoryPath)
		{
			if (Directory.Exists(directoryPath))
			{
				// Delete all files inside the directory
				string[] files = Directory.GetFiles(directoryPath);
				foreach (string file in files)
				{
					File.Delete(file);
				}

				// Delete all directories inside the directory (recursively)
				string[] directories = Directory.GetDirectories(directoryPath);
				foreach (string directory in directories)
				{
					Directory.Delete(directory, true);
				}
			}
		}

		private void ConfigureAppSettings()
		{
			Configuration = new ConfigurationBuilder()
					.SetBasePath(Directory.GetCurrentDirectory())
					.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
					.Build();
		}

		private void InitializeCountyDataBase()
		{
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

					using (var command = new SQLiteCommand(createTableQuery, connection))
					{
						command.ExecuteNonQuery();
					}
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

			using (var connection = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
			{
				connection.Open();

				foreach (var county in counties)
				{
					string insertQuery = @"
                    INSERT INTO Counties (Name, AdminSystem, CAMASystem)
                    VALUES (@Name, @AdminSystem, @CAMASystem);";

					using (var command = new SQLiteCommand(insertQuery, connection))
					{
						command.Parameters.AddWithValue("@Name", county.Name);
						command.Parameters.AddWithValue("@AdminSystem", county.AdminSystem);
						command.Parameters.AddWithValue("@CAMASystem", county.CAMASystem);

						command.ExecuteNonQuery();
					}
				}
			}
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
	}
}
