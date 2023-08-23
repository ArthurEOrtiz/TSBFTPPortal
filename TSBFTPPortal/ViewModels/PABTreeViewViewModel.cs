using System.Collections.ObjectModel;
using TSBFTPPortal.Models;
using TSBFTPPortal.Services;

namespace TSBFTPPortal.ViewModels
{
	public class PABTreeViewViewModel : ViewModelBase
	{
		public County SelectedCounty { get; }
		public readonly FtpService _ftpService;
	
		public PABTreeViewViewModel(County selectedCounty, FtpService ftpService)
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
