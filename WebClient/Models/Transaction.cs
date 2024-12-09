using System;
using System.Collections.Generic;

namespace WebClient.Models;

public partial class Transaction
{
    public int Id { get; set; }

    public int? AccountId { get; set; }

    public DateTime? Date { get; set; }

    public double? Price { get; set; }

    public virtual Account? Account { get; set; }
}
