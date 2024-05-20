using System;
using System.Collections.Generic;

namespace Shop.Model;

public partial class Sale
{
    public int SaleId { get; set; }

    public int AmountOfProducts { get; set; }

    public decimal Cost { get; set; }

    public DateTime Date { get; set; }

    public int ProductId { get; set; }

    public int SellerId { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual Seller Seller { get; set; } = null!;
}
