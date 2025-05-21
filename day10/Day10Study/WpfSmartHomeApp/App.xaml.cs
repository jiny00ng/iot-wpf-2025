using System.Windows;
using WpfSmartHomeApp.ViewModels;
using WpfSmartHomeApp;
using WpfSmartHomeApp.Helpers;

namespace WpfSmartHomeApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
        var viwModel = new MainViewModel();
        var view = new MainWindow
        {
            DataContext = viwModel,
        };
            Common.LOGGER.Info("스마트홈 모니터링앱 실행");
            view.ShowDialog();
        }
    }
}
