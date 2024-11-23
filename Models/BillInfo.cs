using System;
using System.Collections.Generic;

namespace ProjectPRN212.Models;

public partial class BillInfo
{
    public int Id { get; set; }

    public int? IdBill { get; set; }

    public int? IdFood { get; set; }

    public int? UserId { get; set; }

    public int Count { get; set; }

    public virtual Bill? IdBillNavigation { get; set; }

    public virtual Food? IdFoodNavigation { get; set; }

    public virtual User? User { get; set; }
}
