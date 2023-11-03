using Serilog;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using TSBFTPPortal.Commands;
using TSBFTPPortal.ViewModels;
using TSBFTPPortal.Views;
using WinSCP;

namespace TSBFTPPortal.Services
{
	public class FtpService
	{
		private readonly string _ftpServer;
		private readonly string _username;
		private readonly string _password;
		private readonly string _sshHostKeyFingerprint;
		private CancellationTokenSource? _cancellationTokenSource;
		private bool _isCancellationRequested;
		private ProgressWindow? _progressWindow;
		public delegate void ProgressUpdateHandler(double progressPercentage);
		public event ProgressUpdateHandler ProgressUpdated;

		public FtpService(string ftpServer, string username, string password, string SshHostKeyFingerprint)
		{
			_ftpServer = ftpServer;
			_username = username;
			_password = password;
			_sshHostKeyFingerprint = SshHostKeyFingerprint;
		}


		public async Task<ObservableCollection<DirectoryItemViewModel>> LoadDirectoriesAndFilesFromFTPAsync(string rootPath)
		{
			var items = new ObservableCollection<DirectoryItemViewModel>();

			try
			{
				using (var session = new Session())
				{
					session.DisableVersionCheck = true;

					session.Open(new SessionOptions
					{
						Protocol = Protocol.Sftp,
						PortNumber = 22,
						HostName = _ftpServer,
						UserName = _username,
						Password = _password,
						SshHostKeyFingerprint = _sshHostKeyFingerprint,
					});

					await LoadSubDirectoriesAndFilesAsync(session, rootPath, items);
				}
			}
			catch (Exception ex)
			{
				Log.Error($"Failure to Connect to SFTP : {ex.Message}");
			}

			return items;
		}

		private async Task LoadSubDirectoriesAndFilesAsync(Session session, string path, ObservableCollection<DirectoryItemViewModel> items)
		{
			RemoteDirectoryInfo directory = await Task.Run(() => session.ListDirectory(path));

			foreach (RemoteFileInfo fileInfo in directory.Files)
			{
				if (fileInfo.Name != "." && fileInfo.Name != "..")
				{
					var subItemViewModel = new DirectoryItemViewModel(this)
					{
						Name = fileInfo.Name,
						Path = fileInfo.FullName,
						IsDirectory = fileInfo.IsDirectory,
					};

					items.Add(subItemViewModel);

					if (fileInfo.IsDirectory)
					{
						await LoadSubDirectoriesAndFilesAsync(session, fileInfo.FullName, subItemViewModel.Items);
					}
				}
			}
		}


		public async Task DownloadFileAsync(string path)
		{
			Debug.WriteLine("*******Download Started************");
			if (!IsInternetAvailable())
			{
				Log.Error("Internet connection lost.");
				ShowErrorMessage("No internet connection!");
				return;
			}

			try
			{

				InitializeProgressWindow();

				await Task.Run(async () =>
				{
					using (Session session = new Session())
					{
						// Configure session options
						SessionOptions sessionOptions = new SessionOptions
						{
							Protocol = Protocol.Sftp,
							HostName = _ftpServer,
							UserName = _username,
							Password = _password,
							SshHostKeyFingerprint = _sshHostKeyFingerprint,
							PortNumber = 22,
						};

						session.FileTransferProgress += (sender, e) =>
						{
							if (e.FileName != null)
							{
								double progressPercentage = (double)(e.FileProgress * 100);
								UpdateProgress(progressPercentage);
							}
							else
							{
								// Handle transfer errors here
								Log.Error($"Error transferring file");
							}
						};

						// Connect to the SFTP server
						session.Open(sessionOptions);

						string fileName = Path.GetFileName(path);
						string targetFilePath = GetTargetFilePath(fileName);

						if (targetFilePath != null)
						{
							await PerformFileDownload(session, targetFilePath, path);
						}
					}
				});

			}
			catch (OperationCanceledException)
			{
				Log.Information("Download cancelled.");
			}
			catch (Exception ex)
			{
				Log.Error($"Error downloading file: {ex.Message}");
				ShowErrorMessage($"Error downloading file: {ex.Message}");
			}
			finally
			{
				_progressWindow?.Close();
			}
		}

		private async Task PerformFileDownload(Session session, string targetFilePath, string remoteFilePath)
		{
			try
			{
				// Check if the file exists locally
				if (File.Exists(targetFilePath))
				{
					using (var remoteStream = session.GetFile(remoteFilePath))
					using (var memoryStream = new MemoryStream())
					{
						// Download the file contents to a MemoryStream
						remoteStream.CopyTo(memoryStream);

						// Convert the MemoryStream to a byte array
						byte[] fileBytes = memoryStream.ToArray();

						var fileActionViewModel = new FileActionDialogViewModel();
						var fileActionDialog = CreateFileActionDialog(fileActionViewModel);

						// Show the custom dialog and await the user's choice
						bool? dialogResult = ShowDialog(fileActionDialog);

						// Handle the user's choice
						await HandleUserChoice(session, targetFilePath, remoteFilePath, fileActionViewModel.SelectedAction, dialogResult, fileBytes);
					}
				}
				else
				{
					await DownloadFileFromSftp(session, targetFilePath, remoteFilePath);
				}
			}
			catch (Exception ex)
			{
				HandleError(ex);
			}
		}

