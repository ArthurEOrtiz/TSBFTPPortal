using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSBFTPPortal.Models;

namespace TSBFTPPortal.ViewModels
{
	public class PABTreeViewViewModel : ViewModelBase
	{
		public County SelectedCounty { get; }

		public PABTreeViewViewModel(County selectedCounty)
		{
			SelectedCounty = selectedCounty;
		}
	}
}
