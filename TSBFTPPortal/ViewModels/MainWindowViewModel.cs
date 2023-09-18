using Microsoft.Extensions.Configuration;
using TSBFTPPortal.Models;

namespace TSBFTPPortal.ViewModels
{
	public class MainWindowViewModel : ViewModelBase
	{
		public County SelectedCounty { get; set; }
		public TabControlMainViewModel TabControlMainViewModel { get; }
		public SearchBarViewModel SearchBarViewModel { get; }
		public ToggleThemeViewModel ToggleThemeViewModel { get; }
		public ChangeCountyViewModel ChangeCountyViewModel { get; }

		public MainWindowViewModel(County selectedCounty, IConfiguration configuration)
		{
			SelectedCounty = selectedCounty;
			SearchBarViewModel = new SearchBarViewModel();
			TabControlMainViewModel = new TabControlMainViewModel(selectedCounty, configuration, SearchBarViewModel);
			ToggleThemeViewModel = new ToggleThemeViewModel();
			ChangeCountyViewModel = new ChangeCountyViewModel(configuration);
		}
	}
}
