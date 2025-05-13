using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace WpfBookRentalShop01.Models
{
    public class Book : ObservableObject
    {
        private int _idx;
        private string _author;
        private string _division;
        private string _names;
        private DateTime _releaseDate;
        private string _isbn;
        private int _price;
        private string _genreName;

        public int Idx
        {
            get => _idx;
            set => SetProperty(ref _idx, value);
        }

        public string Author
        {
            get => _author;
            set => SetProperty(ref _author, value);
        }

        public string Division
        {
            get => _division;
            set => SetProperty(ref _division, value);
        }

        public string Names
        {
            get => _names;
            set => SetProperty(ref _names, value);
        }

        public DateTime ReleaseDate
        {
            get => _releaseDate;
            set => SetProperty(ref _releaseDate, value);
        }

        public string ISBN
        {
            get => _isbn;
            set => SetProperty(ref _isbn, value);
        }

        public int Price
        {
            get => _price;
            set => SetProperty(ref _price, value);
        }

        public string GenreName
        {
            get => _genreName;
            set => SetProperty(ref _genreName, value);
        }
    }
}

