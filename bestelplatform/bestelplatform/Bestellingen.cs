using System;
using System.Collections.Generic;

namespace bestelplatform.bestelplatform;

public partial class Bestellingen
{
    public int Id { get; set; }

    public int? GebruikerId { get; set; }

    public DateTime TijdstipBesteld { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<Bestellijnen> Bestellijnens { get; set; } = new List<Bestellijnen>();

    public virtual Bezoeker? Gebruiker { get; set; }
}
