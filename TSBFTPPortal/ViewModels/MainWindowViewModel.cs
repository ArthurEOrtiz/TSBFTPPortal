using Microsoft.Extensions.Configuration;
using System.Linq;
using TSBFTPPortal.Models;

namespace TSBFTPPortal.ViewModels
{
  public class MainWindowViewModel : ViewModelBase
  {
    public County SelectedCounty { get; set; }
    public TabControlMainViewModel TabControlMainViewModel { get; }
    public SearchBarViewModel SearchBarViewModel { get; }
    public ToggleThemeViewModel ToggleThemeViewModel { get; }
    public MainWindowViewModel(County selectedCounty, IConfiguration configuration)
    {
      SelectedCounty = selectedCounty;
			SearchBarViewModel = new SearchBarViewModel();
			TabControlMainViewModel = new TabControlMainViewModel(selectedCounty, configuration, SearchBarViewModel);
      ToggleThemeViewModel = new ToggleThemeViewModel();
    }
  }
}
