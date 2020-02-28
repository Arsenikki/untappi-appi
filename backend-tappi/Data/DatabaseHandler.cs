using backend_tappi.BeerModel;
using backend_tappi.VenueModel;
using backend_tappi.MenuModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend_tappi.Data
{
    public class DatabaseHandler
    {
        private static List<ParsedVenue> _venues;
        private static MenuContext _context;

        public DatabaseHandler()
        {
            _venues = new List<ParsedVenue>();
        }

        public static void InitializeDB(MenuContext context)
        {
            _context = context;
        }

        public static void InsertVenuesToDatabase(List<ParsedVenue> venues)
        {
            // This adds to local memory, which is accessed by InsertBeersToDatabase() to get full venue info.
            _venues.AddRange(venues); // TODO: currently duplicates!!
        }

        public static Task<List<ParsedBeer>> ReadBeersFromDB(int selectedVenueId)
        {
            List<Menu> MenuFromDB = _context.Menus.Where(v => v.VenueId == selectedVenueId).ToList();

            // return as List<ParsedBeer>
            return null;
        }

        public static async Task<object> InsertBeersToDatabase(int selectedVenueId, List<ParsedBeer> beers)
        {
            _context.Database.EnsureCreated();

            ParsedVenue selectedVenue = _venues.Find(x => x.VenueID == selectedVenueId);

            // TODO: currently only works with 3
            _context.AddRange(
                new Menu { ParsedVenue = selectedVenue, ParsedBeer = beers[0] },
                new Menu { ParsedVenue = selectedVenue, ParsedBeer = beers[1] },
                new Menu { ParsedVenue = selectedVenue, ParsedBeer = beers[2] }
            );
            _context.SaveChanges();

            return null;
        }

        
    }
}
