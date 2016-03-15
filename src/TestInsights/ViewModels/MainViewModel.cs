using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using TestInsights.Messages;

namespace TestInsights.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private string _message = "Hello, World!";

        public MainViewModel()
        {
            CloseCommand = new RelayCommand(Close);
        }

        public string Message
        {
            get { return _message; }
            set { Set(() => Message, ref _message, value); }
        }

        public ICommand CloseCommand { get; }

        public void Close()
        {
            Messenger.Default.Send(new CloseMessage());
        }
    }
}
