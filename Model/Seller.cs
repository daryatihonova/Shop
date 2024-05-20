using System;
using System.Collections.Generic;

namespace Shop.Model;

public partial class Seller
{
    public int SellerId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? Patronymic { get; set; }

    public string Login { get; set; } = null!;

    public int Password { get; set; }

    public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();
}
