using System;
using System.Collections.Generic;

namespace bestelplatform.bestelplatform;

public partial class Tafel
{
    public int Id { get; set; }

    public int Nummer { get; set; }

    public virtual ICollection<Tafeltoewijzingen> Tafeltoewijzingens { get; set; } = new List<Tafeltoewijzingen>();
}
