using System.Collections.ObjectModel;
using TSBFTPPortal.Models;
using TSBFTPPortal.Services;

namespace TSBFTPPortal.ViewModels
{
	public class PABTreeViewViewModel : ViewModelBase
	{
		//private ObservableCollection<DirectoryItemViewModel> _directories;
		//public ObservableCollection<DirectoryItemViewModel> Directories
		//{
		//	get { return _directories; }
		//	set
		//	{
		//		_directories = value;
		//		OnPropertyChanged(nameof(Directories));
		//	}
		//}

		public County SelectedCounty { get; }
		public readonly IFtpService _ftpService;
	
		public PABTreeViewViewModel(County selectedCounty, IFtpService ftpService)
		{
			SelectedCounty = selectedCounty;
			_ftpService = ftpService;
			Directories = new ObservableCollection<DirectoryItemViewModel>();
			LoadDirectoriesAndFoldersFromFTP();
			
		}


		private void LoadDirectoriesAndFoldersFromFTP()
		{
			string rootPath = $"/PAB/{SelectedCounty.Name.ToUpper()}/";

			var items = _ftpService.LoadDirectoriesAndFilesFromFTP(rootPath);

			foreach (var item in items)
			{
				Directories.Add(item);
			}
		}
	}
}
