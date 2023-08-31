﻿using Serilog;
using System.Collections.ObjectModel;
using TSBFTPPortal.Models;
using TSBFTPPortal.Services;

namespace TSBFTPPortal.ViewModels
{
	public class PABTreeViewViewModel : ViewModelBase
	{
		public County SelectedCounty { get; }
		public readonly FtpService _ftpService;

		public PABTreeViewViewModel(County selectedCounty, FtpService ftpService)
		{
			SelectedCounty = selectedCounty;
			_ftpService = ftpService;
			Directories = new ObservableCollection<DirectoryItemViewModel>();
			LoadDirectoriesAndFoldersFromFTP();

		}

		private void LoadDirectoriesAndFoldersFromFTP()
		{
			string rootPath = GetRootPath();

			ObservableCollection<DirectoryItemViewModel> items = _ftpService.LoadDirectoriesAndFilesFromFTP(rootPath);

			foreach (DirectoryItemViewModel item in items)
			{
				Directories.Add(item);
			}
		}

		private string GetRootPath()
		{
			string rootPath = string.Empty;

			if (SelectedCounty != null && SelectedCounty.Name != null)
			{
				rootPath = $"/PAB/{SelectedCounty.Name.ToUpper()}/";
			}
			else
			{
				Log.Error("PAB, Select County is null");
			}

			return rootPath;
		}
	}
}
