using System.Windows;
using GalaSoft.MvvmLight.Messaging;
using TestInsights.Messages;

namespace TestInsights.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Messenger.Default.Register<CloseMessage>(this, m => Close());
        }
    }
}
