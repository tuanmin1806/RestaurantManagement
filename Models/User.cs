using System;
using System.Collections.Generic;

namespace ProjectPRN212.Models;

public partial class User
{
    public int UserId { get; set; }

    public string? Fullname { get; set; }

    public string? Username { get; set; }

    public string? PassWord { get; set; }

    public string? Address { get; set; }

    public string? Email { get; set; }

    public int? Status { get; set; }

    public int? RoleId { get; set; }

    public virtual ICollection<BillInfo> BillInfos { get; set; } = new List<BillInfo>();

    public virtual ICollection<Food> Foods { get; set; } = new List<Food>();

    public virtual Role? Role { get; set; }
}
