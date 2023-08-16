using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSBFTPPortal.ViewModels
{
	public class FileViewModel : ViewModelBase
	{
		public string? Name { get; set; }
		public string? Path { get; set; }
		public bool IsDirectory => false;
		public bool IsFile => true;
	}
}
