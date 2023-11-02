using Serilog;
using System.Collections.ObjectModel;
using TSBFTPPortal.Models;
using TSBFTPPortal.Services;

namespace TSBFTPPortal.ViewModels
{
	public class PABTreeViewViewModel : ViewModelBase
	{
		public County SelectedCounty { get; }
		public SearchBarViewModel SearchBarViewModel { get; }

		public PABTreeViewViewModel(County selectedCounty, FtpService ftpService, SearchBarViewModel searchBarViewModel )
		{
			SelectedCounty = selectedCounty;
			Directories = new ObservableCollection<DirectoryItemViewModel>();
			SearchBarViewModel = searchBarViewModel;
			LoadAllDirectoriesAndFoldersFromFtpAsync(GetRootPath(), ftpService);	
		}

		private string GetRootPath()
		{
			string rootPath = string.Empty;

			if (SelectedCounty != null && SelectedCounty.Name != null)
			{
				rootPath = $"/PAB/{SelectedCounty.Name.ToUpper().Replace(" ", "_")}/";
			}
			else
			{
				Log.Error("PAB, Select County is null");
			}

			return rootPath;
		}
	}
}
