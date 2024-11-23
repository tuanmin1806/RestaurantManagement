using Microsoft.EntityFrameworkCore;
using ProjectPRN212.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using ProjectPRN212.Services;

namespace ProjectPRN212.ViewModel
{
    class FoodViewModel : BaseViewModel
    
    {
        private ObservableCollection<Food> _ListFoods;
        public ObservableCollection<Food> ListFoods { get => _ListFoods; set { _ListFoods = value; OnPropertyChanged(); } }

        private ObservableCollection<FoodCategory> _ListFoodCategories;
        public ObservableCollection<FoodCategory> ListFoodCategories { get => _ListFoodCategories; set { _ListFoodCategories = value; OnPropertyChanged(); } }

        public ICommand CreateCommand { get; set; }
        public ICommand UpdateCommand { get; set; }

        private string _foodName;
        public string FoodName
        {
            get => _foodName;
            set { _foodName = value; OnPropertyChanged(); }
        }

        private int _foodCategoryId;
        public int FoodCategoryId
        {
            get => _foodCategoryId;
            set { _foodCategoryId = value; OnPropertyChanged(); }
        }

        private double _foodPrice;
        public double FoodPrice
        {
            get => _foodPrice;
            set { _foodPrice = value; OnPropertyChanged(); }
        }

        private FoodCategory _selectedFoodCategory;
        public FoodCategory SelectedFoodCategory
        {
            get => _selectedFoodCategory;
            set
            {
                _selectedFoodCategory = value;
                OnPropertyChanged();
            }
        }

        private Food _SelectedItem;

        public Food SelectedItem
        {
            get => _SelectedItem;
            set
            {
                _SelectedItem = value;
                OnPropertyChanged();
                if (SelectedItem != null)
                {
                    FoodName = SelectedItem.Name;
                    FoodPrice = SelectedItem.Price;
                    SelectedFoodCategory = SelectedItem.IdCategoryNavigation;
                }
            }
        }

        private FoodCategory _selectedCategoryForFilter;
        public FoodCategory SelectedCategoryForFilter
        {
            get => _selectedCategoryForFilter;
            set
            {
                _selectedCategoryForFilter = value;
                OnPropertyChanged();
                FilterFoodsByCategory();
            }
        }

        public FoodViewModel() 
        {
            SelectedCategoryForFilter = new FoodCategory();
            LoadFoods();
            CreateCommand = new RelayCommand<object>((p) =>
            {
                if (string.IsNullOrEmpty(FoodName) || FoodPrice < 0)
                    return false;
                return true;
            }, (p) =>
            {

                var food = new Food()
                {
                    Name = FoodName,
                    Price = FoodPrice,
                    IdCategory = SelectedFoodCategory.IdCategory,

                };
                DataProvider.Ins.DB.Foods.Add(food);
                MessageBox.Show("Add Food Successfully");
                DataProvider.Ins.DB.SaveChanges();
                ListFoods.Add(food);

                
            }
       );
            UpdateCommand = new RelayCommand<object>((p) =>
            {
                if (SelectedItem == null || string.IsNullOrEmpty(FoodName) || FoodPrice < 0)
                    return false;
                var foodList = DataProvider.Ins.DB.Foods.Where(x => x.Id == SelectedItem.Id);
                if (foodList != null && foodList.Count() != 0)
                    return true;
                return false;
            }, (p) =>
            {
                var EditFood = DataProvider.Ins.DB.Foods.Where(x => x.Id == SelectedItem.Id).FirstOrDefault();
                EditFood.Name = FoodName;
                EditFood.Price = FoodPrice;
                EditFood.IdCategory = SelectedFoodCategory.IdCategory;

                DataProvider.Ins.DB.SaveChanges();
                MessageBox.Show("Food Updated Successfully");
                ListFoods = new ObservableCollection<Food>(DataProvider.Ins.DB.Foods);
            }
          );
        }
        private void LoadFoods()
        {
            ListFoods = new ObservableCollection<Food>(DataProvider.Ins.DB.Foods.ToList());
            ListFoodCategories = new ObservableCollection<FoodCategory>(DataProvider.Ins.DB.FoodCategories);
           
        }

        private void FilterFoodsByCategory()
        {
            if (SelectedCategoryForFilter != null && SelectedCategoryForFilter.Name != "All")
            {
                ListFoods = new ObservableCollection<Food>(DataProvider.Ins.DB.Foods
                    .Include(fc => fc.IdCategoryNavigation)
                    .Where(f => f.IdCategory == SelectedCategoryForFilter.IdCategory)
                    .ToList());
            }
            else
            {
                LoadFoods(); // Load all foods
            }
        }
        public void OnCategorySelectionChanged()
        {
            if (SelectedFoodCategory != null)
            {
                FoodCategoryId = SelectedFoodCategory.IdCategory;
                FilterFoodsByCategory();
            }
        }
    }
}
