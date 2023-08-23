using System.Collections.ObjectModel;
using TSBFTPPortal.Models;
using TSBFTPPortal.Services;

namespace TSBFTPPortal.ViewModels
{
	public class PTRTreeViewViewModel : ViewModelBase
	{
		public County SelectedCounty { get; }
		public readonly FtpService _ftpService;

		public PTRTreeViewViewModel(County selectedCounty, FtpService ftpService)
		{
			SelectedCounty = selectedCounty;
			_ftpService = ftpService;
			Directories = new ObservableCollection<DirectoryItemViewModel>();
			LoadDirectoriesAndFoldersFromFTP();

		}

		private void LoadDirectoriesAndFoldersFromFTP()
		{
			string rootPath = $"/PTR/{SelectedCounty.Name.ToUpper()}/";

			var items = _ftpService.LoadDirectoriesAndFilesFromFTP(rootPath);

			foreach (var item in items)
			{
				Directories.Add(item);
			}
		}
	}
}
