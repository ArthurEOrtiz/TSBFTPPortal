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
      SearchBarViewModel.SetAllDirectories(TabControlMainViewModel.CountySpecificTreeViewViewModel.Directories
        .Concat(TabControlMainViewModel.GISTreeViewViewModel.Directories)
        .Concat(TabControlMainViewModel.PABTreeViewViewModel.Directories)
        .Concat(TabControlMainViewModel.PTRTreeViewViewModel.Directories)
        .Concat(TabControlMainViewModel.TabControlCamaViewModel.CamaScriptsTreeViewViewModel.Directories)
        .Concat(TabControlMainViewModel.TabControlCamaViewModel.CamaReportsTreeViewViewModel.Directories)
        .Concat(TabControlMainViewModel.TabControlCamaViewModel.CamaDocumentsTreeViewViewModel.Directories)
        .Concat(TabControlMainViewModel.TabControlAdminViewModel.AdminReportsTreeViewViewModel.Directories)
        .Concat(TabControlMainViewModel.TabControlAdminViewModel.AdminScriptsTreeViewViewModel.Directories)
        .Concat(TabControlMainViewModel.TabControlAdminViewModel.AdminDocumentsTreeViewViewModel.Directories)
        .Concat(TabControlMainViewModel.TabControlAdminViewModel.TabControlAdminAppsViewModel.AdminAppTimberTreeViewViewModel.Directories)
        .Concat(TabControlMainViewModel.TabControlAdminViewModel.TabControlAdminAppsViewModel.AdminAppDataExtractTreeViewViewModel.Directories));
    }
  }
}
