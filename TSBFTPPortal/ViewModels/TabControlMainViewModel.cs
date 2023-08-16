using Microsoft.Extensions.Configuration;
using System.Windows;
using TSBFTPPortal.Models;
using TSBFTPPortal.Services;

namespace TSBFTPPortal.ViewModels
{
	public class TabControlMainViewModel : ViewModelBase
	{
	
		public County SelectedCounty { get; }
		public PABTreeViewViewModel PABTreeViewViewModel { get; }
		public TabControlMainViewModel(County selectedCounty, IConfiguration configuration)
		{
			SelectedCounty = selectedCounty;
			

			string? ftpServer = configuration["FtpSettings:Server"];
			string? username = configuration["FtpSettings:Username"];
			string? password = configuration["FtpSettings:Password"];


			IFtpService ftpService = new FtpService(ftpServer, username, password);
			PABTreeViewViewModel = new PABTreeViewViewModel(SelectedCounty, ftpService);
			
		}
	}
}
