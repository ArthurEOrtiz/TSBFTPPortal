﻿using System;
using System.Collections.ObjectModel;
using TSBFTPPortal.Models;
using TSBFTPPortal.Services;
using TSBFTPPortal.Views;
using System.Linq;

namespace TSBFTPPortal.ViewModels
{
	public class CountySpecificTreeViewViewModel : ViewModelBase
	{
		public County SelectedCounty { get; }
		public readonly FtpService _ftpService;
		public FilterTreeViewViewModel FilterTreeViewViewModel { get; }
		public SearchBarViewModel SearchBarViewModel { get; }

		public CountySpecificTreeViewViewModel(County selectedCounty, FtpService ftpService, FilterTreeViewViewModel filterTreeViewViewModel, SearchBarViewModel searchBarViewModel)
		{
			SelectedCounty = selectedCounty;
			_ftpService = ftpService;
			Directories = new ObservableCollection<DirectoryItemViewModel>();
			FilterTreeViewViewModel = filterTreeViewViewModel;
			SearchBarViewModel = searchBarViewModel;

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
				if (string.IsNullOrEmpty(SearchBarViewModel.SearchText))
				{
					UpdateDirectoryVisibility(directory);
				}
				else
				{
					if (directory.IsVisible && directory.IsHighlighted)
					{
						UpdateDirectoryVisibilitySearchedDirectories(directory);
					}
				}
			}
		}

		private void UpdateDirectoryVisibilitySearchedDirectories(DirectoryItemViewModel directory)
		{
			bool isVisible = IsVisibleRecursive(directory);
			directory.IsVisible = isVisible;

			// Update child items recursively
			foreach (var subDirectory in directory.Items)
			{
				if (subDirectory.IsHighlighted && subDirectory.IsVisible)
				{
					UpdateDirectoryVisibilitySearchedDirectories(subDirectory);
				}
				else
				{
					subDirectory.IsVisible = false;
				}
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