		private static string GetTargetFilePath(string fileName)
		{
			string fileExtension = Path.GetExtension(fileName);

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

		private FileActionDialog CreateFileActionDialog(FileActionDialogViewModel fileActionViewModel)
		{
			var fileActionDialog = new FileActionDialog
			{
				DataContext = fileActionViewModel,
				Owner = Application.Current.MainWindow,
				Topmost = true,
				Top = Application.Current.MainWindow.Top,
				Left = Application.Current.MainWindow.Left
			};

			// Set the CloseAction to close the dialog
			fileActionViewModel.CloseAction = (result) => fileActionDialog.DialogResult = result;

			return fileActionDialog;
		}

		private bool? ShowDialog(FileActionDialog fileActionDialog)
		{
			// Show the custom dialog and await the user's choice
			return fileActionDialog.ShowDialog();
		}

		private async Task HandleUserChoice(Session session, string targetFilePath, string remoteFilePath, string selectedAction, bool? dialogResult)
		{
			if (dialogResult.HasValue && dialogResult.Value)
			{
				switch (selectedAction)
				{
					case "Overwrite":
						await DownloadFileFromSftp(session, targetFilePath, remoteFilePath);
						break;
					case "CreateCopy":
						string newFilePath = GetUniqueFileName(targetFilePath);
						await DownloadFileFromSftp(session, newFilePath, remoteFilePath);
						break;
					case "Cancel":
						// User canceled the operation, do nothing
						break;
				}
			}
		}

		private async Task DownloadFileFromSftp(Session session, string targetFilePath, string remoteFilePath)
		{
			try
			{
				// Start the download
				TransferOptions transferOptions = new TransferOptions { TransferMode = TransferMode.Binary };
				TransferOperationResult transferResult = session.GetFiles(remoteFilePath, targetFilePath, false, transferOptions);


				// Check if the transfer was successful
				if (!transferResult.IsSuccess)
				{
					Log.Error($"Error downloading file: {transferResult.Failures[0].Message}");
					ShowErrorMessage($"Error downloading file: {transferResult.Failures[0].Message}");
					return;
				}

				session.Dispose();

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
				HandleError(ex, targetFilePath);
			}
		}

		private void UpdateProgress(double progressPercentage)
		{
			try
			{
				if (_isCancellationRequested)
				{
					_cancellationTokenSource.Cancel();
					return;
				}

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

		public void InitializeProgressWindow()
		{
			_progressWindow = new ProgressWindow();

			var mainWindow = Application.Current.MainWindow;
			_progressWindow.Owner = mainWindow;

			// Set the location of the progress window 
			_progressWindow.Left = mainWindow.Left + (mainWindow.Width - _progressWindow.Width) / 2;
			_progressWindow.Top = mainWindow.Top + (mainWindow.Height - _progressWindow.Height) / 2;

			_progressWindow.Show();
			var viewModel = (ProgressWindowViewModel)_progressWindow.DataContext;
			viewModel.CancelCommand = new RelayCommand(Cancel);
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

		private void HandleError(Exception ex, string targetFilePath = null)
		{
			Log.Error($"An unexpected error occurred: {ex.Message}");
			ShowErrorMessage($"Error downloading file: {ex.Message}");
		}

		private void ShowErrorMessage(string message)
		{
			Application.Current.Dispatcher.Invoke(() =>
			{
				var errorDialog = new ErrorDialog();
				var viewModel = new ErrorDialogViewModel(message);
				errorDialog.DataContext = viewModel;

				errorDialog.Owner = Application.Current.MainWindow;

				double dialogLeft = Application.Current.MainWindow.Left + (Application.Current.MainWindow.Width - errorDialog.Width) / 2;
				double dialogTop = Application.Current.MainWindow.Top + (Application.Current.MainWindow.Height - errorDialog.Height) / 2;

				errorDialog.Left = dialogLeft;
				errorDialog.Top = dialogTop;

				errorDialog.Topmost = true;

				viewModel.CloseAction = (result) =>
				{
					errorDialog.DialogResult = result;
					errorDialog.Close();
				};

				errorDialog.ShowDialog();
			});
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

			// Set up session options
			SessionOptions sessionOptions = new SessionOptions
			{
				Protocol = Protocol.Sftp,
				HostName = _ftpServer,
				UserName = _username,
				Password = _password,
				SshHostKeyFingerprint = _sshHostKeyFingerprint,

			};

			sessionOptions.AddRawSettings("FSProtocol", "2");


			try
			{
				using (Session session = new Session())
				{
					// Connect to the SFTP server
					session.Open(sessionOptions);

					// Create a remote file info for the file you want to read
					RemoteFileInfo remoteFileInfo = session.GetFileInfo(path);

					using (Stream remoteStream = session.GetFile(path))
					using (MemoryStream memoryStream = new MemoryStream())
					{
						// Download the file contents to a MemoryStream
						remoteStream.CopyTo(memoryStream);

						// Convert the MemoryStream to a string
						string fileContent = Encoding.UTF8.GetString(memoryStream.ToArray());

						return fileContent;
					}
				}
			}
			catch (Exception ex)
			{
				Log.Error($"Error reading JSON file from SFTP: {ex.Message}");
				return null;
			}
		}

		private bool IsInternetAvailable()
		{
			try
			{
				using (var ping = new System.Net.NetworkInformation.Ping())
				{
					var reply = ping.Send("8.8.8.8"); // Ping Google's DNS server
					return reply.Status == System.Net.NetworkInformation.IPStatus.Success;
				}
			}
			catch
			{
				return false; // Exception occurred, probably no internet
			}
		}
	}
}
