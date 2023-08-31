using Serilog;
using System.Collections.ObjectModel;
using System.IO;
using TSBFTPPortal.Models;
using TSBFTPPortal.Services;

namespace TSBFTPPortal.ViewModels
{
	public class CamaScriptsTreeViewViewModel : ViewModelBase
	{
		public County SelectedCounty { get; }
		public readonly FtpService _ftpService;

		public CamaScriptsTreeViewViewModel(County selectedCounty, FtpService ftpService)
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
				if (item.Path != null)
				{
					string fileExtension = Path.GetExtension(item.Path);
					if (fileExtension == ".sql" || item.IsDirectory)
					{
						Directories.Add(item);
					}
					else
					{
						Log.Error($"Invalid file, {item.Name}, in Cama Scripts!");
					}
				}
				else
				{
					Log.Error($"Cama Scripts, Could not find path for {item.Name}!");
				}
			}
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
