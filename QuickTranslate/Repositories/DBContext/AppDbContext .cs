using Microsoft.EntityFrameworkCore;
using QuickTranslate.Entities;

namespace QuickTranslate.Repositories.DBContext
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Language> Languages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Language>()
                .HasKey(l => l.Id);

            modelBuilder.Entity<Language>()
                .HasIndex(l => l.LanguageCode)
                .IsUnique();
        }
    }
}