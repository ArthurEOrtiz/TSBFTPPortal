using System;
using System.Windows.Input;
using TSBFTPPortal.Commands;

namespace TSBFTPPortal.ViewModels
{
	public class FileActionDialogViewModel : ViewModelBase
	{
		private string selectedAction;

		public string SelectedAction
		{
			get => selectedAction;
			set
			{
				if (selectedAction != value)
				{
					selectedAction = value;
					OnPropertyChanged(nameof(SelectedAction));
					DialogResult = true;
				}
			}
		}

		public bool? DialogResult { get; private set; }
		public Action<bool?> CloseAction { get; set; }


		public ICommand OverwriteCommand { get; }
		public ICommand CreateCopyCommand { get; }
		public ICommand CancelCommand { get; }

		public FileActionDialogViewModel()
		{
			OverwriteCommand = new RelayCommand(_ => SetDialogResult("Overwrite"));
			CreateCopyCommand = new RelayCommand(_ => SetDialogResult("CreateCopy"));
			CancelCommand = new RelayCommand(_ => SetDialogResult("Cancel"));
		}

		private void SetDialogResult(string action)
		{
			SelectedAction = action;
			DialogResult = true;
			CloseAction?.Invoke(true); // Close the dialog
		}
	}
}
