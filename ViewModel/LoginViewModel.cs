using Microsoft.EntityFrameworkCore;
using Microsoft.Xaml.Behaviors.Core;
using ProjectPRN212.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ProjectPRN212.ViewModel
{
    class LoginViewModel : BaseViewModel
    {
        //Thuộc tính lưu trạng thái đăng nhập
        public bool IsLogin { get; set; }

        private User _loggedInUser;
        public User LoggedInUser
        {
            get => _loggedInUser;
            set
            {
                _loggedInUser = value;
                OnPropertyChanged(nameof(LoggedInUser));
            }
        }

        private string _UserName;
        private string _Password;
        public string UserName { get => _UserName; set { _UserName = value; OnPropertyChanged(); } }
        public string Password { get => _Password; set { _Password = value; OnPropertyChanged(); } }
        public ICommand LoginCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        public ICommand PasswordChangedCommand { get; set; }
        public LoginViewModel()
        {
            IsLogin = false;
            LoginCommand = new RelayCommand<Window>((p) => { return true; }, (p) => { Login(p); });
            CloseCommand = new RelayCommand<Window>((p) => { return true; }, (p) => { p.Close(); });
            PasswordChangedCommand = new RelayCommand<PasswordBox>((p) => { return true; }, (p) => { Password = p.Password; });
        }

        void Login(Window p)
        {
            var user = DataProvider.Ins.DB.Users
               .Include(u => u.Role)
               .FirstOrDefault(a => a.Username == UserName && a.PassWord == Password);

            if (user != null)
            {
                IsLogin = true;
                LoggedInUser = user;
                p.Close();
            }
            else
            {
                IsLogin = false;
                MessageBox.Show("Wrong UserName or Password");
            }
        }
    }
}
