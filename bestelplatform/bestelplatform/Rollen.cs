using System;
using System.Collections.Generic;

namespace bestelplatform.bestelplatform;

public partial class Rollen
{
    public int Id { get; set; }

    public string Naam { get; set; } = null!;

    public virtual ICollection<Medewerker> Gebruikers { get; set; } = new List<Medewerker>();
}
