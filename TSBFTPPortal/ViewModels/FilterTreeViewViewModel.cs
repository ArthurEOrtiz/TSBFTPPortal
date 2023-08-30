using TSBFTPPortal.ViewModels;

namespace TSBFTPPortal.Views
{
	public class FilterTreeViewViewModel : ViewModelBase
	{
		private bool _isReportsSelected;
		public bool IsReportsSelected
		{
			get => _isReportsSelected;
			set
			{
				if (_isReportsSelected != value)
				{
					_isReportsSelected = value;
					OnPropertyChanged(nameof(IsReportsSelected));
				}
			}
		}

		private bool _isScriptsSelected;
		public bool IsScriptsSelected
		{
			get => _isScriptsSelected;
			set
			{
				if (_isScriptsSelected != value)
				{
					_isScriptsSelected = value;
					OnPropertyChanged(nameof(IsScriptsSelected));
				}
			}
		}

		private bool _isDocumentsSelected;
		public bool IsDocumentsSelected
		{
			get => _isDocumentsSelected;
			set
			{
				if (_isDocumentsSelected != value)
				{
					_isDocumentsSelected = value;
					OnPropertyChanged(nameof(IsDocumentsSelected));
				}
			}
		}

		public FilterTreeViewViewModel()
		{

		}
	}
}
