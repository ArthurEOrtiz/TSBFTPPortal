using Serilog;
using System.Collections.ObjectModel;
using TSBFTPPortal.Models;
using TSBFTPPortal.Services;

namespace TSBFTPPortal.ViewModels
{
	public class PABTreeViewViewModel : ViewModelBase
	{
		public County SelectedCounty { get; }
		public readonly FtpService _ftpService;
		public SearchBarViewModel SearchBarViewModel { get; }

		public PABTreeViewViewModel(County selectedCounty, FtpService ftpService, SearchBarViewModel searchBarViewModel )
		{
			SelectedCounty = selectedCounty;
			_ftpService = ftpService;
			Directories = new ObservableCollection<DirectoryItemViewModel>();
			SearchBarViewModel = searchBarViewModel;
			LoadDirectoriesAndFoldersFromFTP();

		}

		private void LoadDirectoriesAndFoldersFromFTP()
		{
			string rootPath = GetRootPath();

			ObservableCollection<DirectoryItemViewModel> items = _ftpService.LoadDirectoriesAndFilesFromFTP(rootPath);

			foreach (DirectoryItemViewModel item in items)
			{
				Directories.Add(item);
				item.AddDefaultChildIfEmpty();
			}
		}

		private string GetRootPath()
		{
			string rootPath = string.Empty;

			if (SelectedCounty != null && SelectedCounty.Name != null)
			{
				rootPath = $"/PAB/{SelectedCounty.Name.ToUpper()}/";
			}
			else
			{
				Log.Error("PAB, Select County is null");
			}

			return rootPath;
		}
	}
}
