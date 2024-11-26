using System;
using System.Collections.Generic;

namespace WebClient.Models;

public partial class TagBlog
{
    public int? TagId { get; set; }

    public int? BlogId { get; set; }

    public int Id { get; set; }

    public virtual Blog? Blog { get; set; }

    public virtual Tag? Tag { get; set; }
}
