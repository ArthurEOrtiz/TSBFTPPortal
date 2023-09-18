using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace TSBFTPPortal.ViewModels
{
	public class ViewModelBase : INotifyPropertyChanged
	{
		private ObservableCollection<DirectoryItemViewModel> _directories;
		public ObservableCollection<DirectoryItemViewModel> Directories
		{
			get => _directories; 
			set
			{
				_directories = value;
				OnPropertyChanged(nameof(Directories));
			}
		}
		public ViewModelBase()
		{
			_directories = new ObservableCollection<DirectoryItemViewModel>();
		}

		public event PropertyChangedEventHandler? PropertyChanged;

		protected void OnPropertyChanged(string? propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public bool IsScriptFile(string? filePath)
		{
			string fileExtension = Path.GetExtension(filePath);
			string[] scriptExtensions = { ".sql" };
			return scriptExtensions.Contains(fileExtension, StringComparer.OrdinalIgnoreCase);
		}

		public void FilterScriptChildItems(DirectoryItemViewModel item)
		{
			var copyOfItems = new List<DirectoryItemViewModel>(item.Items);

			foreach (var childItem in copyOfItems)
			{
				if (!IsScriptFile(childItem.Name) && childItem.IsFile)
				{
					item.Items.Remove(childItem);
				}
				else if (childItem.IsDirectory)
				{
					FilterScriptChildItems(childItem);
				}
			}
		}


	}
}
