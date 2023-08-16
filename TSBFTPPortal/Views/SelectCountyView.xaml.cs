using Microsoft.Extensions.Configuration;
using System.Windows;
using TSBFTPPortal.ViewModels;

namespace TSBFTPPortal.Views
{
  /// <summary>
  /// Interaction logic for SelectCountyView.xaml
  /// </summary>
  public partial class SelectCountyView : Window
  {
 
    public SelectCountyView(IConfiguration configuration)
    {
      InitializeComponent();
      DataContext = new SelectCountyViewModel(configuration);
    }
  }
}
