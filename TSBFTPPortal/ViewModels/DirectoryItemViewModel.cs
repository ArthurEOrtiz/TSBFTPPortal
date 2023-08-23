using Serilog;
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
		private bool _isVisible = true;
		public bool IsVisible
		{
			get => _isVisible;
			set
			{
				_isVisible = value;
				OnPropertyChanged(nameof(IsVisible));
			}
		}

		private bool _isHighlighted = false;
		public bool IsHighlighted
		{
			get => _isHighlighted;
			set
			{
				_isHighlighted = value;
				OnPropertyChanged(nameof(IsHighlighted));
			}
		}

		public ObservableCollection<DirectoryItemViewModel> Items { get; } = new ObservableCollection<DirectoryItemViewModel>();
		public ICommand DownloadCommand { get; private set; }
		private readonly FtpService _ftpService;

		public DirectoryItemViewModel(FtpService ftpService)
		{
			_ftpService = ftpService;
			DownloadCommand = new RelayCommand(Download);
		}

		private async void Download(object obj)
		{
			if (IsFile && !string.IsNullOrEmpty(Path)) 
			{ 
				try
				{
					await _ftpService.DownloadFileAsync(Path);
				}
				catch (Exception ex)
				{
					Log.Error($"Error downloading file: {ex.Message}");
				}
			}
		}
	}
}
