using Serilog;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TSBFTPPortal.Behaviors
{
	public static class DoubleClickBehavior
	{
		public static readonly DependencyProperty CommandProperty =
				DependencyProperty.RegisterAttached(
						"Command",
						typeof(ICommand),
						typeof(DoubleClickBehavior),
						new UIPropertyMetadata(CommandChanged));

		public static ICommand GetCommand(DependencyObject obj)
		{
			return (ICommand)obj.GetValue(CommandProperty);
		}

		public static void SetCommand(DependencyObject obj, ICommand value)
		{
			obj.SetValue(CommandProperty, value);
		}

		private static void CommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var control = d as Control;
			if (control != null)
			{
				control.MouseDoubleClick += OnMouseDoubleClick;
			}
		}

		private static void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			Control? control = sender as Control;
			if (control != null)
			{
				ICommand command = GetCommand(control);

				if (command != null && command.CanExecute(null))
				{
					command.Execute(null);
				}
				else
				{
					Log.Error("OnMouseDoubleClick, command is null!");
				}
			}
			else
			{
				Log.Error("OnMouseDoubleClick, control is null!");
			}
		}
	}
}
