﻿using Serilog;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using TSBFTPPortal.Models;
using TSBFTPPortal.Services;
using TSBFTPPortal.Views;

namespace TSBFTPPortal.ViewModels
{
	public class CountySpecificTreeViewViewModel : ViewModelBase
	{
		public County SelectedCounty { get; }
		public FilterTreeViewViewModel FilterTreeViewViewModel { get; }
		public SearchBarViewModel SearchBarViewModel { get; }

		public CountySpecificTreeViewViewModel(County selectedCounty, FtpService ftpService, FilterTreeViewViewModel filterTreeViewViewModel, SearchBarViewModel searchBarViewModel)
		{
			SelectedCounty = selectedCounty;
			Directories = new ObservableCollection<DirectoryItemViewModel>();
			FilterTreeViewViewModel = filterTreeViewViewModel;
			SearchBarViewModel = searchBarViewModel;

			FilterTreeViewViewModel.PropertyChanged += (sender, e) =>
			{
				if (e.PropertyName == nameof(FilterTreeViewViewModel.IsReportsSelected) ||
							 e.PropertyName == nameof(FilterTreeViewViewModel.IsScriptsSelected) ||
							 e.PropertyName == nameof(FilterTreeViewViewModel.IsDocumentsSelected) ||
							 e.PropertyName == nameof(FilterTreeViewViewModel.IsFileSelected))
				{
					ApplyFiltering();
				}
			};

			// Attempting to have filtering apply to the directories return from search
			SearchBarViewModel.PropertyChanged += (sender, e) =>
			{
				if (e.PropertyName == nameof(SearchBarViewModel.IsSearchComplete))
				{
					ApplyFilteringForSearchedDirectories();
				}
			};

			LoadAllDirectoriesAndFoldersFromFtp(GetRootPath(), ftpService);
		}

		private string GetRootPath()
		{
			string rootPath = string.Empty;
			if (SelectedCounty != null && SelectedCounty.Name != null)
			{
				rootPath = $"/COUNTIES/{SelectedCounty.Name.ToUpper().Replace(" ", "_")}/";
			}
			else
			{
				Log.Error("County Specific, SelectCounty is null");
			}

			return rootPath;
		}

		public void ApplyFiltering()
		{
			foreach (var directory in Directories)
			{
				ApplyFilteringRecursive(directory);
			}
		}

		private void ApplyFilteringRecursive(DirectoryItemViewModel directory)
		{
			if (string.IsNullOrEmpty(SearchBarViewModel.SearchText))
			{
				UpdateDirectoryVisibility(directory);
			}
			else
			{
				if (directory.IsDirectory)
				{
					// This is a directory, apply UpdateDirectoryVisibility recursively
					foreach (var item in directory.Items)
					{
						ApplyFilteringRecursive(item);
					}
				}
				else
				{
					// This is a file, check if it matches the search text
					if (directory.Name.Contains(SearchBarViewModel.SearchText, StringComparison.OrdinalIgnoreCase))
					{
						UpdateDirectoryVisibility(directory);
					}
				}
			}
		}

		private void ApplyFilteringForSearchedDirectories()
		{
			foreach (var directory in Directories)
			{
				if (directory.IsDirectory)
				{
					foreach (var item in directory.Items)
					{
						if (item.IsVisible)
						{
							UpdateDirectoryVisibility(item);
						}
					}
				}
				else
				{
					if (directory.IsVisible)
					{
						UpdateDirectoryVisibility(directory);
					}
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

			if (FilterTreeViewViewModel.IsFileSelected && IsFileDirectory(directory))
			{
				isVisible = true;
			}

			if (directory.IsDirectory)
			{
				isVisible = true;
			}

			if (directory.Name == "No items in this directory!")
			{
				isVisible = true;
			}

			if (directory.IsDirectory && !directory.Items.Any(item => item.IsVisible))
			{
				isVisible = false;
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
			// List of document file extensions
			string[] documentExtensions = { ".doc", ".docx", ".pdf", ".txt", ".rtf", ".odt", ".xls", ".xlsx", ".csv", ".ppt", ".pptx", ".epub", ".tex", ".pages", ".numbers", ".key" };

			// Check if the file extension is not in the list of document extensions
			string fileExtension = System.IO.Path.GetExtension(directory.Name);
			return documentExtensions.Contains(fileExtension, StringComparer.OrdinalIgnoreCase);
		}

		private static bool IsFileDirectory(DirectoryItemViewModel directory)
		{
			return !IsReportsDirectory(directory) &&
						 !IsScriptsDirectory(directory) &&
						 !IsDocumentsDirectory(directory);
		}
	}
}
