using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using TSBFTPPortal.Models;
using TSBFTPPortal.Services;

namespace TSBFTPPortal.ViewModels
{
	public class CamaScriptsTreeViewViewModel : ViewModelBase
	{
		public County SelectedCounty { get; }
		public readonly FtpService _ftpService;
		public SearchBarViewModel SearchBarViewModel { get; }

		public CamaScriptsTreeViewViewModel(County selectedCounty, FtpService ftpService, SearchBarViewModel searchBarViewModel)
		{
			SelectedCounty = selectedCounty;
			_ftpService = ftpService;
			Directories = new ObservableCollection<DirectoryItemViewModel>();
			SearchBarViewModel = searchBarViewModel;
			LoadDirectoriesAndFoldersFromFTP();
		}

		private void LoadDirectoriesAndFoldersFromFTP()
		{
			string rootPath = GetRootPath();

			ObservableCollection<DirectoryItemViewModel> items = _ftpService.LoadDirectoriesAndFilesFromFTP(rootPath);

			foreach (DirectoryItemViewModel item in items)
			{
				if (item.Path != null)
				{
					
					if (item.IsDirectory)
					{
						// deconstruct the item and only add child items that have 
						// and extension of `.sql` or are a directory
						// to add a directory you use `Directories.Add(item);`
						FilterChildItems(item);
						Directories.Add(item);
						item.AddDefaultChildIfEmpty();
						
					}
					else
					{
						Directories.Add(item);
					}
				}
				else
				{
					Log.Error($"Cama Scripts, Could not find path for {item.Name}!");
				}
			}
		}

		private void FilterChildItems(DirectoryItemViewModel item)
		{
			var copyOfItems = new List<DirectoryItemViewModel>(item.Items);

			foreach (var childItem in copyOfItems)
			{
				if (!IsScriptFile(childItem.Name) && childItem.IsFile)
				{
					item.Items.Remove(childItem);
				}
				else if (childItem.IsDirectory)
				{
					FilterChildItems(childItem);
				}
			}
		}

		private bool IsScriptFile(string? filePath)
		{
			string fileExtension = Path.GetExtension(filePath);
			string[] scriptExtensions = { ".sql" };
			return scriptExtensions.Contains(fileExtension, StringComparer.OrdinalIgnoreCase);
		}

		private string GetRootPath()
		{
			string rootPath = string.Empty;
			if (SelectedCounty != null && SelectedCounty.CAMASystem != null)
			{
				rootPath = $"/FTP_DASHBOARD/CAMA/{SelectedCounty.CAMASystem.ToUpper()}/SCRIPTS/";
			}
			else
			{
				Log.Error("Cama Scrips, Select County is null");
			}

			return rootPath;
		}
	}
}
