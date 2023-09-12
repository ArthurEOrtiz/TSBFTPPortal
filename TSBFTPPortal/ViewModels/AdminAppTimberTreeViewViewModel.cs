using Serilog;
using System.Collections.ObjectModel;
using System.IO;
using TSBFTPPortal.Models;
using TSBFTPPortal.Services;

namespace TSBFTPPortal.ViewModels
{
	public class AdminAppTimberTreeViewViewModel : ViewModelBase
	{
		public County SelectedCounty { get; }
		public readonly FtpService _ftpService;
		public SearchBarViewModel SearchBarViewModel { get; }

		public AdminAppTimberTreeViewViewModel(County selectedCounty, FtpService ftpService, SearchBarViewModel searchBarViewModel)
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
				if (item.Path != null)
				{
					string fileExtension = Path.GetExtension(item.Path);
					if (fileExtension == ".sql" || item.IsDirectory)
					{
						Directories.Add(item);
						item.AddDefaultChildIfEmpty();
					}
					else
					{
						Log.Error($"Invalid File, {item.Name}, in Admin Apps Timber Exports!");
					}
				}
				else
				{
					Log.Error($"Admin Timber Exports, Could not find path for {item.Name}!");
				}
			
			}
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
