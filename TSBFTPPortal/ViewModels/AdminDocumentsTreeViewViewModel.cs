using System.Collections.ObjectModel;
using System.IO;
using TSBFTPPortal.Models;
using TSBFTPPortal.Services;

namespace TSBFTPPortal.ViewModels
{
	public class AdminDocumentsTreeViewViewModel : ViewModelBase
	{
		public County SelectedCounty { get; }
		public readonly FtpService _ftpService;

		public AdminDocumentsTreeViewViewModel(County selectedCounty, FtpService ftpService)
		{
			SelectedCounty = selectedCounty;
			_ftpService = ftpService;
			Directories = new ObservableCollection<DirectoryItemViewModel>();
			LoadDirectoriesAndFoldersFromFTP();
		}

		private void LoadDirectoriesAndFoldersFromFTP()
		{
			string rootPath = $"/FTP_DASHBOARD/ADMIN/{SelectedCounty.AdminSystem.ToUpper()}/DOCUMENTS/";

			var items = _ftpService.LoadDirectoriesAndFilesFromFTP(rootPath);

			foreach (DirectoryItemViewModel item in items)
			{
				string fileExtension = Path.GetExtension(item.Path);
				if (fileExtension != ".rpt" && fileExtension != ".sql")
				{
					Directories.Add(item);
				}
			}
		}
	}
}
