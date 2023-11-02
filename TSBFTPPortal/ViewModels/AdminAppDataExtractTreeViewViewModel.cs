using Serilog;
using System.Collections.ObjectModel;
using TSBFTPPortal.Models;
using TSBFTPPortal.Services;

namespace TSBFTPPortal.ViewModels
{
	public class AdminAppDataExtractTreeViewViewModel : ViewModelBase
	{
		public County SelectedCounty { get; }
		public SearchBarViewModel SearchBarViewModel { get; }

		public AdminAppDataExtractTreeViewViewModel(County selectedCounty, FtpService ftpService, SearchBarViewModel searchBarViewModel)
		{
			SelectedCounty = selectedCounty;
			Directories = new ObservableCollection<DirectoryItemViewModel>();
			SearchBarViewModel = searchBarViewModel;
			LoadScriptDirectoriesAndFoldersFromFTPAsync(GetRootPath(), ftpService);
		}

		private string GetRootPath()
		{
			string rootPath = string.Empty;
			if (SelectedCounty != null && SelectedCounty.AdminSystem != null)
			{
				rootPath = $"/FTP_DASHBOARD/ADMIN/{SelectedCounty.AdminSystem.ToUpper()}/APPS/DATA_EXTRACT/";
			}
			else
			{
				Log.Error("Admin Apps Data Extract, Select County is null");
			}

			return rootPath;
		}
	}
}
