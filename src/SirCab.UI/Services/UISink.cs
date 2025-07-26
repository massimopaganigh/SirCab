namespace SirCab.UI.Services
{
    public class UISink(MainWindowViewModel mainWindowViewModel) : ILogEventSink
    {
        private readonly ITextFormatter _textFormatter = new MessageTemplateTextFormatter("[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}");

        public void Emit(LogEvent logEvent)
        {
            StringWriter stringWriter = new();

            _textFormatter.Format(logEvent, stringWriter);

            string? message = stringWriter.ToString();

            Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
            {
                mainWindowViewModel.LogOut = (mainWindowViewModel.LogOut ?? string.Empty) + message;
            });
        }
    }
}