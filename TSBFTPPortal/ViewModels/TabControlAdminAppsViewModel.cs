using TSBFTPPortal.Models;
using TSBFTPPortal.Services;

namespace TSBFTPPortal.ViewModels
{
	public class TabControlAdminAppsViewModel : ViewModelBase
	{
		public County SelectedCounty { get; }
		public readonly FtpService _ftpService;
		public AdminAppTimberTreeViewViewModel AdminAppTimberTreeViewViewModel { get; }
		public AdminAppDataExtractTreeViewViewModel AdminAppDataExtractTreeViewViewModel { get; }	

		public TabControlAdminAppsViewModel(County selectedCounty, FtpService ftpService)
		{
			SelectedCounty = selectedCounty;
			_ftpService = ftpService;
			AdminAppTimberTreeViewViewModel = new AdminAppTimberTreeViewViewModel(selectedCounty, _ftpService);
			AdminAppDataExtractTreeViewViewModel = new AdminAppDataExtractTreeViewViewModel(selectedCounty, _ftpService);

		}
	}
}
