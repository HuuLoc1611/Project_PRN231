using System;
using System.Collections.Generic;

namespace WebClient.Models;

public partial class CommentBlog
{
    public int Id { get; set; }

    public int? BlogId { get; set; }

    public int? ParentId { get; set; }

    public int? AccountId { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? Content { get; set; }

    public virtual Account? Account { get; set; }

    public virtual Blog? Blog { get; set; }
}
