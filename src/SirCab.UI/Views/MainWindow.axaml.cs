namespace SirCab.UI.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object? sender, EventArgs e)
        {
            if (sender is Window window && window.DataContext is INotifyPropertyChanged oldViewModel)
                oldViewModel.PropertyChanged -= OnViewModelPropertyChanged;

            if (DataContext is INotifyPropertyChanged newViewModel)
                newViewModel.PropertyChanged += OnViewModelPropertyChanged;
        }

        private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MainWindowViewModel.LogOut))
                Dispatcher.UIThread.Post(() =>
                {
                    LogScrollViewer?.ScrollToEnd();
                });
        }
    }
}