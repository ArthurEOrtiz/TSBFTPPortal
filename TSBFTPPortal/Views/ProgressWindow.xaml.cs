using System.Windows;
using TSBFTPPortal.ViewModels;

namespace TSBFTPPortal.Views
{
	/// <summary>
	/// Interaction logic for ProgressWindow.xaml
	/// </summary>
	public partial class ProgressWindow : Window
	{
		public ProgressWindow()
		{
			InitializeComponent();
			DataContext = new ProgressWindowViewModel();
		}
	}
}
