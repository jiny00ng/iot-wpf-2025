using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using WpfMqttSubApp.ViewModels;

namespace WpfMqttSubApp.Views
{
    /// <summary>
    /// MainView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainView : MetroWindow
    {
        public MainView()
        {
            InitializeComponent();
            this.DataContext = new MainViewModel(DialogCoordinator.Instance);
        }

    }
}
