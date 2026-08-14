using bestelplatform.Models.Enums;
using System;
using System.Collections.Generic;

namespace bestelplatform.Data.bestelplatform;

public partial class Productdetail
{
    public int ProductId { get; set; }

    public DateTime Tijdstip { get; set; }

    public string Naam { get; set; } = null!;

    public float Prijs { get; set; }

    public ProductEnum Producttype { get; set; }

    public virtual Producten Product { get; set; } = null!;
}
