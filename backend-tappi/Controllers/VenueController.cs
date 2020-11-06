using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using backend_tappi.VenueModel;
using backend_tappi.Data;
using backend_tappi.Utilities;

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
            _apiUrl = Environment.GetEnvironmentVariable("API_URL");
            _clientIdSecret = Environment.GetEnvironmentVariable("CLIENT_ID_SECRET");
            venueContext = context;
        }

        [HttpGet( Name = "GetVenuesFromDB")]
        public async Task<List<ParsedVenue>> GetAsync()
        {
            // GET FROM DB
            List<ParsedVenue> venuesFromDB = await DatabaseHandler.GetVenuesFromDB(venueContext);
            return venuesFromDB;
        }

        // GET: venue/60.159&24.879
        [HttpGet("{latlng}", Name = "GetVenuesFromApi")]
        public async Task<List<ParsedVenue>> GetAsync(string latlng)
        {
            string[] coords = latlng.Split('&');
            double lat = Convert.ToDouble(coords[0]);
            double lng = Convert.ToDouble(coords[1]);

            // GET FROM DB
            List<ParsedVenue> venuesFromDB = await DatabaseHandler.GetVenuesFromDB(venueContext);

            // GET FROM API
            _logger.LogInformation($"Fetching venues close to your location: lat: {lat} and lng: {lng}");
            List<ParsedVenue> venuesFromAPI = await UntappdApiCaller.GetVenuesFromAPI(lat, lng, 0);

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
                await DatabaseHandler.PutVenuesToDatabase(venueContext, missingVenuesFromDB);
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
            return allVenues;
        }
    }
}