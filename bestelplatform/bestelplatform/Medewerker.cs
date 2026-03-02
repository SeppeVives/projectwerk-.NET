using System;
using System.Collections.Generic;

namespace bestelplatform.bestelplatform;

public partial class Medewerker
{
    public int GebruikerId { get; set; }

    public virtual Gebruiker Gebruiker { get; set; } = null!;

    public virtual ICollection<Rollen> Rols { get; set; } = new List<Rollen>();
}
