using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using backend_tappi.BeerModel;
using backend_tappi.Data;

namespace backend_tappi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BeerController : ControllerBase
    {
        private readonly ILogger<BeerController> _logger;
        private string _apiUrl;
        private string _clientIdSecret;
        private MenuContext beerContext;
        public BeerController(ILogger<BeerController> logger, IConfiguration config, MenuContext context)
        {
            _logger = logger;
            _apiUrl = Environment.GetEnvironmentVariable("API_URL");
            _clientIdSecret = Environment.GetEnvironmentVariable("CLIENT_ID_SECRET");
            beerContext = context;
        }

        // GET: /Beer/189889
        [HttpGet("{venueId}", Name = "GetBeers")]
        public async Task<List<ParsedBeer>> GetAsync(int venueId)
        {
            List<ParsedBeer> allBeers = new List<ParsedBeer>();
            // GET FROM DB
            List<ParsedBeer> beersFromDB = await DatabaseHandler.GetBeersFromDbForVenue(beerContext, venueId);
            allBeers.AddRange(beersFromDB);

            // if no venue found get beers from api by venueId
            if (beersFromDB.Count == 0)
            {
                _logger.LogInformation($"No beers found from DB with venue id: {venueId}");

                // GET FROM API
                List<ParsedBeer> beersFromAPI = await GetBeersFromAPI(venueId);
                allBeers.AddRange(beersFromAPI);

                if (beersFromAPI.Count != 0)
                {
                    _logger.LogInformation($"Adding these beers to DB:");
                    beersFromAPI.ForEach(beer =>
                    {
                        _logger.LogInformation($"     name: {beer.BeerName},     id: {beer.BeerID}");
                    });

                    // PUT BEER TO DB
                    await DatabaseHandler.InsertBeersToDatabase(beerContext, venueId, beersFromAPI);
                }
            }
            return allBeers;
        }
            

        private async Task<List<ParsedBeer>> GetBeersFromAPI(int venueId)
        {
            // Execute API request
            string request = _apiUrl + "venue/info/" + venueId + "?" + _clientIdSecret;
            var items = await ExecuteAPIRequest(request);

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

        private static async Task<List<Item6>> ExecuteAPIRequest(string request)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(request);
            response.EnsureSuccessStatusCode();
            string serializedResponse = await response.Content.ReadAsStringAsync();
            RootObject deserializedObject = JsonConvert.DeserializeObject<RootObject>(serializedResponse);
            return deserializedObject.response.venue.top_beers.items;
        }
    }
}
