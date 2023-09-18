using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using TSBFTPPortal.Services;

namespace TSBFTPPortal.ViewModels
{
	public class ViewModelBase : INotifyPropertyChanged
	{
		private ObservableCollection<DirectoryItemViewModel> _directories;
		public event PropertyChangedEventHandler? PropertyChanged;
		protected void OnPropertyChanged(string? propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
		public ObservableCollection<DirectoryItemViewModel> Directories
		{
			get => _directories; 
			set
			{
				_directories = value;
				OnPropertyChanged(nameof(Directories));
			}
		}

		public ViewModelBase()
		{
			_directories = new ObservableCollection<DirectoryItemViewModel>();
		}

		public void LoadScriptDirectoriesAndFoldersFromFTP(string rootPath, FtpService ftpService)
		{

			ObservableCollection<DirectoryItemViewModel> items = ftpService.LoadDirectoriesAndFilesFromFTP(rootPath);

			foreach (DirectoryItemViewModel item in items)
			{
				if (item.IsDirectory)
				{
					FilterChildItems(item, IsScriptFile);
					Directories.Add(item);
					item.AddDefaultChildIfEmpty();
				}
				else
				{
					Directories.Add(item);
				}
			}
		}

		public void LoadReportDirectoriesAndFoldersFromFTP(string rootPath, FtpService ftpService)
		{
			ObservableCollection<DirectoryItemViewModel> items = ftpService.LoadDirectoriesAndFilesFromFTP(rootPath);

			foreach (DirectoryItemViewModel item in items)
			{
				if (item.IsDirectory)
				{
					FilterChildItems(item, IsReportFile);
					Directories.Add(item);
					item.AddDefaultChildIfEmpty();
				}
				else
				{
					Directories.Add(item);
				}
			}
		}

		private bool IsScriptFile(string filePath)
		{
			string fileExtension = Path.GetExtension(filePath);
			string[] scriptExtensions = { ".sql" };
			return scriptExtensions.Contains(fileExtension, StringComparer.OrdinalIgnoreCase);
		}

		private bool IsReportFile (string filePath)
		{
			string fileExtension = Path.GetExtension(filePath);
			string[] reportExtensions = { ".rpt" };
			return reportExtensions.Contains(fileExtension, StringComparer.OrdinalIgnoreCase);
		}

		private void FilterChildItems(DirectoryItemViewModel item, Func<string?, bool> fileCondition)
		{
			var copyOfItems = new List<DirectoryItemViewModel>(item.Items);

			foreach (var childItem in copyOfItems)
			{
				if (!fileCondition(childItem.Name) && childItem.IsFile)
				{
					item.Items.Remove(childItem);
				}
				else if (childItem.IsDirectory)
				{
					FilterChildItems(childItem, fileCondition);
				}
			}
		}

	

		


	}
}
