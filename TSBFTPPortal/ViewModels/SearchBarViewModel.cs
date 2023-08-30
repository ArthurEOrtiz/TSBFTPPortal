using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

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
			_searchText = string.Empty; // Initializing search text with an empty string. 
		}

		public void SetAllDirectories(IEnumerable<DirectoryItemViewModel> directories)
		{
			foreach (var d in directories)
			{
				AllDirectories.Add(d);
			}
		}

		private void PerformSearch()
		{
			foreach (var d in AllDirectories)
			{
				// Check if the item matches the search criteria
				bool matchesSearch = IsItemVisible(d, SearchText);

				// Set the IsVisible property
				d.IsVisible = matchesSearch;
			}
		}

		private void ResetAllItems(DirectoryItemViewModel item)
		{
			item.IsVisible = true;
			item.IsHighlighted = false;

			foreach (var childItem in item.Items)
			{
				ResetAllItems(childItem);
			}
		}

		//private bool IsItemVisible(DirectoryItemViewModel item, string searchText)
		//{
		//	if (string.IsNullOrEmpty(searchText))
		//	{
		//		ResetAllItems(item);
		//		return true;
		//	}

		//	if (item.Name?.Contains(searchText, StringComparison.OrdinalIgnoreCase) == true)
		//	{
		//		item.IsHighlighted = true;
		//		return true;
		//	}

		//	// Check if any child item matches the search text
		//	bool hasMatchingChild = false;
		//	foreach (var childItem in item.Items)
		//	{
		//		if (IsItemVisible(childItem, searchText))
		//		{
		//			hasMatchingChild = true;
		//			childItem.IsVisible = true; // Make the matching child item visible
		//			childItem.IsHighlighted = true;
		//		}
		//		else
		//		{
		//			childItem.IsVisible = false; // Make non-matching child items not visible
		//			childItem.IsHighlighted = false;
		//		}
		//	}

		//	if (hasMatchingChild)
		//	{
		//		return true; // At least one child item matches the search text
		//	}


		//	return false; // No matches found in this item or its children

		//}


		private bool IsItemVisible(DirectoryItemViewModel item, string searchText)
		{
			if (string.IsNullOrEmpty(searchText))
			{
				ResetAllItems(item);
				return true;
			}

			if (item.Name?.Contains(searchText, StringComparison.OrdinalIgnoreCase) == true)
			{
				item.IsHighlighted = true;
				return true;
			}

			// Check if any child item matches the search text
			bool hasMatchingChild = item.Items.Any(childItem => IsItemVisible(childItem, searchText));

			// Set visibility and highlight accordingly
			foreach (var childItem in item.Items)
			{
				childItem.IsVisible = hasMatchingChild && IsItemVisible(childItem, searchText);
				childItem.IsHighlighted = hasMatchingChild && IsItemVisible(childItem, searchText);
			}

			return hasMatchingChild;
		}


	}
}
