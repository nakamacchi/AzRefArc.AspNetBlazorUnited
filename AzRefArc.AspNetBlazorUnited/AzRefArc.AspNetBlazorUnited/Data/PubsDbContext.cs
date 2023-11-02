using AzRefArc.AspNetBlazorUnited.Client.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AzRefArc.AspNetBlazorUnited.Data
{
    public partial class PubsDbContext : DbContext
    {
        public PubsDbContext(DbContextOptions<PubsDbContext> options) : base(options)
        {
        }

        public DbSet<Author> Authors { get; set; } = null!;
        public DbSet<Title> Titles { get; set; } = null!;
        public DbSet<Publisher> Publishers { get; set; } = null!;
        public DbSet<Store> Stores { get; set; } = null!;
        public DbSet<Sale> Sales { get; set; } = null!;
        public DbSet<TitleAuthor> TitleAuthors { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 複合 PK の指定 (Fluent API でしか定義できない)
            modelBuilder.Entity<Sale>().HasKey(s => new { s.StoreId, s.OrderNumber, s.TitleId });
            modelBuilder.Entity<TitleAuthor>().HasKey(ta => new { ta.AuthorId, ta.TitleId });
        }
    }

}

