using Microsoft.Extensions.Configuration;
using TSBFTPPortal.Models;
using TSBFTPPortal.Services;
using TSBFTPPortal.Views;

namespace TSBFTPPortal.ViewModels
{
	public class TabControlMainViewModel : ViewModelBase
	{
		public County SelectedCounty { get; }
		public CountySpecificTreeViewViewModel CountySpecificTreeViewViewModel { get; }
		public FilterTreeViewViewModel FilterTreeViewViewModel { get; }
		public PABTreeViewViewModel PABTreeViewViewModel { get; }
		public GISTreeViewViewModel GISTreeViewViewModel { get; }
		public PTRTreeViewViewModel PTRTreeViewViewModel { get; }
		public TabControlCamaViewModel TabControlCamaViewModel { get; }
		public TabControlAdminViewModel TabControlAdminViewModel { get; }
		public TabControlMainViewModel(County selectedCounty, IConfiguration configuration)
		{
			SelectedCounty = selectedCounty;

			string? ftpServer = configuration["FtpSettings:Server"];
			string? username = configuration["FtpSettings:Username"];
			string? password = configuration["FtpSettings:Password"];

			FtpService ftpService = new FtpService(ftpServer, username, password);

			FilterTreeViewViewModel = new FilterTreeViewViewModel();

			CountySpecificTreeViewViewModel = new CountySpecificTreeViewViewModel(SelectedCounty, ftpService, FilterTreeViewViewModel);

			PABTreeViewViewModel = new PABTreeViewViewModel(SelectedCounty, ftpService);
			GISTreeViewViewModel = new GISTreeViewViewModel(SelectedCounty, ftpService);
			PTRTreeViewViewModel = new PTRTreeViewViewModel(SelectedCounty, ftpService);
			TabControlCamaViewModel = new TabControlCamaViewModel(SelectedCounty, ftpService);
			TabControlAdminViewModel = new TabControlAdminViewModel(SelectedCounty,ftpService);
		}
	}
}
