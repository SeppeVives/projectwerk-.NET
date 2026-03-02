using System;
using System.Collections.Generic;

namespace bestelplatform.bestelplatform;

public partial class Bestellijnen
{
    public int BestellingId { get; set; }

    public int ProductId { get; set; }

    public int Hoeveelheid { get; set; }

    public virtual Bestellingen Bestelling { get; set; } = null!;

    public virtual Producten Product { get; set; } = null!;
}
