using backend_tappi.BeerModel;
using backend_tappi.VenueModel;
using backend_tappi.MenuModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace backend_tappi.Data
{
    public class DatabaseHandler
    {
        private static List<ParsedVenue> _venues;


        public static void InitializeVenueList()
        {
            _venues = new List<ParsedVenue>();
        }

        public static void InsertVenuesToDatabase(List<ParsedVenue> venues)
        {
            // This adds to local memory, which is accessed by InsertBeersToDatabase() to get full venue info.
            foreach (ParsedVenue venue in venues)
            {
                bool alreadyExists = _venues.Exists(v => v.VenueName == venue.VenueName);
                if (!alreadyExists)
                {
                    _venues.Add(venue);
                }
            }
        }

        public static async Task<List<ParsedBeer>> GetBeersFromDbForVenue(MenuContext context, int selectedVenueId)
        {
            List<ParsedBeer> beersFromMenu = context.Menus
                    .Where(v => v.ParsedVenue.VenueID == selectedVenueId)
                    .Select(beers => beers.ParsedBeer)
                    .ToList();

            // return as List<ParsedBeer>
             
            return beersFromMenu;
        }

        public static async Task<object> InsertBeersToDatabase(MenuContext context, int selectedVenueId, List<ParsedBeer> beers)
        {
            ParsedVenue selectedVenue = _venues.Find(x => x.VenueID == selectedVenueId);
            List<Menu> menus = new List<Menu> {};
            foreach (var beer in beers)
            {
                menus.Add( new Menu { ParsedVenue = selectedVenue, ParsedBeer = beer });
            }
            context.AddRange(menus);
            context.SaveChanges();
            return null;
        }
    }
}
