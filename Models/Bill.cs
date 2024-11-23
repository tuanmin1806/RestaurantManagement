using System;
using System.Collections.Generic;

namespace ProjectPRN212.Models;

public partial class Bill
{
    public int Id { get; set; }

    public DateOnly DateCheckIn { get; set; }

    public DateOnly? DateCheckOut { get; set; }

    public int IdTable { get; set; }

    public int Status { get; set; }

    public decimal? Discount { get; set; }


    public virtual ICollection<BillInfo> BillInfos { get; set; } = new List<BillInfo>();

    public virtual TableFood IdTableNavigation { get; set; } = null!;
}
