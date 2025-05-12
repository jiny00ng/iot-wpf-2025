﻿using Caliburn.Micro;
using MahApps.Metro.Controls.Dialogs;
using System.Windows;

namespace WpfBasicApp01.ViewModels
{
    public class MainViewModel : Conductor<object>
    {
        // 메시지박스, 다이얼로그를 위한 변수
        private readonly IDialogCoordinator _dialogCoordinator;

        private string _greeting;

        public string Greeting {
            get => _greeting;
            set
            {
                _greeting = value;
                NotifyOfPropertyChange(() => Greeting); // 속성 값 바뀐 것을 알려줘야 함!
            }
        }


        // IDialogCoordinator는 DI로 주입 받음
        public MainViewModel(IDialogCoordinator dialogCoordinator) 
        {
            _dialogCoordinator = dialogCoordinator;
            Greeting = "Hello, Calibrun.Micro!";
        }
        public async void SayHello()
        {
            Greeting = "Hi, Everyone~!!!";
            // WinForms 방식
            // MessageBox.Show("Hi, Everyone~", "Greeting", MessageBoxButton.OK, MessageBoxImage.Information);
            await _dialogCoordinator.ShowMessageAsync(this, "Greeting", "Hi, Everyone~");
        }

    }
}