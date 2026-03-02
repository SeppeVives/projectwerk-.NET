using System;
using System.Collections.Generic;

namespace bestelplatform.bestelplatform;

public partial class Bezoeker
{
    public int GebruikerId { get; set; }

    public virtual ICollection<Bestellingen> Bestellingens { get; set; } = new List<Bestellingen>();

    public virtual Gebruiker Gebruiker { get; set; } = null!;

    public virtual ICollection<Tafeltoewijzingen> Tafeltoewijzingens { get; set; } = new List<Tafeltoewijzingen>();
}
