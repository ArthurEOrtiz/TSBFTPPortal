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

		public ICommand CancelCommand { get; } 

		public ProgressWindowViewModel()
		{
			CancelCommand = new RelayCommand(Cancel);
		}

		private void Cancel(object obj)
		{
			//throw new NotImplementedException();
		}
	}
}
