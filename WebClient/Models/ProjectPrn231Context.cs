using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WebClient.Models;

public partial class ProjectPrn231Context : DbContext
{
    public ProjectPrn231Context()
    {
    }

    public ProjectPrn231Context(DbContextOptions<ProjectPrn231Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Blog> Blogs { get; set; }

    public virtual DbSet<CommentBlog> CommentBlogs { get; set; }

    public virtual DbSet<Rating> Ratings { get; set; }

    public virtual DbSet<Tag> Tags { get; set; }

    public virtual DbSet<TagBlog> TagBlogs { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        optionsBuilder.UseSqlServer(config.GetConnectionString("MyCnn"));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.ToTable("Account");

            entity.Property(e => e.FullName).HasColumnName("Full_Name");
            entity.Property(e => e.IsMember).HasColumnName("Is_Member");
        });

        modelBuilder.Entity<Blog>(entity =>
        {
            entity.ToTable("Blog");

            entity.Property(e => e.CreatedDate)
                .HasColumnType("date")
                .HasColumnName("Created_Date");
            entity.Property(e => e.CreatorId).HasColumnName("Creator_Id");
            entity.Property(e => e.IsComment).HasColumnName("isComment");

            entity.HasOne(d => d.Creator).WithMany(p => p.Blogs)
                .HasForeignKey(d => d.CreatorId)
                .HasConstraintName("FK_Blog_Account");
        });

        modelBuilder.Entity<CommentBlog>(entity =>
        {
            entity.ToTable("CommentBlog");

            entity.Property(e => e.AccountId).HasColumnName("Account_id");
            entity.Property(e => e.BlogId).HasColumnName("Blog_id");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("date")
                .HasColumnName("Created_Date");
            entity.Property(e => e.ParentId).HasColumnName("Parent_id");

            entity.HasOne(d => d.Account).WithMany(p => p.CommentBlogs)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("FK_CommentBlog_Account");

            entity.HasOne(d => d.Blog).WithMany(p => p.CommentBlogs)
                .HasForeignKey(d => d.BlogId)
                .HasConstraintName("FK_CommentBlog_Blog");
        });

        modelBuilder.Entity<Rating>(entity =>
        {
            entity.ToTable("Rating");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AccountId).HasColumnName("account_id");
            entity.Property(e => e.BlogId).HasColumnName("blog_id");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("date")
                .HasColumnName("created_date");
            entity.Property(e => e.Quality).HasColumnName("quality");

            entity.HasOne(d => d.Account).WithMany(p => p.Ratings)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("FK_Rating_Account");

            entity.HasOne(d => d.Blog).WithMany(p => p.Ratings)
                .HasForeignKey(d => d.BlogId)
                .HasConstraintName("FK_Rating_Blog");
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.ToTable("Tag");
        });

        modelBuilder.Entity<TagBlog>(entity =>
        {
            entity.ToTable("TagBlog");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BlogId).HasColumnName("Blog_Id");
            entity.Property(e => e.TagId).HasColumnName("Tag_Id");

            entity.HasOne(d => d.Blog).WithMany(p => p.TagBlogs)
                .HasForeignKey(d => d.BlogId)
                .HasConstraintName("FK_TagBlog_Blog");

            entity.HasOne(d => d.Tag).WithMany(p => p.TagBlogs)
                .HasForeignKey(d => d.TagId)
                .HasConstraintName("FK_TagBlog_Tag");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.ToTable("Transaction");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.AccountId).HasColumnName("Account_Id");
            entity.Property(e => e.Date).HasColumnType("date");

            entity.HasOne(d => d.Account).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("FK_Transaction_Account");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
