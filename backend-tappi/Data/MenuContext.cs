using backend_tappi.VenueModel;
using backend_tappi.MenuModel;
using backend_tappi.BeerModel;
using Microsoft.EntityFrameworkCore;

namespace backend_tappi.Data
{
    public class MenuContext : DbContext
    {
        public MenuContext(DbContextOptions<MenuContext> options) : base(options)
        { }

        public DbSet<ParsedVenue> Venues { get; set; }
        public DbSet<ParsedBeer> Beers { get; set; }
        public DbSet<Menu> Menus { get; set; }

        // protected override void OnModelCreating(ModelBuilder modelBuilder)
        // {
        //     modelBuilder.Entity<Menu>().ToTable("Menu");
        //     modelBuilder.Entity<ParsedVenue>().ToTable("Venue");
        //     modelBuilder.Entity<ParsedBeer>().ToTable("Beer");
        // }
    }
}
