using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TSBFTPPortal.Behaviors
{
	public class EnterKeyBehavior
	{
		public static readonly DependencyProperty CommandProperty =
						DependencyProperty.RegisterAttached(
								"Command",
								typeof(ICommand),
								typeof(EnterKeyBehavior),
								new PropertyMetadata(CommandChanged));

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
				control.KeyUp += OnKeyUp;
			}
		}

		private static void OnKeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter || e.Key == Key.Return)
			{
				Control control = sender as Control;
				if (control != null)
				{
					ICommand command = GetCommand(control);

					if (command != null && command.CanExecute(null))
					{
						command.Execute(null);
					}
				}
			}
		}
	}
}
