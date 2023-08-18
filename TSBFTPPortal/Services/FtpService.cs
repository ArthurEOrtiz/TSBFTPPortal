﻿using System;
using System.CodeDom;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FluentFTP;
using Serilog;
using SQLitePCL;
using TSBFTPPortal.Commands;
using TSBFTPPortal.ViewModels;
using TSBFTPPortal.Views;

namespace TSBFTPPortal.Services
{
	public class FtpService : IFtpService
	{
		private readonly string _ftpServer;
		private readonly string _username;
		private readonly string _password;
		private CancellationTokenSource _cancellationTokenSource;
		private bool _isCancellationRequested;
		private ProgressWindow _progressWindow;

		public FtpService(string ftpServer, string username, string password)
		{
			_ftpServer = ftpServer;
			_username = username;
			_password = password;
		}

		public ObservableCollection<DirectoryItemViewModel> LoadDirectoriesAndFilesFromFTP( string rootPath)
		{
			var items = new ObservableCollection<DirectoryItemViewModel>();

			using (var ftpClient = new FtpClient(_ftpServer, new System.Net.NetworkCredential(_username, _password)))
			{
				try
				{
					ftpClient.Connect();

					LoadSubDirectoriesAndFiles(ftpClient, rootPath, items);
				}
				catch (Exception ex)
				{
					Log.Error($"Failure to Connect to FTP : {ex.Message}");
				}
			}

			return items;
		}

		private void LoadSubDirectoriesAndFiles(FtpClient ftpClient, string path, ObservableCollection<DirectoryItemViewModel> items)
		{
			var subItems = ftpClient.GetListing(path);

			foreach (var subItem in subItems)
			{
				var subItemViewModel = new DirectoryItemViewModel(this)
				{
					Name = subItem.Name,
					Path = subItem.FullName,
					IsDirectory = subItem.Type == FluentFTP.FtpObjectType.Directory
				};

				items.Add(subItemViewModel);

				if (subItem.Type == FluentFTP.FtpObjectType.Directory)
				{
					LoadSubDirectoriesAndFiles(ftpClient, subItem.FullName, subItemViewModel.Items);
				}
			}
		}

		public void DownloadFile(string path)
		{
			 _ = DownloadFileAsync(path);
		}

		public async Task DownloadFileAsync(string path)
		{
			using (var ftpClient = new FtpClient(_ftpServer, new System.Net.NetworkCredential(_username, _password)))
			{
				_cancellationTokenSource = new CancellationTokenSource();
				_isCancellationRequested = false;
				_progressWindow = new ProgressWindow();
				InitializeProgressWindow();

				try
				{
					ftpClient.Connect();
					Log.Information("Connected to Ftp Server for download");

					string fileName = System.IO.Path.GetFileName(path);
					string fileExtension = Path.GetExtension(path);

					string targetFilePath = GetTargetFilePath(fileName, fileExtension);

					if (targetFilePath != null)
					{
						await PerformFileDownload(ftpClient, targetFilePath, path);
					}
				}
				catch (OperationCanceledException)
				{
					Log.Information("Download cancelled.");
				}
				catch (Exception ex)
				{
					Log.Error($"Error downloading file: {ex.Message}");
				}
				finally
				{
					_progressWindow.Close();
				}
			}
		}

		private void InitializeProgressWindow()
		{
			_progressWindow.DataContext = new ProgressWindowViewModel();
			_progressWindow.Show();
			var viewModel = (ProgressWindowViewModel)_progressWindow.DataContext;
			viewModel.CancelCommand = new RelayCommand(Cancel);
		}

		private string GetTargetFilePath(string fileName, string fileExtension)
		{
			switch (fileExtension)
			{
				case ".rpt":
					string reportsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "TSBFTPPortal", "Reports");
					return Path.Combine(reportsFolder, fileName);

				case ".sql":
					string scriptsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "TSBFTPPortal", "Scripts");
					return Path.Combine(scriptsFolder, fileName);

				default:
					string downloadsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
					return System.IO.Path.Combine(downloadsFolder, fileName);
			}
		}

		private async Task PerformFileDownload(FtpClient ftpClient, string targetFilePath, string remoteFilePath)
		{
			FtpStatus status = await Task.Run(() => ftpClient.DownloadFile(
					targetFilePath,
					remoteFilePath,
					FtpLocalExists.Overwrite,
					FtpVerify.None,
					progressInfo => UpdateProgress(progressInfo)),
					_cancellationTokenSource.Token);

			if (status == FtpStatus.Success)
			{
				Log.Information($"File downloaded to: {targetFilePath}");
				string fileExtension = Path.GetExtension(targetFilePath);
				if (fileExtension != ".rpt" && fileExtension != ".sql")
				{
					Process.Start(new ProcessStartInfo(targetFilePath) { UseShellExecute = true });
				}
			}
			else
			{
				Log.Error($"Error downloading file. Status: {status}");
			}
		}

		private void UpdateProgress(FtpProgress progressInfo)
		{
			try
			{
				if (_isCancellationRequested)
				{
					_cancellationTokenSource.Cancel();
					return;
				}

				double progressPercentage = (double)progressInfo.Progress;
				_progressWindow.Dispatcher.Invoke(() =>
				{
					var viewModel = (ProgressWindowViewModel)_progressWindow.DataContext;
					viewModel.StatusMessage = "Downloading file . . . ";
					viewModel.ProgressPercentage = progressPercentage;
				});
			}
			catch (Exception ex)
			{
				Log.Error($"Progress update error: {ex}");
			}
		}

		private void Cancel(object obj)
		{
			_isCancellationRequested = true;
			_cancellationTokenSource.Cancel();
			var viewModel = (ProgressWindowViewModel)_progressWindow.DataContext;
			viewModel.StatusMessage = "Download cancelled.";
		}

	}
}
