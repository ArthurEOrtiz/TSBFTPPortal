using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;


namespace TSBFTPPortal.ViewModels
{
	public class SearchBarViewModel : ViewModelBase
	{
		private string _searchText;
		public string SearchText
		{
			get => _searchText;
			set
			{
				_searchText = value;
				OnPropertyChanged(nameof(SearchText));
				PerformSearch();
			}
		}

		public ObservableCollection<DirectoryItemViewModel> AllDirectories { get; set; }

		public SearchBarViewModel()
		{
			AllDirectories = new ObservableCollection<DirectoryItemViewModel>();
		}

		public void SetAllDirectories(IEnumerable<DirectoryItemViewModel> directories)
		{
			AllDirectories.Clear();
			foreach (var d in directories)
			{
				AllDirectories.Add(d);
			}
		}
		//private void PerformSearch()
		//{
		//	foreach (var d in AllDirectories)
		//	{
		//		d.IsVisible = string.IsNullOrEmpty(SearchText) || d.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase);
		//	}
		//}
		//private void PerformSearch()
		//{
		//	foreach (var d in AllDirectories)
		//	{
		//		d.IsVisible = IsItemVisible(d, SearchText);
		//	}
		//}
		private void PerformSearch()
		{
			foreach (var d in AllDirectories)
			{
				if (string.IsNullOrEmpty(SearchText))
				{
					// When the search text is empty, make all items visible
					d.IsVisible = true;
				}
				else
				{
					d.IsVisible = IsItemVisible(d, SearchText);
				}
			}
		}


		private bool IsItemVisible(DirectoryItemViewModel item, string searchText)
		{
			// Check if the current item's name matches the search text
			if (!string.IsNullOrEmpty(searchText) && item.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}

			// Recursively check child items (subdirectories and files)
			foreach (var childItem in item.Items)
			{
				if (IsItemVisible(childItem, searchText))
				{
					return true; // At least one child item matches the search text
				}
			}

			return false; // No matches found in this item or its children
		}


	}
}
