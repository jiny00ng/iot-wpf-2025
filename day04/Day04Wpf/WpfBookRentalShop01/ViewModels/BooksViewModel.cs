using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MahApps.Metro.Controls.Dialogs;
using MySql.Data.MySqlClient;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using WpfBookRentalShop01.Helpers;
using WpfBookRentalShop01.Models;

namespace WpfBookRentalShop01.ViewModels
{
    public partial class BooksViewModel : ObservableObject
    {
        private readonly IDialogCoordinator dialogCoordinator;

        public ObservableCollection<Book> Books { get; set; } = new();
        public ObservableCollection<KeyValuePair<string, string>> GenreList { get; set; } = new();

        private Book _selectedBook;
        public Book SelectedBook
        {
            get => _selectedBook;
            set
            {
                SetProperty(ref _selectedBook, value);
                _isUpdate = true;
            }
        }

        private bool _isUpdate;

        public BooksViewModel(IDialogCoordinator coordinator)
        {
            dialogCoordinator = coordinator;
            LoadGenreList();
            InitVariable();
            LoadGridFromDb();
        }

        private void InitVariable()
        {
            SelectedBook = new Book
            {
                Idx = 0,
                Author = string.Empty,
                Division = string.Empty,
                Names = string.Empty,
                ReleaseDate = DateTime.Now,
                ISBN = string.Empty,
                Price = 0,
                DNames = string.Empty
            };
            _isUpdate = false;
        }

        private void LoadGenreList()
        {
            GenreList = new ObservableCollection<KeyValuePair<string, string>>();
            string query = "SELECT division, names FROM divtbl";

            using var conn = new MySqlConnection(Common.CONNSTR);
            try
            {
                conn.Open();
                using var cmd = new MySqlCommand(query, conn);
                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string division = reader.GetString("division");
                    string name = reader.GetString("names");
                    GenreList.Add(new KeyValuePair<string, string>(division, name));
                }
            }
            catch (Exception ex)
            {
                Common.LOGGER.Error(ex.Message);
            }
        }

        [RelayCommand]
        public void SetInit()
        {
            InitVariable();
        }

        [RelayCommand]
        public async Task SaveDataAsync()
        {
            try
            {
                string query;

                using (var conn = new MySqlConnection(Common.CONNSTR))
                {
                    conn.Open();

                    if (_isUpdate)
                    {
                        query = @"UPDATE bookstbl 
                                  SET author = @author,
                                      division = @division,
                                      names = @names,
                                      releaseDate = @releaseDate,
                                      isbn = @isbn,
                                      price = @price
                                  WHERE idx = @idx";
                    }
                    else
                    {
                        query = @"INSERT INTO bookstbl 
                                  (author, division, names, releaseDate, isbn, price)
                                  VALUES (@author,
                                          @division,
                                          @names,
                                          @releaseDate,
                                          @isbn,
                                          @price)";
                    }

                    using var cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@author", SelectedBook.Author);
                    cmd.Parameters.AddWithValue("@division", SelectedBook.Division);
                    cmd.Parameters.AddWithValue("@names", SelectedBook.Names);
                    cmd.Parameters.AddWithValue("@releaseDate", SelectedBook.ReleaseDate);
                    cmd.Parameters.AddWithValue("@isbn", SelectedBook.ISBN);
                    cmd.Parameters.AddWithValue("@price", SelectedBook.Price);
                    if (_isUpdate) cmd.Parameters.AddWithValue("@idx", SelectedBook.Idx);

                    var result = cmd.ExecuteNonQuery();
                    await dialogCoordinator.ShowMessageAsync(this, "저장", result > 0 ? "저장 성공" : "저장 실패");
                }
            }
            catch (Exception ex)
            {
                Common.LOGGER.Error(ex.Message);
                await dialogCoordinator.ShowMessageAsync(this, "오류", ex.Message);
            }

            LoadGridFromDb();
        }

        [RelayCommand]
        public async Task DelDataAsync()
        {
            if (!_isUpdate)
            {
                await dialogCoordinator.ShowMessageAsync(this, "삭제", "삭제할 책을 선택하세요.");
                return;
            }

            var result = await dialogCoordinator.ShowMessageAsync(this, "삭제", "정말 삭제하시겠습니까?", MessageDialogStyle.AffirmativeAndNegative);
            if (result != MessageDialogResult.Affirmative) return;

            try
            {
                string query = "DELETE FROM bookstbl WHERE idx = @idx";

                using var conn = new MySqlConnection(Common.CONNSTR);
                conn.Open();
                using var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@idx", SelectedBook.Idx);

                int resultCnt = cmd.ExecuteNonQuery();

                if (resultCnt > 0)
                {
                    Common.LOGGER.Info($"도서 데이터 {SelectedBook.Idx} / {SelectedBook.Names} 삭제완료");
                    await dialogCoordinator.ShowMessageAsync(this, "삭제", "삭제성공");
                }
                else
                {
                    Common.LOGGER.Warn("도서 데이터 삭제 실패!");
                    await dialogCoordinator.ShowMessageAsync(this, "삭제", "삭제실패!!");
                }
            }
            catch (Exception ex)
            {
                await dialogCoordinator.ShowMessageAsync(this, "오류", ex.Message);
            }

            LoadGridFromDb();
        }

        private async void LoadGridFromDb()
        {
            try
            {
                string query = @"SELECT b.Idx, b.Author, b.Division, b.Names, b.ReleaseDate, b.ISBN, b.Price,
                                        d.Names AS dNames
                                   FROM bookstbl AS b
                                   JOIN divtbl AS d ON b.Division = d.Division
                                  ORDER BY b.Idx";

                ObservableCollection<Book> books = new();

                using var conn = new MySqlConnection(Common.CONNSTR);
                conn.Open();
                using var cmd = new MySqlCommand(query, conn);
                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    books.Add(new Book
                    {
                        Idx = reader.GetInt32("Idx"),
                        Author = reader.GetString("Author"),
                        Division = reader.GetString("Division"),
                        DNames = reader.GetString("dNames"),
                        Names = reader.GetString("Names"),
                        ReleaseDate = reader.GetDateTime("ReleaseDate"),
                        ISBN = reader.GetString("ISBN"),
                        Price = reader.GetInt32("Price")
                    });
                }

                Books = books;
                Common.LOGGER.Info("도서 데이터 로드");
            }
            catch (Exception ex)
            {
                Common.LOGGER.Error(ex.Message);
                await dialogCoordinator.ShowMessageAsync(this, "오류", ex.Message);
            }
        }
    }
}
