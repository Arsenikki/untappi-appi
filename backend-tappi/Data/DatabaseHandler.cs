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
        public static async Task<List<ParsedVenue>> GetVenuesFromDB(MenuContext context, double lat, double lng)
        {
            List<ParsedVenue> venues = context.Venues
                .Where(v => Math.Sqrt(Math.Pow((v.Lat - lat),2) + Math.Pow((v.Lng - lng),2)) <= 0.1)
                .Select(v => v)
                .ToList();

            return venues;
        }

        public static async Task<object> InsertVenuesToDatabase(MenuContext context, List<ParsedVenue> venues)
        {
            context.AddRange(venues);
            context.SaveChanges();
            return null;
        }

        public static async Task<List<ParsedBeer>> GetBeersFromDbForVenue(MenuContext context, int selectedVenueId)
        {
            List<ParsedBeer> beersFromMenu = context.Menus
                    .Where(v => v.ParsedVenue.VenueID == selectedVenueId)
                    .Select(beers => beers.ParsedBeer)
                    .ToList();

            return beersFromMenu;
        }

        public static async Task<object> InsertBeersToDatabase(MenuContext context, int selectedVenueId, List<ParsedBeer> beersToBeAdded)
        {
            // TODO: probably better way to do this..
            List<ParsedVenue> venue = context.Venues
                .Where(v => v.VenueID == selectedVenueId)
                .ToList();

            // Add menu only if venue found from DB with venueID
            if(venue.Count != 0)
            {
                ParsedVenue selectedVenue = venue[0];

                List<Menu> menus = new List<Menu> { };
                foreach (var beer in beersToBeAdded)
                {
                    menus.Add(new Menu { ParsedVenue = selectedVenue, ParsedBeer = beer });
                }
                context.AddRange(menus);
                context.SaveChanges();
            }
            return null;
        }
    }
}
