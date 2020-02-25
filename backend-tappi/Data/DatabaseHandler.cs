using backend_tappi.BeerModel;
using backend_tappi.VenueModel;
using backend_tappi.MenuModel;
using System;
using System.Collections.Generic;

namespace backend_tappi.Data
{
    public class DatabaseHandler
    {
        private static List<ParsedVenue> _venues;

        public DatabaseHandler()
        {
            _venues = new List<ParsedVenue>();
        }

        public static void InsertVenuesToDatabase(List<ParsedVenue> venues)
        {
            // This adds to local memory, which is accessed by InsertBeersToDatabase() to get full venue info.
            _venues.AddRange(venues); // TODO: currently duplicates!!
        }

        public static void InsertBeersToDatabase(MenuContext context, int selectedVenueId, List<ParsedBeer> beers)
        {
            context.Database.EnsureCreated();

            ParsedVenue selectedVenue = _venues.Find(x => x.VenueID == selectedVenueId);

            // TODO: currently only works with 3
            context.AddRange(
                new Menu { ParsedVenue = selectedVenue, ParsedBeer = beers[0] },
                new Menu { ParsedVenue = selectedVenue, ParsedBeer = beers[1] },
                new Menu { ParsedVenue = selectedVenue, ParsedBeer = beers[2] }
            );
            context.SaveChanges();
        }

        
    }
}
