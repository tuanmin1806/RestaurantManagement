using ProjectPRN212.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ProjectPRN212.ViewModel
{

    public class TableFoodViewModel : BaseViewModel
    {
        private ObservableCollection<TableFood> _ListTables;
        public ObservableCollection<TableFood> ListTables { get => _ListTables; set { _ListTables = value; OnPropertyChanged(); } }

        public ICommand CreateCommand { get; set; }

        private string _tableName;
        public string TableName
        {
            get => _tableName;
            set { _tableName = value; OnPropertyChanged(); }
        }

        private TableFood _SelectedItem;

        public TableFood SelectedItem
        {
            get => _SelectedItem;
            set
            {
                _SelectedItem = value;
                OnPropertyChanged();
                if (SelectedItem != null)
                {
                    TableName = SelectedItem.Tablename;
                }
            }
        }
        private bool _isTableNameExistsMessageShown = false; // Biến để kiểm tra xem đã hiển thị MessageBox lỗi chưa
        public TableFoodViewModel()
        {
            ListTables = new ObservableCollection<TableFood>(DataProvider.Ins.DB.TableFoods);
            CreateCommand = new RelayCommand<object>((p) =>
            {
                // Kiểm tra nếu TableName rỗng hoặc đã tồn tại trong cơ sở dữ liệu
                if (string.IsNullOrEmpty(TableName))
                {
                    return false;
                }

                var existingTable = DataProvider.Ins.DB.TableFoods.FirstOrDefault(t => t.Tablename == TableName);
                if (existingTable != null)
                {
                    if (!_isTableNameExistsMessageShown)
                    {
                        MessageBox.Show("Table Name already exists. Please choose a different name.");
                        _isTableNameExistsMessageShown = true; // Đánh dấu đã hiển thị MessageBox lỗi
                    }
                    return false;
                }

                return true;
            }, (p) =>
            {

                var table = new TableFood()
                {
                    Tablename = TableName,
                    Status = "Trống",

                };
                DataProvider.Ins.DB.TableFoods.Add(table);
                MessageBox.Show("Add Table Successfully");
                DataProvider.Ins.DB.SaveChanges();
                ListTables.Add(table);
            }
      );
        }
    }
}
