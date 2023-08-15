using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TSBFTPPortal.Commands
{
	class RelayCommand : ICommand
	{
		private readonly Action<object> execute;
		private readonly Predicate<object> canExecute;

		public event EventHandler? CanExecuteChanged;

		public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
		{
			this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
			this.canExecute = canExecute;
		}

		public bool CanExecute(object? parameter)
		{
			return canExecute == null || canExecute(parameter);
		}

		public void Execute(object? parameter)
		{
			execute(parameter);
		}

		public void RaiseCanExecuteChanged()
		{
			CanExecuteChanged?.Invoke(this, EventArgs.Empty);
		}
	}
}
