using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using backend_tappi.VenueModel;
using backend_tappi.Data;
using System;
using System.Linq;

namespace backend_tappi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VenueController : ControllerBase
    {
        private readonly ILogger<VenueController> _logger;
        private string _apiUrl;
        private string _clientIdSecret;
        private List<string> _acceptedCategories = new List<string> { "Nightlife Spot", "Food", "Event", "Shop & Service" };
        private MenuContext venueContext;

        public VenueController(ILogger<VenueController> logger, IConfiguration config, MenuContext context)
        {
            _logger = logger;
            _apiUrl = config.GetValue<string>("API_URL");
            _clientIdSecret = config.GetValue<string>("CLIENT_ID_SECRET");
            venueContext = context;
        }

        // GET: venue/60.159&24.879
        [HttpGet("{latlng}", Name = "GetVenues")]
        public async Task<List<ParsedVenue>> GetAsync(string latlng)
        {
            string[] coords = latlng.Split('&');
            double lat = Convert.ToDouble(coords[0]);
            double lng = Convert.ToDouble(coords[1]);

            // GET FROM DB
            List<ParsedVenue> venuesFromDB = await DatabaseHandler.GetVenuesFromDB(venueContext, lat, lng);

            // GET FROM API
            _logger.LogInformation($"Fetching venues close to your location: lat: {lat} and lng: {lng}");
            List<ParsedVenue> venuesFromAPI = await GetVenuesFromAPI(lat, lng, 0);

            // COMBINE VENUES FROM DB AND API
            List<ParsedVenue> allVenues = new List<ParsedVenue>();
            List<ParsedVenue> missingVenuesFromDB = new List<ParsedVenue>();
            allVenues.AddRange(venuesFromDB);
            for (int i = 0; i < venuesFromAPI.Count; i++)
            {
                if (!allVenues.Exists(v => v.VenueID == venuesFromAPI[i].VenueID))
                {
                    allVenues.Add(venuesFromAPI[i]);
                    missingVenuesFromDB.Add(venuesFromAPI[i]);
                }
            }

            // // PUT VENUES FROM API TO DB IF MISSING
            if(missingVenuesFromDB.Count != 0)
            {
                DatabaseHandler.InsertVenuesToDatabase(venueContext, missingVenuesFromDB);
            }
            

            _logger.LogInformation($"DB venues with lat: {lat} and lng: {lng}");
            venuesFromDB.ForEach(venue =>
            {
                _logger.LogInformation($"     name: {venue.VenueName},     address: {venue.Address}");
            });

            _logger.LogInformation($"API venues with lat: {lat} and lng: {lng}");
            venuesFromAPI.ForEach(venue =>
            {
                _logger.LogInformation($"     name: {venue.VenueName},     address: {venue.Address}");
            });
            return venuesFromAPI;
        }

        // TODO: use the offset to get even more places!!
        private async Task<List<ParsedVenue>> GetVenuesFromAPI(double lat, double lng, int offset)
        {
            // Execute API request
            string request = _apiUrl + "thepub/local?" + _clientIdSecret + "&lat=" + lat + "&lng=" + lng + "&radius=3";
            var items = await DoVenueRequest(request);

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

        private static async Task<List<Item>> DoVenueRequest(string request)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(request);
            response.EnsureSuccessStatusCode();
            string serializedResponse = await response.Content.ReadAsStringAsync();
            RootObject deserializedObject = JsonConvert.DeserializeObject<RootObject>(serializedResponse);
            return deserializedObject.response.checkins.items;
        }
    }
}
