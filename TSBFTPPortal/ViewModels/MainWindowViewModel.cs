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
    public MainWindowViewModel(County selectedCounty, IConfiguration configuration)
    {
      SelectedCounty = selectedCounty;
      TabControlMainViewModel = new TabControlMainViewModel(selectedCounty, configuration);
     
      SearchBarViewModel = new SearchBarViewModel();
      SearchBarViewModel.SetAllDirectories(TabControlMainViewModel.GISTreeViewViewModel.Directories
        .Concat(TabControlMainViewModel.PABTreeViewViewModel.Directories)
        .Concat(TabControlMainViewModel.PTRTreeViewViewModel.Directories)
        .Concat(TabControlMainViewModel.TabControlCamaViewModel.CamaScriptsTreeViewViewModel.Directories)
        .Concat(TabControlMainViewModel.TabControlCamaViewModel.CamaReportsTreeViewViewModel.Directories)
        .Concat(TabControlMainViewModel.TabControlCamaViewModel.CamaDocumentsTreeViewViewModel.Directories));
      
    }
  }
}
