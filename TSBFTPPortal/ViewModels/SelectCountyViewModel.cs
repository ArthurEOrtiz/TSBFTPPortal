using Microsoft.Extensions.Configuration;
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
    public ObservableCollection<string> CountyNames { get; } = new ObservableCollection<string>();

    private string _selectedCounty;
    public string SelectedCounty
    {
      get { return _selectedCounty; }
      set
      {
        _selectedCounty = value;
        OnPropertyChanged(nameof(SelectedCounty));
      }
    }

    public ICommand ContinueToMainPageCommand { get; private set; }

    private readonly IConfiguration _configuration;

    public SelectCountyViewModel(IConfiguration configuration) 
    {
      _configuration = configuration;

      ContinueToMainPageCommand = new RelayCommand(ContinueToMainPage);
      LoadCountyNames();
    }

		private void ContinueToMainPage(object obj)
		{
      County selectedCountyModel = FindCountyModel(SelectedCounty);

			var mainWindowViewModel = new MainWindowViewModel(selectedCountyModel, _configuration);
			var mainWindow = new MainWindow { DataContext = mainWindowViewModel };

			var currentWindow = Application.Current.MainWindow; // Get a reference to the current window
			currentWindow.Close(); // Close the current window
			mainWindow.Show();

		}

		private County FindCountyModel(string countyName)
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
            string countyName = reader["Name"].ToString();
            CountyNames.Add(countyName);
          }
        }
      }
		}
	}
}
