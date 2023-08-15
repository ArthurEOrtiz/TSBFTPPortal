using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using TSBFTPPortal.Views;

namespace TSBFTPPortal
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			Window selectCountyView = new SelectCountyView();
			selectCountyView.Show();

			InitializeLocalAppDataFolder();

		}

		private void InitializeLocalAppDataFolder()
		{
			string commonAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
			string dataFolderPath = System.IO.Path.Combine(commonAppDataPath, "TSBFTPDashBoard");

			if (!Directory.Exists(dataFolderPath))
			{
				try
				{
					Directory.CreateDirectory(dataFolderPath);
				}
				catch (Exception ex) 
				{
					throw;
				}
			}

		}
	}
}
