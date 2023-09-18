using Microsoft.Extensions.Configuration;
using System.Windows;
using System.Windows.Input;
using TSBFTPPortal.Commands;
using TSBFTPPortal.Views;

namespace TSBFTPPortal.ViewModels
{
	public class ChangeCountyViewModel : ViewModelBase
	{
		private readonly IConfiguration _configuration;

		public ICommand ChangeCountyCommand { get; private set; }

		public ChangeCountyViewModel(IConfiguration configuration)
		{
			_configuration = configuration;
			ChangeCountyCommand = new RelayCommand(_ => ChangeCounty());
		}

		public void ChangeCounty()
		{
			var selectCountyView = new SelectCountyView(_configuration);

			var currentWindow = App.Current.MainWindow;
			selectCountyView.Owner= Application.Current.MainWindow;

			selectCountyView.Left = currentWindow.Left + ((currentWindow.Width - selectCountyView.Width) / 2);
			selectCountyView.Top = currentWindow.Top + ((currentWindow.Height - selectCountyView.Height) / 2);

			selectCountyView.Show();
			selectCountyView.Owner = null;

			Application.Current.MainWindow = selectCountyView;

			currentWindow.Close();
		}
	}
}
