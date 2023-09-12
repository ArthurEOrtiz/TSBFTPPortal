using System.Linq;
using TSBFTPPortal.Models;
using TSBFTPPortal.Services;

namespace TSBFTPPortal.ViewModels
{
	public class TabControlAdminAppsViewModel : ViewModelBase
	{
		public County SelectedCounty { get; }
		public readonly FtpService _ftpService;
		public SearchBarViewModel SearchBarViewModel { get; set; }
		public AdminAppTimberTreeViewViewModel AdminAppTimberTreeViewViewModel { get; }
		public AdminAppDataExtractTreeViewViewModel AdminAppDataExtractTreeViewViewModel { get; }	

		public TabControlAdminAppsViewModel(County selectedCounty, FtpService ftpService, SearchBarViewModel searchBarViewModel)
		{
			SelectedCounty = selectedCounty;
			_ftpService = ftpService;
			SearchBarViewModel = searchBarViewModel;

			AdminAppTimberTreeViewViewModel = new AdminAppTimberTreeViewViewModel(selectedCounty, _ftpService, SearchBarViewModel);
			AdminAppDataExtractTreeViewViewModel = new AdminAppDataExtractTreeViewViewModel(selectedCounty, _ftpService, SearchBarViewModel);

			SearchBarViewModel.SetAllDirectories(AdminAppTimberTreeViewViewModel.Directories
				.Concat(AdminAppDataExtractTreeViewViewModel.Directories));
		}
	}
}
