using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSBFTPPortal.Models;

namespace TSBFTPPortal.ViewModels
{
	public class TabControlMainViewModel : ViewModelBase
	{
		public County SelectedCounty { get; }
		public PABTreeViewViewModel PABTreeViewViewModel { get; }
		public TabControlMainViewModel(County selectedCounty)
		{
			SelectedCounty = selectedCounty;
			PABTreeViewViewModel = new PABTreeViewViewModel(SelectedCounty);
		}
	}
}
