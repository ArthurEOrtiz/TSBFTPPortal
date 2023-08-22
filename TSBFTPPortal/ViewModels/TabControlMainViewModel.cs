using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Windows;
using TSBFTPPortal.Models;
using TSBFTPPortal.Services;

namespace TSBFTPPortal.ViewModels
{
	public class TabControlMainViewModel : ViewModelBase
	{
		public County SelectedCounty { get; }
		public CountySpecificTreeViewViewModel CountySpecificTreeViewViewModel { get; }
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


			IFtpService ftpService = new FtpService(ftpServer, username, password);

			CountySpecificTreeViewViewModel = new CountySpecificTreeViewViewModel(SelectedCounty, ftpService);
			PABTreeViewViewModel = new PABTreeViewViewModel(SelectedCounty, ftpService);
			GISTreeViewViewModel = new GISTreeViewViewModel(SelectedCounty, ftpService);
			PTRTreeViewViewModel = new PTRTreeViewViewModel(SelectedCounty, ftpService);
			TabControlCamaViewModel = new TabControlCamaViewModel(SelectedCounty, ftpService);
			TabControlAdminViewModel = new TabControlAdminViewModel(SelectedCounty,ftpService);
		}
	}
}
