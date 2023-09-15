using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

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

		private bool _hasVisibleItems = true; // Initialize it as true since initially, all items are visible.
		public bool HasVisibleItems
		{
			get => _hasVisibleItems;
			set
			{
				_hasVisibleItems = value;
				OnPropertyChanged(nameof(HasVisibleItems));
			}
		}

		private bool _isSearchComplete = false;
		public bool IsSearchComplete
		{
			get => _isSearchComplete;
			set
			{
				_isSearchComplete = value;
				OnPropertyChanged(nameof(IsSearchComplete));
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
			bool anyVisibleItem = false; // Initialize it as false.

			foreach (var d in AllDirectories)
			{
				// Check if the item matches the search criteria
				bool matchesSearch = IsItemVisible(d, SearchText);

				// Set the IsVisible property
				d.IsVisible = matchesSearch;

				// Update the visibility flag
				anyVisibleItem |= matchesSearch;
			}

			// Update the HasVisibleItems property based on the flag
			HasVisibleItems = anyVisibleItem;
			IsSearchComplete = true;
		}

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
				//childItem.IsHighlighted = childMatches;
			}

			// Check if the parent item matches the search text
			bool parentMatches = item.Name?.Contains(searchText, StringComparison.OrdinalIgnoreCase) == true;

			//if (parentMatches)
			//{
			//	// If the parent item matches, highlight it
			//	//item.IsHighlighted = true;
			//}
			//else
			//{
			//	// If the parent item doesn't match, set highlight based on child matches
			//	//item.IsHighlighted = hasMatchingChild;
			//}

			// Return whether the parent or any child matches the search text
			return parentMatches || hasMatchingChild;
		}

		private void ResetAllItems(DirectoryItemViewModel item)
		{
			item.IsVisible = true;
			//item.IsHighlighted = false;

			foreach (var childItem in item.Items)
			{
				ResetAllItems(childItem);
			}
		}
	}
}
