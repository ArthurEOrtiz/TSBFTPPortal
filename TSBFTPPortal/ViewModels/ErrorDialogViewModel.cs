using System;
using System.Windows.Input;
using TSBFTPPortal.Commands;

namespace TSBFTPPortal.ViewModels
{
	public class ErrorDialogViewModel : ViewModelBase
	{
		private string message;

		public string Message
		{
			get => message;
			set
			{
				message = value;
				OnPropertyChanged(nameof(Message));
			}
		}

		public bool? DialogResult { get; private set; }
		public Action<bool?> CloseAction { get; set; }

		public ICommand Okay { get; }

		public ErrorDialogViewModel(string message)
		{
			Message = message;
			Okay = new RelayCommand(_ => CloseWindow());
		}

		private void CloseWindow()
		{
			DialogResult = true;
			CloseAction?.Invoke(true);
		}
	}
}
