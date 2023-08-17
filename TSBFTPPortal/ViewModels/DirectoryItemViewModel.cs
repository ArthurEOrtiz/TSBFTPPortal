using System.Collections.ObjectModel;
using System.Windows.Input;
using TSBFTPPortal.Commands;

namespace TSBFTPPortal.ViewModels
{
	public class DirectoryItemViewModel : ViewModelBase
	{
		public string? Name { get; set; }
		public string? Path { get; set; }
		public bool IsDirectory { get; set; }
		public bool IsFile => !IsDirectory;
		public ObservableCollection<DirectoryItemViewModel> Items { get; } = new ObservableCollection<DirectoryItemViewModel>();

		public ICommand DownloadCommand { get; private set; }

		public DirectoryItemViewModel()
		{
			DownloadCommand = new RelayCommand(Download);
		}

		private void Download(object obj)
		{
			
		}
	}
}
