using Microsoft.Extensions.Configuration;
using TSBFTPPortal.Models;


namespace TSBFTPPortal.ViewModels
{
  public class MainWindowViewModel : ViewModelBase
  {
    public County SelectedCounty { get; set; }
    public TabControlMainViewModel TabControlMainViewModel { get; }
    public MainWindowViewModel(County selectedCounty, IConfiguration configuration)
    {
      SelectedCounty = selectedCounty;
      TabControlMainViewModel = new TabControlMainViewModel(selectedCounty, configuration);
      
    }
  }
}
