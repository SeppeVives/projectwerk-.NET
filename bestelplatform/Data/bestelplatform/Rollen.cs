using System;
using System.Collections.Generic;

namespace bestelplatform.Data.bestelplatform;

public partial class Rollen
{
    public int Id { get; set; }

    public string Naam { get; set; } = null!;

    public virtual ICollection<Gebruiker> Gebruikers { get; set; } = new List<Gebruiker>();
}
