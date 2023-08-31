using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Core.Common.CommandTrees;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSBFTPPortal.Models;
using TSBFTPPortal.Services;

namespace TSBFTPPortal.ViewModels
{
	public class CamaDocumentsTreeViewViewModel : ViewModelBase
	{
		public County SelectedCounty { get; }
		public readonly FtpService _ftpService;

		public CamaDocumentsTreeViewViewModel(County selectedCounty, FtpService ftpService)
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
					if (fileExtension != ".rpt" && fileExtension != ".sql")
					{
						Directories.Add(item);
					}
					else
					{
						Log.Error($"Invalid file, {item.Name}, in Cama Documents!");
					}
				}
				else
				{
					Log.Error($"Cama Documents, Could not find path for {item.Name}!");
				}
				
			}
		}

		private string GetRootPath()
		{
			string rootPath = string.Empty;
			if (SelectedCounty != null && SelectedCounty.CAMASystem != null)
			{
				rootPath = $"/FTP_DASHBOARD/CAMA/{SelectedCounty.CAMASystem.ToUpper()}/DOCUMENTATION/";
			}
			else
			{
				Log.Error("Cama Documents, Select County is null!");
			}

			return rootPath;
		}
	}
}
