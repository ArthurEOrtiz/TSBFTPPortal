using System;
using System.Collections.ObjectModel;
using TSBFTPPortal.Models;
using TSBFTPPortal.Services;
using TSBFTPPortal.Views;

namespace TSBFTPPortal.ViewModels
{
	public class CountySpecificTreeViewViewModel : ViewModelBase
	{
		public County SelectedCounty { get; }
		public readonly IFtpService _ftpService;
		public FilterTreeViewViewModel FilterTreeViewViewModel { get; }

		public CountySpecificTreeViewViewModel(County selectedCounty, IFtpService ftpService, FilterTreeViewViewModel filterTreeViewViewModel)
		{
			SelectedCounty = selectedCounty;
			_ftpService = ftpService;
			Directories = new ObservableCollection<DirectoryItemViewModel>();
			FilterTreeViewViewModel = filterTreeViewViewModel;

			FilterTreeViewViewModel.PropertyChanged += (sender, e) =>
			{
				if (e.PropertyName == nameof(FilterTreeViewViewModel.IsReportsSelected) ||
							 e.PropertyName == nameof(FilterTreeViewViewModel.IsScriptsSelected) ||
							 e.PropertyName == nameof(FilterTreeViewViewModel.IsDocumentsSelected))
				{
					ApplyFiltering();
				}
			};

			LoadDirectoriesAndFoldersFromFTP();
		}

		private void LoadDirectoriesAndFoldersFromFTP()
		{
			string rootPath = $"/COUNTIES/{SelectedCounty.Name.ToUpper()}/";

			var items = _ftpService.LoadDirectoriesAndFilesFromFTP(rootPath);

			foreach (var item in items)
			{
				Directories.Add(item);
			}

			ApplyFiltering();
		}

		public void ApplyFiltering()
		{
			foreach (var directory in Directories)
			{
				UpdateDirectoryVisibility(directory);
			}
		}

		private void UpdateDirectoryVisibility(DirectoryItemViewModel directory)
		{
			bool isVisible = IsVisibleRecursive(directory);
			directory.IsVisible = isVisible;

			// Update child items recursively
			foreach (var subDirectory in directory.Items)
			{
				UpdateDirectoryVisibility(subDirectory);
			}
		}

		private bool IsVisibleRecursive(DirectoryItemViewModel directory)
		{
			bool isVisible = false;

			if (FilterTreeViewViewModel.IsReportsSelected && IsReportsDirectory(directory))
			{
				isVisible = true;
			}

			if (FilterTreeViewViewModel.IsScriptsSelected && IsScriptsDirectory(directory))
			{
				isVisible = true;
			}

			if (FilterTreeViewViewModel.IsDocumentsSelected && IsDocumentsDirectory(directory))
			{
				isVisible = true;
			}

			foreach (var subDirectory in directory.Items)
			{
				bool isSubVisible = IsVisibleRecursive(subDirectory);
				if (isSubVisible)
				{
					isVisible = true;
				
				}
				else
				{
					subDirectory.IsVisible = false;
				}
			}

			return isVisible;
		}

		private bool IsReportsDirectory(DirectoryItemViewModel directory)
		{
			return !directory.IsDirectory && directory.Name != null && directory.Name.EndsWith(".rpt", StringComparison.OrdinalIgnoreCase);
		}

		private bool IsScriptsDirectory(DirectoryItemViewModel directory)
		{
			return !directory.IsDirectory && directory.Name != null && directory.Name.EndsWith(".sql", StringComparison.OrdinalIgnoreCase);
		}

		private bool IsDocumentsDirectory(DirectoryItemViewModel directory)
		{
			return !directory.IsDirectory && directory.Name != null &&
					!directory.Name.EndsWith(".sql", StringComparison.OrdinalIgnoreCase) &&
					!directory.Name.EndsWith(".rpt", StringComparison.OrdinalIgnoreCase);
		}

		public void UpdateDirectoryVisibility()
		{
			foreach (var directory in Directories)
			{
				bool isVisible = IsVisibleRecursive(directory);
				directory.IsVisible = isVisible;
			}
		}

	}
}
