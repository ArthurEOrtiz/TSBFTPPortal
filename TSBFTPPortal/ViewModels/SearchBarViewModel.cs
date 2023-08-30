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
		//		// The problem is here, if the parent Item contains a match for the search text is does not check the child items 
		//	}

		//	// Check if any child item matches the search text
		//	bool hasMatchingChild = item.Items.Any(childItem => IsItemVisible(childItem, searchText));

		//	// Set visibility and highlight accordingly
		//	foreach (var childItem in item.Items)
		//	{
		//		childItem.IsVisible = hasMatchingChild && IsItemVisible(childItem, searchText);
		//		childItem.IsHighlighted = hasMatchingChild && IsItemVisible(childItem, searchText);
		//	}

		//	return hasMatchingChild;
		//}

		//private bool IsItemVisible(DirectoryItemViewModel item, string searchText)
		//{
		//	if (string.IsNullOrEmpty(searchText))
		//	{
		//		ResetAllItems(item);
		//		return true;
		//	}

		//	bool matchesSearchText = item.Name?.Contains(searchText, StringComparison.OrdinalIgnoreCase) == true;

		//	// Check if any child item matches the search text
		//	bool hasMatchingChild = item.Items.Any(childItem => IsItemVisible(childItem, searchText));

		//	// If the item itself matches the search text, set IsHighlighted and return true
		//	if (matchesSearchText)
		//	{
		//		item.IsHighlighted = true;
		//		return true;
		//	}

		//	// Set visibility and highlight accordingly for child items
		//	foreach (var childItem in item.Items)
		//	{
		//		childItem.IsVisible = hasMatchingChild && IsItemVisible(childItem, searchText);
		//		childItem.IsHighlighted = hasMatchingChild && IsItemVisible(childItem, searchText);
		//	}

		//	return hasMatchingChild;
		//}

		private bool IsItemVisible(DirectoryItemViewModel item, string searchText)
		{
			if (string.IsNullOrEmpty(searchText))
			{
				ResetAllItems(item);
				return true;
			}

			bool hasMatchingChild = false;

			// Check if any child item matches the search text
			foreach (var childItem in item.Items)
			{
				bool childMatches = IsItemVisible(childItem, searchText);
				hasMatchingChild |= childMatches;

				// Set visibility and highlight for the child item
				childItem.IsVisible = childMatches;
				childItem.IsHighlighted = childMatches;
			}

			// Check if the parent item matches the search text
			bool parentMatches = item.Name?.Contains(searchText, StringComparison.OrdinalIgnoreCase) == true;

			if (parentMatches)
			{
				// If the parent item matches, highlight it
				item.IsHighlighted = true;
			}
			else
			{
				// If the parent item doesn't match, set highlight based on child matches
				item.IsHighlighted = hasMatchingChild;
			}

			// Return whether the parent or any child matches the search text
			return parentMatches || hasMatchingChild;
		}




	}
}
