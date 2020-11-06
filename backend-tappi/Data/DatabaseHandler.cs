using backend_tappi.BeerModel;
using backend_tappi.VenueModel;
using backend_tappi.MenuModel;
using backend_tappi.VenueWithBeerModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace backend_tappi.Data
{
    public class DatabaseHandler
    {
        public static async Task<List<VenueWithBeers>> GetAllMenus(MenuContext context)
        {
            List<VenueWithBeers> venuesWithBeers = new List<VenueWithBeers>();

            List<Menu> menus = context.Menus
                .Include(menu => menu.ParsedVenue)
                .Include(menu => menu.ParsedBeer)
                .ToList();

            menus.ForEach(menu => {
                var foundVenueIndex = venuesWithBeers.FindIndex(v => v.VenueID == menu.VenueID);
                if (foundVenueIndex != -1) {
                    venuesWithBeers[foundVenueIndex].ParsedBeer.Add(menu.ParsedBeer);
                }
                else {
                    venuesWithBeers.Add(
                        new VenueWithBeers {
                            VenueID = menu.ParsedVenue.VenueID,
                            VenueName = menu.ParsedVenue.VenueName,
                            Address = menu.ParsedVenue.Address,
                            Category = menu.ParsedVenue.Category,
                            Lat = menu.ParsedVenue.Lat,
                            Lng = menu.ParsedVenue.Lng,
                            ParsedBeer = new List<ParsedBeer>()
                            {
                                menu.ParsedBeer
                            }
                        }
                    );
                };
            });
            return venuesWithBeers;
        }

        public static async Task<List<ParsedVenue>> GetVenuesFromDB(MenuContext context)
        {
            List<ParsedVenue> venues = context.Venues
                .Select(v => v)
                .ToList();

            return venues;
        }

        public static async Task<object> PutVenuesToDatabase(MenuContext context, List<ParsedVenue> venues)
        {
            context.AddRange(venues);
            context.SaveChanges();

            return null;
        }

        public static async Task<List<ParsedBeer>> GetBeersFromDbForVenue(MenuContext context, int selectedVenueId)
        {
            List<ParsedBeer> singleVenueBeers = context.Menus
                    .Where(v => v.ParsedVenue.VenueID == selectedVenueId)
                    .Select(beers => beers.ParsedBeer)
                    .ToList();

            return singleVenueBeers;
        }

        public static void PutBeersToDatabase(MenuContext context, int selectedVenueId, List<ParsedBeer> beersToBeAdded)
        {
            // TODO: probably better way to do this..
            List<ParsedVenue> venue = context.Venues
                .Where(v => v.VenueID == selectedVenueId)
                .ToList();

            // Add menu only if venue found from DB with venueID
            if (venue.Count > 0)
            {
                ParsedVenue selectedVenue = venue[0];

                List<Menu> menus = new List<Menu> { };
                foreach (var beer in beersToBeAdded)
                {
                    // TODO: Do I need to also check if venue exists to avoid duplicate key error? 
                    var existingBeer = context.Beers.Find(beer.BeerID);
                    if (existingBeer == null)
                    {
                        context.Add(new Menu { ParsedVenue = selectedVenue, ParsedBeer = beer });
                    }
                    else
                    {
                        context.Entry(existingBeer).CurrentValues.SetValues(beer);
                    }
                }
                context.SaveChanges();
            }
        }
    }
}
