using System;
using System.Collections.Generic;

namespace ProjectPRN212.Models;

public partial class TableFood
{
    public int Tableid { get; set; }

    public string? Tablename { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<Bill> Bills { get; set; } = new List<Bill>();
}
