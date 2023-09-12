﻿using System;
using System.Collections.ObjectModel;
using TSBFTPPortal.Models;
using TSBFTPPortal.Services;
using TSBFTPPortal.Views;
using System.Linq;
using Serilog;

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
			string rootPath = string.Empty;
			if (SelectedCounty != null && SelectedCounty.Name != null)
			{
				rootPath = $"/COUNTIES/{SelectedCounty.Name.ToUpper()}/";
			}
			else
			{
				Log.Error("County Specific, SelectCounty is null");
			}
			

			var items = _ftpService.LoadDirectoriesAndFilesFromFTP(rootPath);

			foreach (var item in items)
			{
				Directories.Add(item);
				item.AddDefaultChildIfEmpty();
			}
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
					if (directory.IsHighlighted)
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
				if (subDirectory.IsHighlighted)
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

			if (directory.IsDirectory)
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

		private static bool IsReportsDirectory(DirectoryItemViewModel directory)
		{
			return directory.Name?.EndsWith(".rpt", StringComparison.OrdinalIgnoreCase) == true;
		}


		private static bool IsScriptsDirectory(DirectoryItemViewModel directory)
		{
			return directory.Name?.EndsWith(".sql", StringComparison.OrdinalIgnoreCase) == true;
		}

		private static bool IsDocumentsDirectory(DirectoryItemViewModel directory)
		{
			return !directory.Name?.EndsWith(".sql", StringComparison.OrdinalIgnoreCase) == true &&
						 !directory.Name?.EndsWith(".rpt", StringComparison.OrdinalIgnoreCase) == true;
		}

	}
}
