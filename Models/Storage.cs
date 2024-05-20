using System;
using System.Collections.Generic;

namespace Shop.Models;

public partial class Storage
{
    public int StorageId { get; set; }

    public int QuantityOfProducts { get; set; }

    public decimal TotalCost { get; set; }

    public DateTime DateDelivery { get; set; }

    public int ProductId { get; set; }

    public virtual Product Product { get; set; } = null!;
}
