using System;
using System.Collections.Generic;

namespace bestelplatform.Data.bestelplatform;

public partial class Roltoewijzing
{
    public int GebruikerId { get; set; }

    public int? RolId { get; set; }

    public virtual Gebruiker Gebruiker { get; set; } = null!;
    public virtual Rollen Rollen { get; set; } = null!;
}
