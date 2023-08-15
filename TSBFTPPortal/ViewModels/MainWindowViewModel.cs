using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSBFTPPortal.ViewModels
{
  public class MainWindowViewModel
  {
    public string SelectedCounty { get; }
    public MainWindowViewModel(string selectedCounty)
    {
      SelectedCounty = selectedCounty;
    }
  }
}
