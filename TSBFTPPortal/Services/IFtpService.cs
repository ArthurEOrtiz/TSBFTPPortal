using System.Collections.ObjectModel;
using System.Threading.Tasks;
using TSBFTPPortal.ViewModels;

namespace TSBFTPPortal.Services
{
	public interface IFtpService
	{
		ObservableCollection<DirectoryItemViewModel> LoadDirectoriesAndFilesFromFTP(string rootPath);
		void DownloadFile(string path);
	}
}
