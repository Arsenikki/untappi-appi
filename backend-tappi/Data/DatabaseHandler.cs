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

        // public DatabaseHandler(MenuContext context)
        // {
        //     _context = context;
        // }

        public static void InitializeDB(MenuContext context)
        {
            _context = context;
            _context.Database.EnsureCreated();

            _venues = new List<ParsedVenue>();

            // context.Venues.Add(new ParsedVenue {
            //     VenueID = 123,
            //     VenueName = "kallionhovi",
            //     Address = "testi",
            //     Category = "pahaa",
            //     Lat = 1,
            //     Lng = 2,
            // });
            // 
            // context.Beers.Add(new ParsedBeer
            // {
            //     BeerID = 123,
            //     BeerName = "3.5€ setti",
            //     Brewery = "kallio-brew",
            //     Country = "SUAMI",
            //     Rating = 1,
            //     Stronkness = 100,
            //     Style = "PAHAA",
            // });
            // 
            // context.SaveChanges();
        }

        public static void InsertVenuesToDatabase(List<ParsedVenue> venues)
        {
            // This adds to local memory, which is accessed by InsertBeersToDatabase() to get full venue info.
            _venues.AddRange(venues); // TODO: currently duplicates!!
        }

        public static Task<List<ParsedBeer>> ReadBeersFromDB(int selectedVenueId)
        {
            List<Menu> MenuFromDB = _context.Menus
                    .Where(v => v.VenueId == selectedVenueId)
                    .ToList();
            // return as List<ParsedBeer>
            return null;
        }

        public static async Task<object> InsertBeersToDatabase(int selectedVenueId, List<ParsedBeer> beers)
        {
            ParsedVenue selectedVenue = _venues.Find(x => x.VenueID == selectedVenueId);

            _context.AddRange(
                new Menu { ParsedVenue = selectedVenue, ParsedBeer = beers[0] },
                new Menu { ParsedVenue = selectedVenue, ParsedBeer = beers[1] },
                new Menu { ParsedVenue = selectedVenue, ParsedBeer = beers[2] }
            );
            _context.SaveChanges();
            // TODO: currently only works with 3
            return null;
        }

        
    }
}
