using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSBFTPPortal.Models;
using TSBFTPPortal.Services;

namespace TSBFTPPortal.ViewModels
{
	public class TabControlAdminViewModel : ViewModelBase
	{
		public County SelectedCounty { get; }
		public readonly IFtpService _ftpService;
		public AdminReportsTreeViewViewModel AdminReportsTreeViewViewModel { get; }
		public AdminScriptsTreeViewViewModel AdminScriptsTreeViewViewModel { get; }
		public AdminDocumentsTreeViewViewModel AdminDocumentsTreeViewViewModel { get; }

		public TabControlAdminViewModel(County selectedCounty, IFtpService ftpService)
		{
			SelectedCounty = selectedCounty;
			_ftpService = ftpService;

			AdminReportsTreeViewViewModel = new AdminReportsTreeViewViewModel(selectedCounty, _ftpService);
			AdminScriptsTreeViewViewModel = new AdminScriptsTreeViewViewModel(selectedCounty, _ftpService);
			AdminDocumentsTreeViewViewModel = new AdminDocumentsTreeViewViewModel(selectedCounty, _ftpService);
		}
	}
}
