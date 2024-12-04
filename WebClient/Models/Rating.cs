using System;
using System.Collections.Generic;

namespace WebClient.Models;

public partial class Rating
{
    public int Id { get; set; }

    public int? AccountId { get; set; }

    public int? BlogId { get; set; }

    public DateTime? CreatedDate { get; set; }

    public double? Quality { get; set; }

    public virtual Account? Account { get; set; }

    public virtual Blog? Blog { get; set; }
}
