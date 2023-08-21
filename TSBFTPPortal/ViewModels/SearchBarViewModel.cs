using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

		private bool IsItemVisible(DirectoryItemViewModel item, string searchText)
		{
			if (string.IsNullOrEmpty(searchText))
			{
				// When the search text is empty, make all items visible
				foreach (var childItem in item.Items)
				{
					childItem.IsVisible = true;
					childItem.IsHighlighted = false;
				}
				item.IsHighlighted = false;
				return true;
			}

			if (item.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase))
			{
				item.IsHighlighted = true;
				return true;
			}

			// Check if any child item matches the search text
			bool hasMatchingChild = false;
			foreach (var childItem in item.Items)
			{
				if (IsItemVisible(childItem, searchText))
				{
					hasMatchingChild = true;
					childItem.IsVisible = true; // Make the matching child item visible
					childItem.IsHighlighted = true;
				}
				else
				{
					childItem.IsVisible = false;
					childItem.IsVisible = false; // Make non-matching child items not visible
				}
			}

			if (hasMatchingChild)
			{
				return true; // At least one child item matches the search text
			}

			return false; // No matches found in this item or its children
		}

	}
}
