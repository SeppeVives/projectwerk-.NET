using System;
using System.Collections.Generic;

namespace bestelplatform.bestelplatform;

public partial class Producten
{
    public int Id { get; set; }

    public virtual ICollection<Bestellijnen> Bestellijnens { get; set; } = new List<Bestellijnen>();

    public virtual ICollection<Productdetail> Productdetails { get; set; } = new List<Productdetail>();
}
