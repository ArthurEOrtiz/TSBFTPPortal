﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using TSBFTPPortal.Views;
using Serilog;
using TSBFTPPortal.Models;
using System.Data.SQLite;

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
			InitializeCountyDataBase();
			
		}

		private void InitializeCountyDataBase()
		{
			string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "TSBFTPPortal", "Counties.db");

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
			string jsonFilePath = Path.Combine(Environment.CurrentDirectory, "counties.json");
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
