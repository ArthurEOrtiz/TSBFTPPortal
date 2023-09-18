using Serilog;
using System.Collections.ObjectModel;
using TSBFTPPortal.Models;
using TSBFTPPortal.Services;

namespace TSBFTPPortal.ViewModels
{
	public class GISTreeViewViewModel : ViewModelBase
	{
		public County SelectedCounty { get; }
		public SearchBarViewModel SearchBarViewModel { get; }

		public GISTreeViewViewModel(County selectedCounty, FtpService ftpService, SearchBarViewModel searchBarViewModel)
		{
			SelectedCounty = selectedCounty;
			Directories = new ObservableCollection<DirectoryItemViewModel>();
			SearchBarViewModel = searchBarViewModel;
			LoadAllDirectoriesAndFoldersFromFtp(GetRootPath(), ftpService);
		}

		private string GetRootPath()
		{
			string rootPath = string.Empty;

			if (SelectedCounty != null && SelectedCounty.Name != null)
			{
				rootPath = $"/GIS/{SelectedCounty.Name.ToUpper()}/";
			}
			else
			{
				Log.Error("GIS, Select County is null");
			}

			return rootPath;
		}
	}
}
