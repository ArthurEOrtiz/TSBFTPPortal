using System.Windows;
using TSBFTPPortal.ViewModels;

namespace TSBFTPPortal.Views
{
  /// <summary>
  /// Interaction logic for SelectCountyView.xaml
  /// </summary>
  public partial class SelectCountyView : Window
  {
    public SelectCountyView()
    {
      InitializeComponent();
      DataContext = new SelectCountyViewModel();
    }
  }
}
