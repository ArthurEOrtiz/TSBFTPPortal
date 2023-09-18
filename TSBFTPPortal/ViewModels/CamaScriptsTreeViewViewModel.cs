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
				//if (item.Path != null)
				//{
					
					if (item.IsDirectory)
					{
						FilterScriptChildItems(item);
						Directories.Add(item);
						item.AddDefaultChildIfEmpty();
					}
					else
					{
						Directories.Add(item);
					}
				//}
			}
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
