using Microsoft.Extensions.Configuration;
using System.Linq;
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
		public SearchBarViewModel SearchBarViewModel { get; }
		public TabControlMainViewModel(County selectedCounty, IConfiguration configuration, SearchBarViewModel searchBarViewModel)
		{
			SelectedCounty = selectedCounty;

			string? ftpServer = configuration["FtpSettings:Server"];
			string? username = configuration["FtpSettings:Username"];
			string? password = configuration["FtpSettings:Password"];

			FtpService ftpService = new FtpService(ftpServer, username, password);

			FilterTreeViewViewModel = new FilterTreeViewViewModel();
			SearchBarViewModel = searchBarViewModel;

			CountySpecificTreeViewViewModel = new CountySpecificTreeViewViewModel(SelectedCounty, ftpService, FilterTreeViewViewModel);

			PABTreeViewViewModel = new PABTreeViewViewModel(SelectedCounty, ftpService);
			GISTreeViewViewModel = new GISTreeViewViewModel(SelectedCounty, ftpService);
			PTRTreeViewViewModel = new PTRTreeViewViewModel(SelectedCounty, ftpService);
			TabControlCamaViewModel = new TabControlCamaViewModel(SelectedCounty, ftpService, SearchBarViewModel);
			TabControlAdminViewModel = new TabControlAdminViewModel(SelectedCounty,ftpService, SearchBarViewModel);

			SearchBarViewModel.SetAllDirectories(CountySpecificTreeViewViewModel.Directories
			 .Concat(GISTreeViewViewModel.Directories)
			 .Concat(PABTreeViewViewModel.Directories)
			 .Concat(PTRTreeViewViewModel.Directories));
		}
	}
}
