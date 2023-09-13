using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.IO;
using System.Windows;
using System.Windows.Input;
using TSBFTPPortal.Commands;
using TSBFTPPortal.Models;

namespace TSBFTPPortal.ViewModels
{
  public class SelectCountyViewModel : ViewModelBase
  {
    public ObservableCollection<string> CountyNames { get; } 

    private string _selectedCounty;
    public string SelectedCounty
    {
      get => _selectedCounty; 
      set
      {
        _selectedCounty = value;
        OnPropertyChanged(nameof(SelectedCounty));

        Properties.Settings.Default.LastSelectedCounty = value;
        Properties.Settings.Default.Save();
      }
    }

    public ICommand ContinueToMainPageCommand { get; private set; }

    private readonly IConfiguration _configuration;

    public SelectCountyViewModel(IConfiguration configuration) 
    {
      _configuration = configuration;
      _selectedCounty = string.Empty;
      CountyNames = new ObservableCollection<string>();
      ContinueToMainPageCommand = new RelayCommand(ContinueToMainPage);
      LoadCountyNames();

			SelectedCounty = Properties.Settings.Default.LastSelectedCounty;
		}

		private void ContinueToMainPage(object obj)
		{
      County selectedCountyModel = FindCountyModel(SelectedCounty);

			var mainWindowViewModel = new MainWindowViewModel(selectedCountyModel, _configuration);
			var mainWindow = new MainWindow { DataContext = mainWindowViewModel };

			var currentWindow = Application.Current.MainWindow; // Get a reference to the current window

			currentWindow.Close(); // Close the current window
			mainWindow.Owner = Application.Current.MainWindow;
			mainWindow.Left = currentWindow.Left;
			mainWindow.Top = currentWindow.Top;
			mainWindow.Show();
			
		}

		private static County FindCountyModel(string countyName)
		{
      County? selectedCounty = null;

			string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "TSBFTPPortal", "Counties.db");

      using ( var connection = new SQLiteConnection($"Data Source={dbPath}; Version=3;"))
      {
        connection.Open();
        string query = $"SELECT * FROM Counties WHERE Name = '{countyName}';";

        using (var command = new SQLiteCommand(query, connection))
        using (var reader = command.ExecuteReader())
        {
          if (reader.Read())
          {
            selectedCounty = new County
            {
              Name = reader["Name"].ToString(),
              AdminSystem = reader["AdminSystem"].ToString(),
              CAMASystem = reader["CAMASystem"].ToString()
            };
          }
          else
          {
            selectedCounty = new County
            {
              Name = string.Empty,
              AdminSystem = string.Empty,
              CAMASystem = string.Empty,
            };
            Log.Error($"County {countyName} was not found in the database!");
          }
        }
      }

      return selectedCounty;
		}

		private void LoadCountyNames()
		{
      string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "TSBFTPPortal", "Counties.db");

      using (var connection = new SQLiteConnection($"Data Source={dbPath}; Version=3;"))
      {
        connection.Open();  
        string query = "SELECT Name FROM Counties;";

        using (var command = new SQLiteCommand(query, connection))
        using (var reader = command.ExecuteReader())
        {
          while (reader.Read())
          {
            string? countyName = reader["Name"].ToString();
            if (countyName != null)
            {
							CountyNames.Add(countyName);
						}
            else
            {
              Log.Error($"Error loading {countyName}!");
            }
          }
        }
      }
		}
	}
}
