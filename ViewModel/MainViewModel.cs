using ProjectPRN212.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using System.Windows.Media;
using System.ComponentModel;
using System.Windows.Data;
using System.Net;


namespace ProjectPRN212.ViewModel
{
    public class MainViewModel : BaseViewModel
    {


        public bool isLoaded = false;
        public ICommand ShowBillCommand { get; set; }
        public ICommand LoadedWindowCommand { get; set; }
        public ICommand FoodsCommand { get; set; }
        public ICommand ReportsCommand { get; set; }
        public ICommand UsersCommand { get; set; }
        public ICommand TablesCommand { get; set; }
        public ICommand MyAccountCommand { get; set; }
        public ICommand AddFoodCommand { get; set; }
        public ICommand ChangePassword { get; set; }
        public ICommand CancelCommand { get; set; }
        public ICommand UpdateCommand { get; set; }
        public ICommand ConfirmCommand { get; set; }
        public ICommand CheckoutBillCommand { get; set; } // Thêm lệnh thanh toán


        private ObservableCollection<TableFood> _listTables;
        public ObservableCollection<TableFood> ListTables
        {
            get => _listTables;
            set
            {
                if (_listTables != value)
                {
                    _listTables = value;
                    OnPropertyChanged(nameof(ListTables));
                    UpdateTableCounts();
                }
            }
        }


        private ObservableCollection<FoodCategory> _ListFoodCategory;
        public ObservableCollection<FoodCategory> ListFoodCategory { get => _ListFoodCategory; set { _ListFoodCategory = value; OnPropertyChanged(); } }

        private FoodCategory _selectedFoodCategory;
        public FoodCategory SelectedFoodCategory
        {
            get => _selectedFoodCategory;
            set
            {
                _selectedFoodCategory = value;
                OnPropertyChanged(nameof(SelectedFoodCategory));
                LoadFoodsByCategory();
            }
        }

        private int _countEmptyTables;
        public int CountEmptyTables
        {
            get => _countEmptyTables;
            set
            {
                _countEmptyTables = value;
                OnPropertyChanged(nameof(CountEmptyTables));
            }
        }

        private int _countOccupiedTables;
        public int CountOccupiedTables
        {
            get => _countOccupiedTables;
            set
            {
                _countOccupiedTables = value;
                OnPropertyChanged(nameof(CountOccupiedTables));
            }
        }

        private int _countTotalTables;
        public int CountTotalTables
        {
            get => _countTotalTables;
            set
            {
                _countTotalTables = value;
                OnPropertyChanged(nameof(CountTotalTables));
            }
        }

        private TableFood _selectedTable;
        public TableFood SelectedTable
        {
            get => _selectedTable;
            set
            {
                _selectedTable = value;
                OnPropertyChanged(nameof(SelectedTable));
            }
        }

        private DateTime _currentDate;
        public DateTime CurrentDate
        {
            get { return _currentDate; }
            set
            {
                _currentDate = value;
                OnPropertyChanged(nameof(CurrentDate));
            }
        }

        private ObservableCollection<Food> _listfoods;
        public ObservableCollection<Food> ListFoods
        {
            get => _listfoods;
            set { _listfoods = value; OnPropertyChanged(); }
        }

        private Food _selectedFood;
        public Food SelectedFood
        {
            get => _selectedFood;
            set { _selectedFood = value; OnPropertyChanged(); }
        }


        private ObservableCollection<BillItem> _selectedTableBill;
        public ObservableCollection<BillItem> SelectedTableBill
        {
            get => _selectedTableBill;
            set
            {
                _selectedTableBill = value;
                OnPropertyChanged(nameof(SelectedTableBill));
            }
        }
        private decimal _totalPrice;
        public decimal TotalPrice
        {
            get => _totalPrice;
            set
            {
                _totalPrice = value;
                OnPropertyChanged(nameof(TotalPrice));
            }
        }

        private decimal _discount;
        public decimal Discount
        {
            get { return _discount; }
            set
            {
                _discount = value;
                OnPropertyChanged(nameof(Discount));
            }
        }
        private int _quantity;
        public int Quantity
        {
            get { return _quantity; }
            set
            {
                _quantity = value;
                OnPropertyChanged(nameof(Quantity));
            }
        }
        private string? _loggedInUserName;
        public string? LoggedInUserName
        {
            get => _loggedInUserName;
            set
            {
                _loggedInUserName = value;
                OnPropertyChanged(nameof(LoggedInUserName));
            }
        }

