using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using backend_tappi.VenueModel;
using backend_tappi.Data;

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

        public VenueController(ILogger<VenueController> logger, IConfiguration config)
        {
            _logger = logger;
            _apiUrl = config.GetValue<string>("API_URL");
            _clientIdSecret = config.GetValue<string>("CLIENT_ID_SECRET");
        }

        // GET: venue/60.159&24.879
        [HttpGet("{latlng}", Name = "GetVenues")]
        public async Task<List<ParsedVenue>> GetAsync(string latlng)
        {
            string[] coords = latlng.Split('&');
            string lat = coords[0];
            string lng = coords[1];

            // GET FROM API
            List<ParsedVenue> nearVenues = await GetNearbyVenues(lat, lng, 0);

            // PUT TO DB
            DatabaseHandler.InsertVenuesToDatabase(nearVenues);

            _logger.LogInformation($"Got these venues with lat {lat} and lng {lng}:");
            nearVenues.ForEach(venue =>
            {
                _logger.LogInformation($"     name: {venue.VenueName},     address: {venue.Address}");
            });
            return nearVenues;
        }

        private async Task<List<ParsedVenue>> GetNearbyVenues(string lat, string lng, int offset)
        {
            string request = _apiUrl + "thepub/local?" + _clientIdSecret + "&lat=" + lat + "&lng=" + lng + "&radius=1";
            RootObject payloadObject = await DoVenueRequest(request);

            // Start parsing the output object
            int checkinAmount = payloadObject.response.checkins.count;
            var venues = new List<ParsedVenue>();

            var items = payloadObject.response.checkins.items;
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

        private static async Task<RootObject> DoVenueRequest(string request)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(request);
            response.EnsureSuccessStatusCode();
            string serializedResponse = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<RootObject>(serializedResponse);
        }
    }
}
