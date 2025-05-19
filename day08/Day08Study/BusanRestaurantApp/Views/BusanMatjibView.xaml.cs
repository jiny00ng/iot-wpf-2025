using MahApps.Metro.Controls;
using System.Windows;
using System.Windows.Input;
using BusanRestaurantApp.ViewModels; // ViewModel을 연결해줘야 함

namespace BusanRestaurantApp.Views
{
    /// <summary>
    /// BusanMatjibView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class BusanMatjibView : MetroWindow
    {
        public BusanMatjibView()
        {
            InitializeComponent();
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is BusanMatjibViewModel vm)
            {
                if (vm.MatjibItemDoubleClickCommand.CanExecute(null))
                {
                    vm.MatjibItemDoubleClickCommand.Execute(null);
                }
            }
        }
    }
}
