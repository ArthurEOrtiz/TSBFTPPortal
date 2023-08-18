using System;
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
				_progressWindow.DataContext = new ProgressWindowViewModel();

				_progressWindow.Show();

				var viewModel = (ProgressWindowViewModel) _progressWindow.DataContext;
				viewModel.CancelCommand = new RelayCommand(Cancel);

				try
				{
					ftpClient.Connect();

					// Get the file name and extension from the path
					string fileName = System.IO.Path.GetFileName(path);
					string fileExtension = Path.GetExtension(path);

					string targetFilePath = string.Empty;

					switch (fileExtension)
					{
						case ".rpt":
							string reportsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "TSBFTPPortal", "Reports");
							targetFilePath = Path.Combine(reportsFolder, fileName);
							break;

						case ".sql":
							string scriptsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "TSBFTPPortal", "Scripts");
							targetFilePath = Path.Combine(scriptsFolder, fileName);
							break;

						default:
							string downloadsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
							targetFilePath = System.IO.Path.Combine(downloadsFolder, fileName);
							break;
					}

					if (targetFilePath != null)
					{
						FtpStatus status = await Task.Run(() => ftpClient.DownloadFile(
								targetFilePath,
								path,
								FtpLocalExists.Overwrite,
								FtpVerify.None,
								(progressInfo) =>
								{
									//Check for cancellation 
									if (_isCancellationRequested)
									{
										_cancellationTokenSource.Cancel();
										return;
									}

									double progressPercentage = (double)progressInfo.Progress;
									_progressWindow.Dispatcher.Invoke(() =>
									{
										viewModel.StatusMessage = "Downloading file . . . ";
										viewModel.ProgressPercentage = progressPercentage;
									});
								}), _cancellationTokenSource.Token);

						if (status == FtpStatus.Success)
						{
							Log.Information($"File downloaded to: {targetFilePath}");

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

		private void Cancel(object obj)
		{
			_isCancellationRequested = true;
			_cancellationTokenSource.Cancel();
			var viewModel = (ProgressWindowViewModel)_progressWindow.DataContext;
			viewModel.StatusMessage = "Download cancelled.";
		}
	}
}
