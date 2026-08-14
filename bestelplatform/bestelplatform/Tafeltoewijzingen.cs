using System;
using System.Collections.Generic;

namespace bestelplatform.bestelplatform;

public partial class Tafeltoewijzingen
{
    public int GebruikerId { get; set; }

    public int TafelId { get; set; }

    public DateTime TijdstipToegewezen { get; set; }
}
