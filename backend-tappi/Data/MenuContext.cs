using backend_tappi.VenueModel;
using backend_tappi.MenuModel;
using backend_tappi.BeerModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace backend_tappi.Data
{
    public class MenuContext : DbContext
    {
        public MenuContext(DbContextOptions<MenuContext> options) : base(options)
        { }

        public DbSet<ParsedVenue> Venues { get; set; }
        public DbSet<ParsedBeer> Beers { get; set; }
        public DbSet<Menu> Menus { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Menu>()
                .HasKey(m => new { m.VenueID, m.BeerID });
            modelBuilder.Entity<Menu>()
                .HasOne(m => m.ParsedVenue)
                .WithMany(v => v.Menus)
                .HasForeignKey(m => m.VenueID);
            modelBuilder.Entity<Menu>()
                .HasOne(m => m.ParsedBeer)
                .WithMany(b => b.Menus)
                .HasForeignKey(m => m.BeerID);
            var allEntities = modelBuilder.Model.GetEntityTypes();

            foreach (var entity in allEntities)
            {
                entity.AddProperty("CreatedDate", typeof(DateTime));
                entity.AddProperty("UpdatedDate", typeof(DateTime));
            }
        }

        public override int SaveChanges()
        {
            var entries = ChangeTracker
                .Entries()
                .Where(e =>
                        e.State == EntityState.Added
                        || e.State == EntityState.Modified);

            foreach (var entityEntry in entries)
            {
                entityEntry.Property("UpdatedDate").CurrentValue = DateTime.Now;

                if (entityEntry.State == EntityState.Added)
                {
                    entityEntry.Property("CreatedDate").CurrentValue = DateTime.Now;
                }
            }

            return base.SaveChanges();
        }
    }
}
