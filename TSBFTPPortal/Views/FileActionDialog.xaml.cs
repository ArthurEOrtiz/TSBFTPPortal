using System.Diagnostics;
using System.Windows;

namespace TSBFTPPortal.Views
{
	/// <summary>
	/// Interaction logic for FileActionDialog.xaml
	/// </summary>
	public partial class FileActionDialog : Window
	{
		public FileActionDialog()
		{
			InitializeComponent();
			Loaded += FileActionDialog_Loaded; // Attach the Loaded event handler
		}

		private void FileActionDialog_Loaded(object sender, RoutedEventArgs e)
		{
			// DataContext is now initialized; you can access it here.
			Debug.WriteLine("DataContext of FileActionDialog: " + DataContext.GetType().Name);
		}
	}
}
