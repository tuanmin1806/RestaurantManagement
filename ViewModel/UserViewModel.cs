using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client.NativeInterop;
using Microsoft.VisualBasic.ApplicationServices;
using ProjectPRN212.Models;
using ProjectPRN212.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace ProjectPRN212.ViewModel
{
    class UserViewModel : BaseViewModel
    {

        private ObservableCollection<Models.User> _ListUsers;
        public ObservableCollection<Models.User> ListUsers { get => _ListUsers; set { _ListUsers = value; OnPropertyChanged(); } }

        private ObservableCollection<Models.Role> _ListRoles;
        public ObservableCollection<Models.Role> ListRoles
        {
            get => _ListRoles;
            set { _ListRoles = value; OnPropertyChanged(); }
        }


        private string _SearchKeyword;
        public string SearchKeyword
        {
            get => _SearchKeyword;
            set
            {
                _SearchKeyword = value;
                OnPropertyChanged();
                ApplyFilter();
            }
        }

        private ICollectionView _UsersView;
        public ICollectionView UsersView
        {
            get { return _UsersView; }
            set { _UsersView = value; OnPropertyChanged(); }
        }
        public ICommand CreateCommand { get; set; }
        public ICommand UpdateCommand { get; set; }
        public ICommand CancelCommand { get; set; }



        private string? _UserName;
        public string? UserName { get => _UserName; set { _UserName = value; OnPropertyChanged(); } }

        private string? _FullName;
        public string? FullName { get => _FullName; set { _FullName = value; OnPropertyChanged(); } }


        private string? _Address;
        public string? Address { get => _Address; set { _Address = value; OnPropertyChanged(); } }

        private string? _PassWord;
        public string? PassWord { get => _PassWord; set { _PassWord = value; OnPropertyChanged(); } }

        private string? _Email;
        public string? Email { get => _Email; set { _Email = value; OnPropertyChanged(); } }

        private int? _Status;
        public int? Status { get => _Status; set { _Status = value; OnPropertyChanged(); } }

        private string _Role;
        public string Role { get => _Role; set { _Role = value; OnPropertyChanged(); } }

        private Models.User _SelectedItem;

        public Models.User SelectedItem
        {
            get => _SelectedItem;
            set
            {
                _SelectedItem = value;
                OnPropertyChanged();
                if (SelectedItem != null)
                {
                    UserName = SelectedItem.Username;
                    FullName = SelectedItem.Fullname;
                    Address = SelectedItem.Address;
                    Email = SelectedItem.Email;
                    Status = SelectedItem.Status;
                    SelectedRole = SelectedItem.Role;
                }
            }
        }
        private Models.Role _SelectedRole;
        public Models.Role SelectedRole
        {
            get => _SelectedRole;
            set
            {
                _SelectedRole = value;
                OnPropertyChanged();
            }
        }
     

        public UserViewModel()
        {
            ListUsers = new ObservableCollection<Models.User>(DataProvider.Ins.DB.Users.Include(u => u.Role));
            ListRoles = new ObservableCollection<Models.Role>(DataProvider.Ins.DB.Roles);

            // tạo một chế độ xem có thể lọc được từ danh sách người dùng.
            UsersView = CollectionViewSource.GetDefaultView(ListUsers);
            UsersView.Filter = UserFilter;

            CreateCommand = new RelayCommand<object>((p) =>
            {
                if (string.IsNullOrEmpty(FullName) || string.IsNullOrEmpty(Email))
                    return false;
                return true;
            }, (p) =>
            {
                string generatedUserName = GenerateUserName(FullName);
                string generatedPassword = GeneratePassword();
                var user = new Models.User()
                {
                    Fullname = FullName,
                    Username = generatedUserName,
                    PassWord = generatedPassword,
                    Address = Address,
                    Email = Email,
                    Status = 1,
                    RoleId = SelectedRole.RoleId,

                };
                DataProvider.Ins.DB.Users.Add(user);
                MessageBox.Show("Create User successfully !!!");
                DataProvider.Ins.DB.SaveChanges();
                ListUsers.Add(user);

                var subject = $"no-reply-email-GreenZRestaurantServices <{generatedUserName} created>";
                var body = $"Chào {FullName},<br><br>Tài khoản của bạn đã được tạo thành công.<br>Tài khoản: {generatedUserName}<br>Mật khẩu: {generatedPassword}<br><br>Trân trọng!";
                var emailService = new EmailService("smtp.gmail.com", 587, "tuanmin18062003@gmail.com", "olqp ucvh iwyd hrwj");
                emailService.SendEmail(Email, subject, body);
            }
        );

            UpdateCommand = new RelayCommand<object>((p) =>
            {
                if (SelectedItem == null)
                    return false;
                var userList = DataProvider.Ins.DB.Users.Where(x => x.UserId == SelectedItem.UserId);
                if (userList != null && userList.Count() != 0)
                    return true;
                return false;
            }, (p) =>
            {
                var EditUser = DataProvider.Ins.DB.Users.Where(x => x.UserId == SelectedItem.UserId).FirstOrDefault();
                EditUser.Fullname = FullName;
                EditUser.Address = Address;
                EditUser.Email = Email;
                EditUser.Status = Status;
                EditUser.RoleId = SelectedRole.RoleId;

                MessageBox.Show("Update User successfully !!!");
                DataProvider.Ins.DB.SaveChanges();
                ListUsers = new ObservableCollection<Models.User>(DataProvider.Ins.DB.Users);
            }
          );
        }

        private bool UserFilter(object obj)
        {
            if (string.IsNullOrEmpty(SearchKeyword))
                return true;

            var trimmedKeyword = SearchKeyword.Trim();

            var user = obj as Models.User;
            if (user != null && !string.IsNullOrEmpty(user.Username))
            {
                return user.Username.Contains(trimmedKeyword, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }

        private void ApplyFilter()
        {
            UsersView.Refresh();
        }
        public void HandleTextChanged(string searchText)
        {
            SearchKeyword = searchText;
        }
        // Method to generate Username from FullName with Vietnamese diacritics removed
        private string GenerateUserName(string fullName)
        {
            if (string.IsNullOrEmpty(fullName))
                return string.Empty;

            // Remove Vietnamese diacritics
            string normalizedFullName = RemoveVietnameseDiacritics(fullName);

            // Split the normalized full name into words
            var nameParts = normalizedFullName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (nameParts.Length < 2)
                return normalizedFullName; // Return full name if it cannot be split into more than one part

            // Take the last name as the base for the username
            var lastName = nameParts.Last();
            var initials = string.Join("", nameParts.Take(nameParts.Length - 1).Select(n => n[0]));

            string baseUserName = $"{lastName}{initials}";

            // Check if the base username already exists
            string generatedUserName = baseUserName;
            int count = 1;
            while (DataProvider.Ins.DB.Users.Any(u => u.Username == generatedUserName))
            {
                generatedUserName = $"{baseUserName}{count}";
                count++;
            }

            return generatedUserName;
        }
        // Method to remove Vietnamese diacritics from a string
        private string RemoveVietnameseDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        // Method to generate random password
        private string GeneratePassword()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 8)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
