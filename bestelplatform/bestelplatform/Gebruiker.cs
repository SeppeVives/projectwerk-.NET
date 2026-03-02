using System;
using System.Collections.Generic;

namespace bestelplatform.bestelplatform;

public partial class Gebruiker
{
    public int Id { get; set; }

    public string Naam { get; set; } = null!;

    public string WachtwoordHash { get; set; } = null!;

    public string UniekeCode { get; set; } = null!;

    public bool? Geactiveerd { get; set; }

    public virtual Bezoeker? Bezoeker { get; set; }

    public virtual Medewerker? Medewerker { get; set; }
}