        private string? _fullName;
        public string? FullName
        {
            get => _fullName;
            set
            {
                _fullName = value;
                OnPropertyChanged(nameof(FullName));
            }
        }

        private string? _address;
        public string? Address
        {
            get => _address;
            set
            {
                _address = value;
                OnPropertyChanged(nameof(Address));
            }
        }

        private string? _email;
        public string? Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged(nameof(Email));
            }
        }

        private string? _UserRole;
        public string? UserRole
        {
            get => _UserRole;
            set { _UserRole = value; OnPropertyChanged(); }
        }
        private User _currentUser;
        public User CurrentUser
        {
            get => _currentUser;
            set
            {
                _currentUser = value;
                OnPropertyChanged(nameof(CurrentUser));
            }
        }

        // mọi thứ xử lý sẽ nằm trong này
        public MainViewModel()
        {
            ListTables = new ObservableCollection<TableFood>();
            CurrentDate = DateTime.Today;
            LoadedWindowCommand = new RelayCommand<Window>((p) => true, (p) =>
            {
                if (isLoaded)
                    return;
                isLoaded = true;
                if (p == null)
                    return;
                p.Hide();
                LoginWindow loginWindow = new LoginWindow();
                loginWindow.ShowDialog();

                if (loginWindow.DataContext == null)
                    return;
                var loginVM = loginWindow.DataContext as LoginViewModel;

                if (loginVM != null && loginVM.IsLogin)
                {
                    LoadFoodCategories();
                    LoadTablesFood();
                    // Set logged-in user information
                    OnLoginSuccess(loginVM.LoggedInUser);
                    LoggedInUser(loginVM.LoggedInUser);
                    // Lấy vai trò của người dùng từ LoginViewModel
                    UserRole = loginVM.LoggedInUser.Role.RoleName;
                    if (UserRole.Equals("Admin")) // Giả sử vai trò admin có RoleId là 1
                    {
                        ManageUsersWindow manageUsersWindow = new ManageUsersWindow();
                        manageUsersWindow.ShowDialog();
                    }
                    else if (UserRole.Equals("Staff")) // Giả sử vai trò nhân viên có RoleId là 2
                    {
                        MainWindow mainWindow = new MainWindow();
                        mainWindow.Show();
                    }
                    p.Close(); // Đóng cửa sổ hiện tại sau khi hiển thị cửa sổ tương ứng
                }
                else
                {
                    p.Close();
                }
            });
            FoodsCommand = new RelayCommand<object>((p) => { return true; }, (p) => { FoodsWindow wd = new FoodsWindow(); wd.ShowDialog(); });
            ReportsCommand = new RelayCommand<object>((p) => { return true; }, (p) => { ReportsWindow wd = new ReportsWindow(); wd.ShowDialog(); });
            UsersCommand = new RelayCommand<object>((p) => { return true; }, (p) => { ManageUsersWindow wd = new ManageUsersWindow(); wd.ShowDialog(); });
            TablesCommand = new RelayCommand<object>((p) => { return true; }, (p) => { ManageTablesWindow wd = new ManageTablesWindow(); wd.ShowDialog(); });
            ChangePassword = new RelayCommand<object>((p) => { return true; }, (p) => { ChangePassword wd = new ChangePassword(); wd.ShowDialog(); });
            MyAccountCommand = new RelayCommand<object>((p) => { return true; }, (p) => { MyAccountWindow wd = new MyAccountWindow(); wd.ShowDialog(); });
            UpdateCommand = new RelayCommand<object>((p) => true, (p) => UpdateUserInfo(FullName, Address, Email)); // Thêm lệnh cập nhật
            ShowBillCommand = new RelayCommand<TableFood>((table) => true, (table) => ShowBill(table));
            AddFoodCommand = new RelayCommand<object>((p) => CanAddFood(), (p) => AddFoodToBill());
            CheckoutBillCommand = new RelayCommand<object>((p) => CanCheckout(), (p) => CheckoutBill());





            LoadFoodCategories();
            if (ListFoodCategory.Any())
            {
                SelectedFoodCategory = ListFoodCategory.First();
            }
        }
        private bool CanAddFood()
        {
            return SelectedFood != null && SelectedTable != null;
        }
        private bool CanCheckout()
        {
            return SelectedTable != null;
        }

        private void AddFoodToBill()
        {
            using (var context = new RestaurantManagementContext())
            {
                // Kiểm tra xem table đã có bill chưa
                var bill = context.Bills
                                  .Include(b => b.BillInfos)
                                  .FirstOrDefault(b => b.IdTable == SelectedTable.Tableid && b.Status == 0);

                if (bill == null)
                {
                    // Nếu chưa có bill, tạo mới
                    bill = new Bill
                    {
                        IdTable = SelectedTable.Tableid,
                        DateCheckIn = DateOnly.FromDateTime(DateTime.Now),
                        Status = 0 // chưa tt
                    };

                    context.Bills.Add(bill);
                    context.SaveChanges();

                    // Cập nhật trạng thái của bàn thành "Có người"
                    var tableToUpdate = context.TableFoods.FirstOrDefault(t => t.Tableid == SelectedTable.Tableid);
                    if (tableToUpdate != null)
                    {
                        tableToUpdate.Status = "Có người";
                        context.SaveChanges(); // Lưu lại trạng thái của bàn
                    }
                }


                // Tạo hoặc cập nhật BillInfo cho món được chọn
                var billInfo = bill.BillInfos.FirstOrDefault(bi => bi.IdFood == SelectedFood.Id);
                if (billInfo == null)
                {
                    // Nếu chưa có món này trong bill, thêm mới
                    billInfo = new BillInfo
                    {
                        IdBill = bill.Id,
                        IdFood = SelectedFood.Id,
                        Count = Quantity // Số lượng mặc định
                    };

                    context.BillInfos.Add(billInfo);
                }
                else
                {
                    // Nếu đã có món này, chỉ cập nhật số lượng
                    billInfo.Count += Quantity;
                    context.BillInfos.Update(billInfo);
                }

                // Lưu thay đổi vào cơ sở dữ liệu
                context.SaveChanges();

                RefreshTables();
                UpdateTableCounts();

                // Hiển thị lại bill
                ShowBill(SelectedTable);
            }
        }

        private void CheckoutBill()
        {
            if (SelectedTable == null)
            {
                MessageBox.Show("No table selected for checkout.");
                return;
            }
            // Tính giá cuối cùng dựa trên discount
            decimal discountPercentage = Discount / 100;
            decimal finalTotalPrice = TotalPrice - (TotalPrice * discountPercentage);


            // Thông báo xác nhận thanh toán
            var confirmResult = MessageBox.Show($"Total bill for table {SelectedTable.Tablename}:\nTotal price: {TotalPrice}\nDiscount: {Discount}%\nFinal total price: {finalTotalPrice}", "Confirm Checkout", MessageBoxButton.YesNo);
            if (confirmResult != MessageBoxResult.Yes)
            {
                return;
            }

            using (var context = new RestaurantManagementContext())
            {
                // Lấy bill hiện tại của bàn
                var bill = context.Bills
                                  .Include(b => b.BillInfos)
                                  .FirstOrDefault(b => b.IdTable == SelectedTable.Tableid && b.Status == 0);

                if (bill == null)
                {
                    MessageBox.Show("No active bill found for this table.");
                    return;
                }


                // Cập nhật trạng thái của bill thành "1" (đã thanh toán)
                bill.Status = 1;
                bill.DateCheckOut = DateOnly.FromDateTime(DateTime.Now);
                context.SaveChanges();

                // Cập nhật trạng thái của bàn thành "Trống"
                var tableToUpdate = context.TableFoods.FirstOrDefault(t => t.Tableid == SelectedTable.Tableid);
                if (tableToUpdate != null)
                {
                    tableToUpdate.Status = "Trống";
                    context.SaveChanges();
                }
                // Hiển thị thông báo thành công
                MessageBox.Show("Checkout successful!");
                // Làm mới danh sách bàn để cập nhật trạng thái
                RefreshTables();

                UpdateTableCounts();

                // Xóa danh sách bill của bàn hiện tại
                SelectedTableBill = new ObservableCollection<BillItem>();
                TotalPrice = 0;
            }
        }

        private void RefreshTables()
        {
            using (var context = new RestaurantManagementContext())
            {
                var updatedTables = context.TableFoods.ToList();
                ListTables.Clear();
                foreach (var table in updatedTables)
                {
                    ListTables.Add(table);
                }
            }
        }



        private void ShowBill(TableFood table)
        {
            SelectedTable = table;
            using (var context = new RestaurantManagementContext())
            {
                var bill = context.Bills
                                .Include(b => b.BillInfos)
                                .ThenInclude(bi => bi.IdFoodNavigation)
                                .FirstOrDefault(b => b.IdTable == table.Tableid && b.Status == 0);

                if (bill != null)
                {
                    var billItems = bill.BillInfos.Select(bi => new BillItem
                    {
                        FoodName = bi.IdFoodNavigation.Name,
                        Price = bi.IdFoodNavigation.Price,
                        Quantity = bi.Count,
                        Total = bi.IdFoodNavigation.Price * bi.Count
                    }).ToList();

                    SelectedTableBill = new ObservableCollection<BillItem>(billItems);
                    TotalPrice = (decimal)billItems.Sum(bi => bi.Total);
                }
                else
                {
                    SelectedTableBill = new ObservableCollection<BillItem>();
                    TotalPrice = 0;
                }
            }
        }

        void LoadTablesFood()
        {
            ListTables = new ObservableCollection<TableFood>(DataProvider.Ins.DB.TableFoods.ToList());
        }


        private void LoadFoodCategories()
        {
            ListFoodCategory = new ObservableCollection<FoodCategory>(DataProvider.Ins.DB.FoodCategories.ToList());
        }

        private void LoadFoodsByCategory()
        {
            if (SelectedFoodCategory != null)
            {
                ListFoods = new ObservableCollection<Food>(DataProvider.Ins.DB.Foods.Where(f => f.IdCategory == SelectedFoodCategory.IdCategory).ToList());
                if (ListFoods.Any())
                {
                    SelectedFood = ListFoods.First();
                }
            }
            else
            {
                ListFoods = new ObservableCollection<Food>();
            }
        }
        private void UpdateTableCounts()
        {
            if (ListTables == null)
            {
                CountEmptyTables = 0;
                CountOccupiedTables = 0;
                CountTotalTables = 0;
            }
            else
            {
                CountEmptyTables = ListTables.Count(t => t.Status == "Trống");
                CountOccupiedTables = ListTables.Count(t => t.Status == "Có người");
                CountTotalTables = ListTables.Count;
            }
        }
        public void LoggedInUser(User user)
        {
            LoggedInUserName = user.Username;
            FullName = user.Fullname;
            Address = user.Address;
            Email = user.Email;
            UserRole = user.Role.RoleName; // Assuming RoleName is a property of the Role class
        }
        private void OnLoginSuccess(User user)
        {
            CurrentUser = user;
            FullName = user.Fullname;
            Address = user.Address;
            Email = user.Email;
        }
        private void UpdateUserInfo(string fullName, string address, string email)
        {
            if (CurrentUser == null)
                return;

            using (var context = new RestaurantManagementContext())
            {
                // Kiểm tra xem email đã tồn tại chưa, nhưng bỏ qua email của người dùng hiện tại
                var existingEmail = context.Users.FirstOrDefault(u => u.Email == email && u.UserId != CurrentUser.UserId);

                if (existingEmail != null)
                {
                    MessageBox.Show("Email already exists. Please use a different email.");
                    return;
                }

                // Lấy thông tin người dùng hiện tại để cập nhật
                var userToUpdate = context.Users.FirstOrDefault(u => u.UserId == CurrentUser.UserId);

                if (userToUpdate != null)
                {
                    userToUpdate.Fullname = fullName;
                    userToUpdate.Address = address;
                    userToUpdate.Email = email;

                    context.SaveChanges();
                    MessageBox.Show("Update information successful!!!");
                    CurrentUser = userToUpdate; // Cập nhật thông tin người dùng hiện tại
                }
            }
        }

    }
}