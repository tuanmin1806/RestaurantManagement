using System;
using System.Collections.Generic;

namespace ProjectPRN212.Models;

public partial class FoodCategory
{
    public int IdCategory { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Food> Foods { get; set; } = new List<Food>();
}
