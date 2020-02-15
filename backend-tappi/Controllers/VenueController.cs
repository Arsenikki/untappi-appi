using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using backend_tappi.VenueModel;

namespace backend_tappi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VenueController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly ILogger<VenueController> _logger;

        public VenueController(ILogger<VenueController> logger, IConfiguration config)
        {
            _config = config;
            _logger = logger;
        }

        // GET: api/Venue/60.159&24.879     // maybe rather have api/Venue?lat=60.159&lng=24.879
        [HttpGet("{latlng}", Name = "GetVenues")]
        public async Task<List<ParsedVenue>> GetAsync(string latlng)
        {
            string[] coords = latlng.Split('&');
            string lat = coords[0];
            string lng = coords[1];
            _logger.LogInformation($"Requested near venues with lat: {lat} and lng: {lng}");
            List<ParsedVenue> nearVenues = await GetNearbyVenues(lat, lng, 0);
            _logger.LogInformation($"Got these venues with lat {lat} and lng {lng}:");
            nearVenues.ForEach(venue =>
            {
                _logger.LogInformation($"     name: {venue.Name},     address: {venue.Address}");
            });
            return nearVenues;
        }

        private async Task<List<ParsedVenue>> GetNearbyVenues(string lat, string lng, int offset)
        {
            string apiUrl = _config.GetValue<string>("API_URL");
            string clientIdSecret = _config.GetValue<string>("CLIENT_ID_SECRET");
            string request = apiUrl + "thepub/local?" + clientIdSecret + "&lat=" + lat + "&lng=" + lng + "&radius=1";
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
                    Name = items[i].venue.venue_name,
                    Address = items[i].venue.location.venue_address,
                    Lat = items[i].venue.location.lat,
                    Lng = items[i].venue.location.lng,
                    Id = items[i].venue.venue_id,
                    Category = category
                });

                bool alreadyExists = venues.Exists(f => f.Name == items[i].venue.venue_name);
                if (category != "Outdoors & Recreation" && !alreadyExists)
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
