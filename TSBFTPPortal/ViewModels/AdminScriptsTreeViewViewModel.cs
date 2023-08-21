using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSBFTPPortal.Models;
using TSBFTPPortal.Services;

namespace TSBFTPPortal.ViewModels
{
	public class AdminScriptsTreeViewViewModel : ViewModelBase
	{
		public County SelectedCounty { get; }
		public readonly IFtpService _ftpService;

		public AdminScriptsTreeViewViewModel(County selectedCounty, IFtpService ftpService)
		{
			SelectedCounty = selectedCounty;
			_ftpService = ftpService;
			Directories = new ObservableCollection<DirectoryItemViewModel>();
			LoadDirectoriesAndFoldersFromFTP();
		}

		private void LoadDirectoriesAndFoldersFromFTP()
		{
			string rootPath = $"/FTP_DASHBOARD/ADMIN/{SelectedCounty.AdminSystem.ToUpper()}/SCRIPTS/";

			var items = _ftpService.LoadDirectoriesAndFilesFromFTP(rootPath);

			foreach (DirectoryItemViewModel item in items)
			{

				string fileExtension = Path.GetExtension(item.Path);
				if (fileExtension == ".sql" || fileExtension == "")
				{
					Directories.Add(item);
				}
			}
		}
	}
}
