using Serilog;
using System.Collections.ObjectModel;
using TSBFTPPortal.Models;
using TSBFTPPortal.Services;

namespace TSBFTPPortal.ViewModels
{
	public class GISTreeViewViewModel : ViewModelBase
	{
		public County SelectedCounty { get; }
		public readonly FtpService _ftpService;

		public GISTreeViewViewModel(County selectedCounty, FtpService ftpService)
		{
			SelectedCounty = selectedCounty;
			_ftpService = ftpService;
			Directories = new ObservableCollection<DirectoryItemViewModel>();
			LoadDirectoriesAndFoldersFromFTP();

		}

		private void LoadDirectoriesAndFoldersFromFTP()
		{
			string rootPath = GetRootPath();

			ObservableCollection<DirectoryItemViewModel> items = _ftpService.LoadDirectoriesAndFilesFromFTP(rootPath);

			foreach (DirectoryItemViewModel item in items)
			{
				Directories.Add(item);
			}
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
