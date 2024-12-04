using System;
using System.Collections.Generic;

namespace WebClient.Models;

public partial class Blog
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public string? Image { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? CreatorId { get; set; }

    public bool? Status { get; set; }

    public bool? IsComment { get; set; }

    public string? Content { get; set; }

    public virtual ICollection<CommentBlog> CommentBlogs { get; set; } = new List<CommentBlog>();

    public virtual Account? Creator { get; set; }

    public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();

    public virtual ICollection<TagBlog> TagBlogs { get; set; } = new List<TagBlog>();
}
