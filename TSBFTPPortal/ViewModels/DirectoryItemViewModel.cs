﻿using Serilog;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using TSBFTPPortal.Commands;
using TSBFTPPortal.Services;

namespace TSBFTPPortal.ViewModels
{
	public class DirectoryItemViewModel : ViewModelBase
	{
		public string? Name { get; set; }
		public string? Path { get; set; }
		public bool IsDirectory { get; set; }
		public bool IsFile => !IsDirectory;
		public ObservableCollection<DirectoryItemViewModel> Items { get; } = new ObservableCollection<DirectoryItemViewModel>();
		public ICommand DownloadCommand { get; private set; }
		private readonly IFtpService _ftpService;

		public DirectoryItemViewModel(IFtpService ftpService)
		{
			_ftpService = ftpService;
			DownloadCommand = new RelayCommand(Download);
		}

		private void Download(object obj)
		{
			if (IsFile && !string.IsNullOrEmpty(Path)) 
			{ 
				try
				{
					_ftpService.DownloadFile(Path);
				}
				catch (Exception ex)
				{
					Log.Error($"Error downloading file: {ex.Message}");
				}
			}
		}
	}
}
