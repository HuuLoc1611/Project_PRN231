using System;
using System.Collections.Generic;

namespace CallAPI.Models;

public partial class Account
{
    public int Id { get; set; }

    public string? Username { get; set; }

    public string? Password { get; set; }

    public int? Role { get; set; }

    public string? Email { get; set; }

    public string? FullName { get; set; }

    public bool? IsMember { get; set; }

    public string? Image { get; set; }

    public virtual ICollection<Blog> Blogs { get; set; } = new List<Blog>();

    public virtual ICollection<CommentBlog> CommentBlogs { get; set; } = new List<CommentBlog>();

    public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();
}
