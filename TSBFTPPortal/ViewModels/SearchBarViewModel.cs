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
		private void PerformSearch()
		{
			foreach (var d in AllDirectories)
			{
				d.IsVisible = string.IsNullOrEmpty(SearchText) || d.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase);
			}
		}
	}
}
