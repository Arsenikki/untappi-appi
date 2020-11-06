using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using backend_tappi.BeerModel;
using backend_tappi.VenueModel;
using VenueRootObject = backend_tappi.VenueModel.RootObject;
using BeerRootObject = backend_tappi.BeerModel.RootObject;

namespace backend_tappi.Utilities
{
    public class UntappdApiCaller
    {
        private static string _apiUrl = Environment.GetEnvironmentVariable("API_URL");
        private static string _clientIdSecret = Environment.GetEnvironmentVariable("CLIENT_ID_SECRET");
        private static List<string> _acceptedCategories = new List<string> { "Nightlife Spot", "Food", "Event", "Shop & Service" };

        public static async Task<List<ParsedBeer>> GetBeersFromAPI(int venueId)
        {
            // Execute API request
            string request = _apiUrl + "venue/info/" + venueId + "?" + _clientIdSecret;
            var items = await SendBeerRequest(request);

            // Create list of beers
            int topBeerCount = items.Count;
            var listOfBeers = new List<ParsedBeer>();
            for (int i = 0; i < topBeerCount; i++)
            {
                var beer = (new ParsedBeer
                {
                    BeerName = items[i].beer.beer_name,
                    Brewery = items[i].brewery.brewery_name,
                    Country = items[i].brewery.country_name,
                    Style = items[i].beer.beer_style,
                    Rating = items[i].beer.rating_score,
                    RatingCount = items[i].beer.rating_count,
                    Stronkness = items[i].beer.beer_abv,
                    BeerID = items[i].beer.bid
                });
                listOfBeers.Add(beer);
            }
            return listOfBeers;
        }

        // TODO: use the offset to get even more places!!
        public static async Task<List<ParsedVenue>> GetVenuesFromAPI(double lat, double lng, int offset)
        {
            // Execute API request
            string request = _apiUrl + "thepub/local?" + _clientIdSecret + "&lat=" + lat + "&lng=" + lng + "&radius=10";
            var items = await SendVenueRequest(request);

            // Create list of presumably legit venues
            int checkinAmount = items.Count;
            var venues = new List<ParsedVenue>();
            for (int i = 0; i < checkinAmount; i++)
            {
                string category = items[i].venue.primary_category;
                var parsedVenue = (new ParsedVenue
                {
                    VenueName = items[i].venue.venue_name,
                    Address = items[i].venue.location.venue_address,
                    Lat = items[i].venue.location.lat,
                    Lng = items[i].venue.location.lng,
                    Category = category,
                    VenueID = items[i].venue.venue_id
                });

                bool alreadyExists = venues.Exists(f => f.VenueName == items[i].venue.venue_name);
                if (_acceptedCategories.Contains(category) && !alreadyExists)
                {
                    venues.Add(parsedVenue);
                }
            }
            return venues;
        }

        private static async Task<List<backend_tappi.BeerModel.Item6>> SendBeerRequest(string request)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(request);
            response.EnsureSuccessStatusCode();
            string serializedResponse = await response.Content.ReadAsStringAsync();
            BeerRootObject deserializedObject = JsonConvert.DeserializeObject<BeerRootObject>(serializedResponse);
            return deserializedObject.response.venue.top_beers.items;
        }

        private static async Task<List<backend_tappi.VenueModel.Item>> SendVenueRequest(string request)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(request);
            response.EnsureSuccessStatusCode();
            string serializedResponse = await response.Content.ReadAsStringAsync();
            VenueRootObject deserializedObject = JsonConvert.DeserializeObject<VenueRootObject>(serializedResponse);
            return deserializedObject.response.checkins.items;
        }
    }

}
