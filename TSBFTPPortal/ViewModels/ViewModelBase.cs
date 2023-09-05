using System.Collections.ObjectModel;
using System.ComponentModel;

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
		public event PropertyChangedEventHandler? PropertyChanged;

		protected void OnPropertyChanged(string? propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public ViewModelBase()
		{
			_directories = new ObservableCollection<DirectoryItemViewModel>();
		}
	}
}
