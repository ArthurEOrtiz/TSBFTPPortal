using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSBFTPPortal.ViewModels
{
	public class DirectoryViewModel : ViewModelBase
	{
		public string Name { get; set; }
		public string Path { get; set; }
		public bool IsDirectory => true;
		public bool IsFile => false;
		public ObservableCollection<FileViewModel> Files { get; } = new ObservableCollection<FileViewModel>();
		public ObservableCollection<DirectoryViewModel> SubDirectories { get; } = new ObservableCollection<DirectoryViewModel>();
	}
}
