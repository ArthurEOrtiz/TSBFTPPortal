using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.IO;

namespace TSBFTPPortal.ViewModels
{
  public class SelectCountyViewModel
  {
    public ObservableCollection<string> CountyNames { get; } = new ObservableCollection<string>();

    public SelectCountyViewModel() 
    {
      LoadCountyNames();
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
