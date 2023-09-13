using FluentFTP;
using Serilog;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using TSBFTPPortal.Commands;
using TSBFTPPortal.ViewModels;
using TSBFTPPortal.Views;

namespace TSBFTPPortal.Services
{
	public class FtpService
	{
		private readonly string _ftpServer;
		private readonly string _username;
		private readonly string _password;
		private CancellationTokenSource? _cancellationTokenSource;
		private bool _isCancellationRequested;
		private ProgressWindow? _progressWindow;

		public FtpService(string ftpServer, string username, string password)
		{
			_ftpServer = ftpServer;
			_username = username;
			_password = password;
		}


		public ObservableCollection<DirectoryItemViewModel> LoadDirectoriesAndFilesFromFTP(string rootPath)
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

		public async Task DownloadFileAsync(string path)
		{

			using (var ftpClient = new FtpClient(_ftpServer, new System.Net.NetworkCredential(_username, _password)))
			{
				_cancellationTokenSource = new CancellationTokenSource();
				_isCancellationRequested = false;
				_progressWindow = new ProgressWindow();
				_progressWindow.Show();
				InitializeProgressWindow();

				try
				{
					ftpClient.Connect();
					Log.Information("Connected to Ftp Server for download");

					string fileName = Path.GetFileName(path);
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

		private static string GetTargetFilePath(string fileName, string fileExtension)
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
					return Path.Combine(downloadsFolder, fileName);
			}
		}

		private async Task PerformFileDownload(FtpClient ftpClient, string targetFilePath, string remoteFilePath)
		{
			try
			{
				await Task.Run(() =>
				{
					bool fileExists = File.Exists(targetFilePath);

					if (fileExists)
					{
						Application.Current.Dispatcher.Invoke(() =>
						{
							var fileActionViewModel = new FileActionDialogViewModel();

							// Create and set up the custom dialog window
							var fileActionDialog = new FileActionDialog { DataContext = fileActionViewModel };

							// Set the CloseAction to close the dialog
							fileActionViewModel.CloseAction = (result) => fileActionDialog.DialogResult = result;

							// Show the custom dialog
							fileActionDialog.ShowDialog();

							// Handle the user's choice
							switch (fileActionViewModel.SelectedAction)
							{
								case "Overwrite":
									DownloadFileFromFtp(ftpClient, targetFilePath, remoteFilePath, FtpLocalExists.Overwrite);
									break;
								case "CreateCopy":
									string newFilePath = GetUniqueFileName(targetFilePath);
									DownloadFileFromFtp(ftpClient, newFilePath, remoteFilePath);
									break;
								case "Cancel":
									// User canceled the operation, do nothing
									break;
							}
						});
					}
					else
					{
						DownloadFileFromFtp(ftpClient, targetFilePath, remoteFilePath);
					}
				});
			}
			catch (Exception ex)
			{
				HandleError(ex);
			}
		}


		private void DownloadFileFromFtp(FtpClient ftpClient, string targetFilePath, string remoteFilePath, FtpLocalExists localExists = FtpLocalExists.Skip)
		{
			try
			{
				ftpClient.DownloadFile(
						targetFilePath,
						remoteFilePath,
						localExists,
						FtpVerify.None,
						progressInfo => UpdateProgress(progressInfo));

				LogInformation(targetFilePath);
				string fileExtension = Path.GetExtension(targetFilePath);

				if (fileExtension != ".rpt" && fileExtension != ".sql")
				{
					OpenFileWithShellExecute(targetFilePath, fileExtension);
				}
				else if (fileExtension == ".rpt")
				{
					RunCrystalReportsViewer(targetFilePath);
				}
				else if (fileExtension == ".sql")
				{
					RunScriptRunner(targetFilePath);
				}
			}
			catch (Exception ex)
			{
				HandleFtpException(ex, targetFilePath);
			}
		}

		private string GetUniqueFileName(string filePath)
		{
			string directory = Path.GetDirectoryName(filePath);
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
			string fileExtension = Path.GetExtension(filePath);

			int counter = 1;
			string newFilePath = filePath;

			while (File.Exists(newFilePath))
			{
				string newFileName = $"{fileNameWithoutExtension} ({counter}){fileExtension}";
				newFilePath = Path.Combine(directory, newFileName);
				counter++;
			}

			return newFilePath;
		}

		private void LogInformation(string targetFilePath)
		{
			Log.Information($"File downloaded to: {targetFilePath}");
		}

		private void OpenFileWithShellExecute(string targetFilePath, string fileExtension)
		{
			try
			{
				Process.Start(new ProcessStartInfo(targetFilePath) { UseShellExecute = true });
				Log.Information($"Download successful: {fileExtension}");
			}
			catch (Exception ex)
			{
				HandleError(ex, targetFilePath);
			}
		}

		private void RunCrystalReportsViewer(string targetFilePath)
		{
			try
			{
				Task.Run(() =>
				{
					new CrystalReportsViewerService(targetFilePath).ExecuteProgram();
					Log.Information($"Report successfully downloaded : {targetFilePath}");
				});
			}
			catch (Exception ex)
			{
				HandleError(ex, targetFilePath);
			}
		}

		private void RunScriptRunner(string targetFilePath)
		{
			try
			{
				Task.Run(() =>
				{
					new ScriptRunnerService(targetFilePath).ExecuteProgram();
					Log.Information($"Script successfully downloaded: {targetFilePath}");
				});
			}
			catch (Exception ex)
			{
				HandleError(ex, targetFilePath);
			}
		}

		private void HandleFtpException(Exception ex, string targetFilePath)
		{
			if (ex is FluentFTP.Exceptions.FtpCommandException commandException)
			{
				if (commandException.Message.Contains("Failed to open file"))
				{
					Log.Error($"FtpCommandException: {ex.Message}");
				}
				else
				{
					Log.Error($"Download Error: {targetFilePath}\n{ex.Message}");
				}
			}
			else if (ex is FluentFTP.Exceptions.FtpMissingObjectException missingObjectException)
			{
				Log.Error($"FtpMissingObjectException: {ex.Message}");
			}

			ShowErrorMessage(ex.Message);
		}

		private void HandleError(Exception ex, string targetFilePath = null)
		{
			Log.Error($"An unexpected error occurred: {ex.Message}");
			ShowErrorMessage($"Error downloading file: {ex.Message}");
		}

		private void ShowErrorMessage(string message)
		{
			MessageBox.Show(message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
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

		public async Task<string?> ReadJsonFileFromFTPAsync(string path)
		{
			using (var ftpClient = new FtpClient(_ftpServer, new NetworkCredential(_username, _password)))
			{
				try
				{
					ftpClient.Connect();
					Log.Information("Connected to FTP Server for JSON file reading");

					using var stream = await Task.Run(() => ftpClient.OpenRead(path));
					using var memoryStream = new MemoryStream();
					await stream.CopyToAsync(memoryStream);
					return Encoding.UTF8.GetString(memoryStream.ToArray());
				}
				catch (Exception ex)
				{
					Log.Error($"Error reading JSON file from FTP: {ex.Message}");
					return null;
				}
			}
		}

	}
}
