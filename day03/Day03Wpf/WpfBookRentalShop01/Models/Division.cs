using CommunityToolkit.Mvvm.ComponentModel;

namespace WpfBookRentalShop01.Models
{
    public class Genre : ObservableObject
    {
        public string _division;
        private string _names;

        public string Divison {
            get => _division;
            set => SetProperty(ref _division, value);
        }

        public string Names {
            get => _names;
            set => SetProperty(ref _names, value);  
        }
        public string Division { get; internal set; }
    }
}
