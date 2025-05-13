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

        private ObservableCollection<Book> _books;
        public ObservableCollection<Book> Books
        {
            get => _books;
            set => SetProperty(ref _books, value);
        }

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
                Price = 0
            };
            _isUpdate = false;
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
                string query = string.Empty;

                using (MySqlConnection conn = new MySqlConnection(Common.CONNSTR))
                {
                    conn.Open();

                    if (_isUpdate)
                    {
                        query = @"UPDATE booktbl 
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
                        query = @"INSERT INTO booktbl 
                              (author, division, names, releaseDate, isbn, price)
                              VALUES 
                              (@author, @division, @names,  @releaseDate, @isbn, @price)";
                    }

                    MySqlCommand cmd = new MySqlCommand(query, conn);
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
                string query = "DELETE FROM booktbl WHERE idx = @idx";

                using var conn = new MySqlConnection(Common.CONNSTR);
                {
                    conn.Open();
                    using var cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@idx", SelectedBook.Idx);

                    int resultCnt = cmd.ExecuteNonQuery();

                    if (resultCnt >0)
                    {
                        Common.LOGGER.Info($"도서 데이터 {SelectedBook.Idx} / {SelectedBook.Names} 삭제완료");
                        await this.dialogCoordinator.ShowMessageAsync(this, "삭제", "삭제성공");
                    }
                    else
                    {
                        Common.LOGGER.Warn("도서 데이터 삭제 실패!");
                        await this.dialogCoordinator.ShowMessageAsync(this, "삭제", "삭제실패!!");
                    }
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
                string query = "SELECT idx, author, division, names, releaseDate, isbn, price FROM booktbl";
                ObservableCollection<Book> books = new ObservableCollection<Book>();

                using (MySqlConnection conn = new MySqlConnection(Common.CONNSTR))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        var idx = reader.GetInt32("idx");
                        var author = reader.GetString("author");
                        var division = reader.GetString("division");
                        var names = reader.GetString("names");
                        var releaseDate = reader.GetDateTime("releaseDate");
                        var isbn = reader.GetString("isbn");
                        var price = reader.GetInt32("price");

                        books.Add(new Book
                        {
                            Idx = idx,
                            Author = author,
                            Division = division,
                            Names = names,
                            ReleaseDate = releaseDate,
                            ISBN = isbn,
                            Price = price
                        });
                    }

                    Books = books;
                }
            }
                
            catch (Exception ex)
            {
                await dialogCoordinator.ShowMessageAsync(this, "오류", ex.Message);
            }
        }
    }
}

