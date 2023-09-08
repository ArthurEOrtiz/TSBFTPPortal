using System.Threading;
using System.Windows.Input;
using TSBFTPPortal.Commands;

namespace TSBFTPPortal.ViewModels
{
	public class ProgressWindowViewModel : ViewModelBase
	{
		private double _progressPercentage;
		public double ProgressPercentage
		{
			get => _progressPercentage;
			set
			{
				_progressPercentage = value;
				OnPropertyChanged(nameof(ProgressPercentage));
			}
		}

		private string _statusMessage;
		public string StatusMessage
		{
			get => _statusMessage;
			set
			{
				_statusMessage = value;
				OnPropertyChanged(nameof(StatusMessage));
			}
		}

		private ICommand _cancelCommand;
		public ICommand CancelCommand
		{
			get => _cancelCommand;
			set
			{
				_cancelCommand = value;
				OnPropertyChanged(nameof(CancelCommand));
			}
		}

		private CancellationTokenSource _cancellationTokenSource;

		public ProgressWindowViewModel()
		{
			_statusMessage = string.Empty;
			_cancelCommand = new RelayCommand(Cancel);
			_cancellationTokenSource = new CancellationTokenSource();
		}

		private void Cancel(object obj)
		{
			_cancellationTokenSource.Cancel();
			StatusMessage = "Download cancelled";
		}
	}
}
