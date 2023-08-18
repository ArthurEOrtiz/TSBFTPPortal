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
			var control = sender as Control;
			var command = GetCommand(control);

			if (control != null && command != null && command.CanExecute(null))
			{
				command.Execute(null);
			}
		}
	}
}
