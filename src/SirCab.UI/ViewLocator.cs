namespace SirCab.UI
{
    public class ViewLocator : IDataTemplate
    {
        public Control? Build(object? param)
        {
            if (param is null)
                return null;

            // AOT-compatible explicit mapping instead of using Type.GetType()
            return param switch
            {
                MainWindowViewModel => new MainWindow(),
                _ => new TextBlock { Text = "Not Found: " + param.GetType().Name }
            };
        }

        public bool Match(object? data) => data is ViewModelBase;
    }
}