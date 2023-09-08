using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Windows;
using TSBFTPPortal.Models;
using TSBFTPPortal.Services;
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
		private readonly string LogFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "TSBFTPPortal", "Log");
		private FtpService? _ftpService;

		protected async override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);
			
			ConfigureLogger();

			Log.Information($"\nProgram Started\n***************************");

			InitializeLocalAppDataFolder();
			ConfigureAppSettings();
			
			await InitializeCountyDataBaseAsync();

			if (Configuration != null)
			{
				Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary
				{
					Source = new Uri("/Themes/LightTheme.xaml", UriKind.Relative)
				});
				Window selectCountyView = new SelectCountyView(Configuration);
				selectCountyView.Show();
			}
			else
			{
				Log.Error($"Configuration is null");
			}

			Current.Exit += Current_Exit;
		}

		private static void ConfigureLogger()
		{
			string loggerFilePath = Path.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "TSBFTPPortal/Log", "log.txt");

			Log.Logger = new LoggerConfiguration()
				.MinimumLevel.Debug()
				.WriteTo.File(loggerFilePath, rollingInterval: RollingInterval.Day)
				.CreateLogger();

			Log.Information("Logger Configured.");
		}

		private void ConfigureAppSettings()
		{
			Log.Information("Configuring App settings.");
			Configuration = new ConfigurationBuilder()
					.SetBasePath(Directory.GetCurrentDirectory())
					.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
					.Build();

			string? ftpServer = Configuration["FtpSettings:Server"];
			string? userName = Configuration["FtpSettings:UserName"];
			string? password = Configuration["FtpSettings:Password"];

			if (ftpServer != null && userName != null && password != null)
			{
				_ftpService = new FtpService(ftpServer, userName, password);
			}
		}

		private void InitializeLocalAppDataFolder()
		{

			Log.Information("Initializing Local App Folder");

			string commonAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
			string dataFolderPath = Path.Combine(commonAppDataPath, "TSBFTPPortal");

			if (!Directory.Exists(dataFolderPath))
			{
				Log.Error("App Folder does not exist, creating directories!");
				try
				{

					// Create the root directory "FTPDashboard"
					Directory.CreateDirectory(dataFolderPath);
					Log.Information($"Created root directory at : {dataFolderPath}");

				}
				catch (Exception ex)
				{
					Log.Error($"Failed to create root directory: {ex.Message}");
				}			
			}

			if (!Directory.Exists(ReportsFolderPath))
			{
				try
				{
					// Create the reports folder 
					Directory.CreateDirectory(ReportsFolderPath);
					Log.Information("Created reports directory");
				}
				catch (Exception ex)
				{
					Log.Error($"Failed to create reports directory: {ex.Message}");
				}
			}

			if(!Directory.Exists(ScriptsFolderPath))
			{
				try
				{
					// Create the scripts folder 
					Directory.CreateDirectory(ScriptsFolderPath);
					Log.Information("Created scripts directory.");
				}
				catch (Exception ex)
				{
					Log.Error($"Failed to create scripts directory: {ex.Message}");
				}
			}
			
			if (!Directory.Exists(LogFolderPath))
			{
				try
				{
					// Create the Log Folder 
					Directory.CreateDirectory(LogFolderPath);
					Log.Information("Created log directory.");
				}
				catch (Exception ex)
				{
					Log.Error($"Failed to create log directory: {ex.Message}");
				}
			}
		
		}

		private async Task InitializeCountyDataBaseAsync()
		{
			Log.Information("Initializing County Data Base");
			string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "TSBFTPPortal", "Counties.db");

			if (File.Exists(dbPath))
			{
				// Check if the database content matches the expected JSON data
				string expectedJsonData = await ReadJsonFileAsync("/FTP_DASHBOARD/countyData.json");
				string existingJsonData = await ReadDataFromDatabase(dbPath);

				// Normalize JSON strings by removing spaces and tabs
				expectedJsonData = RemoveWhitespace(expectedJsonData);
				existingJsonData = RemoveWhitespace(existingJsonData);

				if (expectedJsonData != existingJsonData)
				{
					Log.Information("Database content does not match the expected JSON data. Re-populating the database.");
					PopulateDataBase(dbPath, expectedJsonData);
				}
				else
				{
					Log.Information("Database content matches the expected JSON data. No need to re-populate.");
				}
			}
			else
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
				PopulateDataBase(dbPath, await ReadJsonFileAsync("/FTP_DASHBOARD/countyData.json"));
			}
		}

		private string RemoveWhitespace(string input)
		{
			return new string(input.Where(c => !char.IsWhiteSpace(c)).ToArray());
		}

		private static Task<string> ReadDataFromDatabase(string dbPath)
		{
			using (var connection = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
			{
				connection.Open();

				string selectQuery = "SELECT Name, AdminSystem, CAMASystem FROM Counties";
				using var command = new SQLiteCommand(selectQuery, connection);
				using var reader = command.ExecuteReader();

				List<County> counties = new List<County>();

				while (reader.Read())
				{
					counties.Add(new County
					{
						Name = reader.GetString(0),
						AdminSystem = reader.GetString(1),
						CAMASystem = reader.GetString(2)
					});
				}
				
				var customData = counties.Select(c => new
				{
					Name = c.Name,
					AdminSystem = c.AdminSystem,
					CAMASystem = c.CAMASystem
				}).ToList();

				string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(customData, Newtonsoft.Json.Formatting.Indented);
				return Task.FromResult(jsonData);
			}
		}

		public async Task<string> ReadJsonFileAsync(string path)
		{
			string? jsonContent = string.Empty;

			if (_ftpService != null)
			{
				jsonContent = await _ftpService.ReadJsonFileFromFTPAsync(path);
			}
			else
			{
				Log.Error("Ftp Service is null!");
			}
		
			if (jsonContent != null)
			{
				return jsonContent;
			}
			else
			{
				Log.Error("Error reading JSON file!");
				return string.Empty;
			}
		}

		private void PopulateDataBase(string dbPath, string jsonData)
		{
			List<County>? counties = JsonConvert.DeserializeObject<List<County>>(jsonData);

			using (var connection = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
			{
				connection.Open();

				// Clear existing data in the Counties table
				string clearTableQuery = "DELETE FROM Counties";
				using (var clearCommand = new SQLiteCommand(clearTableQuery, connection))
				{
					clearCommand.ExecuteNonQuery();
				}

				foreach (var county in counties)
				{
					// Insert the updated records into the database
					string insertQuery = "INSERT INTO Counties (Name, AdminSystem, CAMASystem) VALUES (@Name, @AdminSystem, @CAMASystem)";
					using (var insertCommand = new SQLiteCommand(insertQuery, connection))
					{
						insertCommand.Parameters.AddWithValue("@Name", county.Name);
						insertCommand.Parameters.AddWithValue("@AdminSystem", county.AdminSystem);
						insertCommand.Parameters.AddWithValue("@CAMASystem", county.CAMASystem);
						insertCommand.ExecuteNonQuery();
					}
				}
			}
		}

		private void Current_Exit(object sender, ExitEventArgs e)
		{
			Log.Information("Exiting App.");

			DeleteDirectoryContents(ReportsFolderPath);
			DeleteDirectoryContents(ScriptsFolderPath);
			DeleteOldLogs(LogFolderPath);


			// Close and flush the logger
			Log.Information($"\nProgram Closed\n***************************");
			Log.CloseAndFlush();

			Current.Shutdown();
		}

		private void DeleteOldLogs(string logFolderPath)
		{
			try
			{
				DirectoryInfo directoryInfo = new DirectoryInfo(logFolderPath);

				FileInfo[] files = directoryInfo.GetFiles();

				DateTime cutoffDate = DateTime.Now.AddDays(-7);

				foreach (FileInfo file in files)
				{
					if (file.LastWriteTime < cutoffDate)
					{
						// File is older than a week, so delete it
						file.Delete();
						Log.Information($"Deleted old log file: {file.Name}");
					}
				}
			}
			catch (Exception ex)
			{
				Log.Error($"Error deleting old log files: {ex.Message}");
			}
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
