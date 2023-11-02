using Serilog;
using System.Collections.ObjectModel;
using TSBFTPPortal.Models;
using TSBFTPPortal.Services;

namespace TSBFTPPortal.ViewModels
{
	public class CamaScriptsTreeViewViewModel : ViewModelBase
	{
		public County SelectedCounty { get; }
		public SearchBarViewModel SearchBarViewModel { get; }

		public CamaScriptsTreeViewViewModel(County selectedCounty, FtpService ftpService, SearchBarViewModel searchBarViewModel)
		{
			SelectedCounty = selectedCounty;
			Directories = new ObservableCollection<DirectoryItemViewModel>();
			SearchBarViewModel = searchBarViewModel;
			LoadScriptDirectoriesAndFoldersFromFTPAsync(GetRootPath(), ftpService);
		}

		private string GetRootPath()
		{
			string rootPath = string.Empty;
			if (SelectedCounty != null && SelectedCounty.CAMASystem != null)
			{
				rootPath = $"/FTP_DASHBOARD/CAMA/{SelectedCounty.CAMASystem.ToUpper()}/SCRIPTS/";
			}
			else
			{
				Log.Error("Cama Scrips, Select County is null");
			}

			return rootPath;
		}
	}
}
