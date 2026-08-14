using System;
using System.Collections.Generic;

namespace bestelplatform.bestelplatform;

public partial class Bestellijnen
{
    public int BestellingId { get; set; }

    public int ProductId { get; set; }

    public int Hoeveelheid { get; set; }
}
