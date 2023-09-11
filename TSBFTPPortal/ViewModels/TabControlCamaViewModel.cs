using System.Linq;
using TSBFTPPortal.Models;
using TSBFTPPortal.Services;

namespace TSBFTPPortal.ViewModels
{
	public class TabControlCamaViewModel : ViewModelBase
	{
		public County SelectedCounty { get; }
		public readonly FtpService _ftpService;
		public SearchBarViewModel SearchBarViewModel { get; }
		public CamaReportsTreeViewViewModel CamaReportsTreeViewViewModel { get; }
		public CamaScriptsTreeViewViewModel CamaScriptsTreeViewViewModel { get; }
		public CamaDocumentsTreeViewViewModel CamaDocumentsTreeViewViewModel { get; }

		public TabControlCamaViewModel(County selectedCounty, FtpService ftpService, SearchBarViewModel searchBarViewModel)
		{
			SelectedCounty = selectedCounty;
			_ftpService = ftpService;
			SearchBarViewModel = searchBarViewModel;

			CamaReportsTreeViewViewModel = new CamaReportsTreeViewViewModel(selectedCounty, ftpService);
			CamaScriptsTreeViewViewModel = new CamaScriptsTreeViewViewModel(selectedCounty, ftpService);
			CamaDocumentsTreeViewViewModel = new CamaDocumentsTreeViewViewModel(selectedCounty, ftpService);

			SearchBarViewModel.SetAllDirectories(CamaScriptsTreeViewViewModel.Directories
			 .Concat(CamaReportsTreeViewViewModel.Directories)
			 .Concat(CamaDocumentsTreeViewViewModel.Directories));

		}
	}
}
