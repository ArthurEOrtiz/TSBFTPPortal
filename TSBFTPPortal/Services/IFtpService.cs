using System.Collections.Generic;
using System.Collections.ObjectModel;
using TSBFTPPortal.ViewModels;

namespace TSBFTPPortal.Services
{
	public interface IFtpService
	{
		ObservableCollection<DirectoryItemViewModel> LoadDirectoriesAndFilesFromFTP(string rootPath);
	}
}
