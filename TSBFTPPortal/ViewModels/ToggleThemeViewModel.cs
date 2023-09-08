﻿using System;
using System.Windows;

namespace TSBFTPPortal.ViewModels
{
	public class ToggleThemeViewModel : ViewModelBase
	{
		private bool _isDarkTheme = false;
		public bool IsDarkTheme
		{
			get => _isDarkTheme;
			set
			{
				if (_isDarkTheme != value)
				{
					_isDarkTheme = value;
					OnPropertyChanged(nameof(IsDarkTheme));
					ApplySelectedTheme();

					// Save the selected theme in application settings
					string selectedTheme = value ? "Dark" : "Light";
					Properties.Settings.Default.Theme = selectedTheme;
					Properties.Settings.Default.Save();

					// Apply the selected theme
					((App)Application.Current).ApplyTheme(selectedTheme);
				}
			}
		}

		private void ApplySelectedTheme()
		{
			if(IsDarkTheme)
			{
				ApplyDarkTheme();
			}
			else
			{
				ApplyLightTheme();
			}
		}

		private void ApplyLightTheme()
		{
			Application.Current.Resources.MergedDictionaries.Clear();
			Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary
			{
				Source = new Uri("/Themes/LightTheme.xaml", UriKind.Relative)
			});
		}

		private void ApplyDarkTheme()
		{
			Application.Current.Resources.MergedDictionaries.Clear();
			Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary
			{
				Source = new Uri("/Themes/DarkTheme.xaml", UriKind.Relative)
			});
		}
	}
}
