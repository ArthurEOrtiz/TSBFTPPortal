using System.Linq;
using TSBFTPPortal.Models;
using TSBFTPPortal.Services;

namespace TSBFTPPortal.ViewModels
{
	public class TabControlAdminViewModel : ViewModelBase
	{
		public County SelectedCounty { get; }
		public readonly FtpService _ftpService;
		public SearchBarViewModel SearchBarViewModel { get; }
		public AdminReportsTreeViewViewModel AdminReportsTreeViewViewModel { get; }
		public AdminScriptsTreeViewViewModel AdminScriptsTreeViewViewModel { get; }
		public AdminDocumentsTreeViewViewModel AdminDocumentsTreeViewViewModel { get; }
		public TabControlAdminAppsViewModel TabControlAdminAppsViewModel { get; }

		public TabControlAdminViewModel(County selectedCounty, FtpService ftpService, SearchBarViewModel searchBarViewModel)
		{
			SelectedCounty = selectedCounty;
			_ftpService = ftpService;
			SearchBarViewModel = searchBarViewModel;

			AdminReportsTreeViewViewModel = new AdminReportsTreeViewViewModel(selectedCounty, _ftpService);
			AdminScriptsTreeViewViewModel = new AdminScriptsTreeViewViewModel(selectedCounty, _ftpService);
			AdminDocumentsTreeViewViewModel = new AdminDocumentsTreeViewViewModel(selectedCounty, _ftpService);
			TabControlAdminAppsViewModel = new TabControlAdminAppsViewModel(selectedCounty, _ftpService, SearchBarViewModel);

			SearchBarViewModel.SetAllDirectories(AdminReportsTreeViewViewModel.Directories
			 .Concat(AdminScriptsTreeViewViewModel.Directories)
			 .Concat(AdminDocumentsTreeViewViewModel.Directories));
		}
	}
}
