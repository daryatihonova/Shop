using System;
using System.Collections.Generic;

namespace Shop.Model;

public partial class Product
{
    public int ProductId { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string UnitOfMeasurement { get; set; } = null!;

    public decimal PriceUnit { get; set; }

    public int Quantity { get; set; }

    public DateTime DateOfLastDelivery { get; set; }

    public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();

    public virtual ICollection<Storage> Storages { get; set; } = new List<Storage>();
}
