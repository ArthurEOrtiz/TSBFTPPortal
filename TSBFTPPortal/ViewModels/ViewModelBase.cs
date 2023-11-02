using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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

		public async Task LoadScriptDirectoriesAndFoldersFromFTPAsync(string rootPath, FtpService ftpService)
		{
			var items = await ftpService.LoadDirectoriesAndFilesFromFTPAsync(rootPath);
			IOrderedEnumerable<DirectoryItemViewModel> sortedItems = (IOrderedEnumerable<DirectoryItemViewModel>)SortByDirectoryAndFileType(items);

			foreach (DirectoryItemViewModel item in sortedItems)
			{
				if (item.IsDirectory)
				{
					FilterChildItems(item, IsScriptFile);
					Directories.Add(item);
					item.AddDefaultChildIfEmpty();
				}
				else if (IsScriptFile(item.Name))
				{
					Directories.Add(item);
				}
			}
		}

		public async Task LoadReportDirectoriesAndFoldersFromFTPAsync(string rootPath, FtpService ftpService)
		{
			var items = await ftpService.LoadDirectoriesAndFilesFromFTPAsync(rootPath);
			IOrderedEnumerable<DirectoryItemViewModel> sortedItems = (IOrderedEnumerable<DirectoryItemViewModel>)SortByDirectoryAndFileType(items);

			foreach (DirectoryItemViewModel item in sortedItems)
			{
				if (item.IsDirectory)
				{
					FilterChildItems(item, IsReportFile);
					Directories.Add(item);
					item.AddDefaultChildIfEmpty();
				}
				else if (IsReportFile(item.Name))
				{
					Directories.Add(item);
				}
			}
		}

		public async Task LoadDocumentAndFilesDirectoriesAndFoldersFromFtpAsync(string rootPath, FtpService ftpService)
		{
			var items = await ftpService.LoadDirectoriesAndFilesFromFTPAsync(rootPath);
			IOrderedEnumerable<DirectoryItemViewModel> sortedItems = (IOrderedEnumerable<DirectoryItemViewModel>)SortByDirectoryAndFileType(items);

			foreach (DirectoryItemViewModel item in sortedItems)
			{
				if (item.IsDirectory)
				{
					FilterChildItems(item, IsDocumentOrFile);
					Directories.Add(item);
					item.AddDefaultChildIfEmpty();
				}
				else if (IsDocumentOrFile(item.Name))
				{
					Directories.Add(item);
				}
			}
		}

		public async Task LoadAllDirectoriesAndFoldersFromFtpAsync(string rootPath, FtpService ftpService)
		{
			var items = await ftpService.LoadDirectoriesAndFilesFromFTPAsync(rootPath);
			IOrderedEnumerable<DirectoryItemViewModel> sortedItems = (IOrderedEnumerable<DirectoryItemViewModel>)SortByDirectoryAndFileType(items);

			foreach (DirectoryItemViewModel item in sortedItems)
			{
				Directories.Add(item);
				item.AddDefaultChildIfEmpty();
			}
		}

		private static IEnumerable<DirectoryItemViewModel> SortByDirectoryAndFileType(IEnumerable<DirectoryItemViewModel> items)
		{
			var sortedItems = items
					.OrderByDescending(item => item.IsDirectory)
					.ThenBy(item => item.IsFile ? Path.GetExtension(item.Name) : "")
					.ThenBy(item => item.Name);

			// Recursively sort child items
			foreach (var item in sortedItems)
			{
				item.Items = new ObservableCollection<DirectoryItemViewModel>(
						SortByDirectoryAndFileType(item.Items)
				);
			}

			return sortedItems;
		}

		private bool IsScriptFile(string filePath)
		{
			string fileExtension = Path.GetExtension(filePath);
			string[] scriptExtensions = { ".sql" };
			return scriptExtensions.Contains(fileExtension, StringComparer.OrdinalIgnoreCase);
		}

		private bool IsReportFile(string filePath)
		{
			string fileExtension = Path.GetExtension(filePath);
			string[] reportExtensions = { ".rpt" };
			return reportExtensions.Contains(fileExtension, StringComparer.OrdinalIgnoreCase);
		}

		private bool IsDocumentOrFile(string filePath)
		{
			return !IsScriptFile(filePath) && !IsReportFile(filePath);
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
