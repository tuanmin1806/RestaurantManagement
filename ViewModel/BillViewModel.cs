using Microsoft.EntityFrameworkCore;
using ProjectPRN212.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectPRN212.ViewModel
{
    class BillViewModel : BaseViewModel
    {
        private ObservableCollection<BillDisplayModel> _ListBills;
        public ObservableCollection<BillDisplayModel> ListBills
        {
            get => _ListBills;
            set { _ListBills = value; OnPropertyChanged(); }
        }
        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set { _searchText = value; OnPropertyChanged(); LoadBills(); }
        }

        private DateTime? _startDate;
        public DateTime? StartDate
        {
            get => _startDate;
            set { _startDate = value; OnPropertyChanged(); LoadBills(); }
        }

        private DateTime? _endDate;
        public DateTime? EndDate
        {
            get => _endDate;
            set { _endDate = value; OnPropertyChanged(); LoadBills(); }
        }

        public BillViewModel()
        {
            LoadBills();
        }

        private void LoadBills()
        {
            using (var context = new RestaurantManagementContext())
            {
                var query = context.Bills
                                   .Include(b => b.IdTableNavigation) // Load TableFood for TableName
                                   .Include(b => b.BillInfos) // Load BillInfos for TotalPrice calculation
                                   .ThenInclude(bi => bi.IdFoodNavigation) // Load Food for Price calculation
                                   .Where(b => b.Status == 1)
                                   .AsEnumerable();

                if (StartDate.HasValue)
                {
                    query = query.Where(b => b.DateCheckIn.ToDateTime(new TimeOnly(0, 0)) >= StartDate.Value);
                }

                if (EndDate.HasValue)
                {
                    query = query.Where(b => b.DateCheckOut?.ToDateTime(new TimeOnly(23, 59)) <= EndDate.Value);
                }
                if (!string.IsNullOrEmpty(SearchText))
                {
                    query = query.Where(b => b.IdTableNavigation.Tablename.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
                }

                var bills = query.ToList();

                // Create BillDisplayModel and calculate TotalPrice for each Bill
                ListBills = new ObservableCollection<BillDisplayModel>(
                    bills.Select(bill => new BillDisplayModel
                    {
                        Id = bill.Id,
                        TableName = bill.IdTableNavigation.Tablename,
                        TotalPrice = (decimal)bill.BillInfos.Sum(bi => bi.IdFoodNavigation.Price * bi.Count),
                        DateCheckIn = bill.DateCheckIn,
                        DateCheckOut = bill.DateCheckOut
                    }));
            }
        }

        public class BillDisplayModel
        {
            public int Id { get; set; }
            public string TableName { get; set; }
            public decimal TotalPrice { get; set; }
            public DateOnly DateCheckIn { get; set; }
            public DateOnly? DateCheckOut { get; set; }
        }
    }
}
