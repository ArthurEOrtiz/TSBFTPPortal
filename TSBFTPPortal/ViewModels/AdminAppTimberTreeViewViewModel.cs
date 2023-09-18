using Serilog;
using System.Collections.ObjectModel;
using TSBFTPPortal.Models;
using TSBFTPPortal.Services;

namespace TSBFTPPortal.ViewModels
{
	public class AdminAppTimberTreeViewViewModel : ViewModelBase
	{
		public County SelectedCounty { get; }
		public SearchBarViewModel SearchBarViewModel { get; }

		public AdminAppTimberTreeViewViewModel(County selectedCounty, FtpService ftpService, SearchBarViewModel searchBarViewModel)
		{
			SelectedCounty = selectedCounty;
			Directories = new ObservableCollection<DirectoryItemViewModel>();
			SearchBarViewModel = searchBarViewModel;
			LoadScriptDirectoriesAndFoldersFromFTP(GetRootPath(), ftpService);
		}


		private string GetRootPath()
		{
			string rootPath = string.Empty;
			if (SelectedCounty != null && SelectedCounty.AdminSystem != null)
			{
				rootPath = $"/FTP_DASHBOARD/ADMIN/{SelectedCounty.AdminSystem.ToUpper()}/APPS/TIMBER_EXPORTS/";
			}
			else
			{
				Log.Error("Admin Apps Timber Export, Select County is null");
			}

			return rootPath;
		}
	}
}
