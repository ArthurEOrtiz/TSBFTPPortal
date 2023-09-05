using Microsoft.Extensions.Configuration;
using Serilog;
using System.Linq;
using TSBFTPPortal.Models;
using TSBFTPPortal.Services;
using TSBFTPPortal.Views;

namespace TSBFTPPortal.ViewModels
{
	public class TabControlMainViewModel : ViewModelBase
	{
		public County SelectedCounty { get; }
		public CountySpecificTreeViewViewModel? CountySpecificTreeViewViewModel { get; }
		public FilterTreeViewViewModel FilterTreeViewViewModel { get; }
		public PABTreeViewViewModel? PABTreeViewViewModel { get; }
		public GISTreeViewViewModel? GISTreeViewViewModel { get; }
		public PTRTreeViewViewModel? PTRTreeViewViewModel { get; }
		public TabControlCamaViewModel? TabControlCamaViewModel { get; }
		public TabControlAdminViewModel? TabControlAdminViewModel { get; }
		public SearchBarViewModel SearchBarViewModel { get; }

		private bool _isAdminSystemTabVisible = false;
		public bool IsAdminSystemTabVisible
		{
			get => _isAdminSystemTabVisible;
			set
			{
				if (_isAdminSystemTabVisible != value)
				{
					_isAdminSystemTabVisible = value;
					OnPropertyChanged(nameof(IsAdminSystemTabVisible));
				}
			}
		}

		private bool _isCamaSystemTabVisible = false;
		public bool IsCamaSystemTabVisible
		{
			get => _isCamaSystemTabVisible;
			set
			{
				if (_isCamaSystemTabVisible != value)
				{
					_isCamaSystemTabVisible = value;
					OnPropertyChanged(nameof(IsCamaSystemTabVisible));
				}
			}
		}

		public TabControlMainViewModel(County selectedCounty, IConfiguration configuration, SearchBarViewModel searchBarViewModel)
		{
			SelectedCounty = selectedCounty;

			IsAdminSystemTabVisible = selectedCounty.CAMASystem != "AS400"
																&& selectedCounty.CAMASystem != "CAI"
																&& selectedCounty.CAMASystem != "Custom";

			IsCamaSystemTabVisible = selectedCounty.CAMASystem != "AS400"
															 && selectedCounty.CAMASystem != "CAI"
															 && selectedCounty.CAMASystem != "Custom";

			FilterTreeViewViewModel = new FilterTreeViewViewModel();
			SearchBarViewModel = searchBarViewModel;

			string? ftpServer = configuration["FtpSettings:Server"];
			string? username = configuration["FtpSettings:Username"];
			string? password = configuration["FtpSettings:Password"];



			if (ftpServer != null && username != null && password != null)
			{
				FtpService ftpService = new(ftpServer, username, password);
				CountySpecificTreeViewViewModel = new CountySpecificTreeViewViewModel(SelectedCounty, ftpService, FilterTreeViewViewModel, SearchBarViewModel);
				PABTreeViewViewModel = new PABTreeViewViewModel(SelectedCounty, ftpService);
				GISTreeViewViewModel = new GISTreeViewViewModel(SelectedCounty, ftpService);
				PTRTreeViewViewModel = new PTRTreeViewViewModel(SelectedCounty, ftpService);
				TabControlCamaViewModel = new TabControlCamaViewModel(SelectedCounty, ftpService, SearchBarViewModel);
				TabControlAdminViewModel = new TabControlAdminViewModel(SelectedCounty, ftpService, SearchBarViewModel);

				SearchBarViewModel.SetAllDirectories(CountySpecificTreeViewViewModel.Directories
			 .Concat(GISTreeViewViewModel.Directories)
			 .Concat(PABTreeViewViewModel.Directories)
			 .Concat(PTRTreeViewViewModel.Directories));

			}
			else
			{
				Log.Error("FTP Service parameters are null");
			}


		}
	}
}
