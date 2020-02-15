using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using backend_tappi.BeerModel;
using System;
using MySql.Data.MySqlClient;

namespace backend_tappi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BeerController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly ILogger<BeerController> _logger;
        private MySqlConnection connection;

        public BeerController(ILogger<BeerController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        // GET: /Beer/5
        [HttpGet("{venueId}", Name = "GetBeers")]
        public async Task<List<ParsedBeer>> GetAsync(int venueId)
        {
            // Search venue from database by venueId
            // var beersInVenue = await GetBeersFromDB
            List<ParsedBeer> beersInVenue = await FetchBeersForVenue(venueId);
            // if no venue found get beers from api by venueId
            if (beersInVenue.Count == 0)
            {
                _logger.LogInformation($"Requested beers for venue with id: {venueId}");
                beersInVenue = await GetBeersFromAPI(venueId);

                // Add beer information to personal database
                await AddBeersToDatabase(beersInVenue);
            }

            _logger.LogInformation($"Got these beers for id {venueId}:");
            beersInVenue.ForEach(beer =>
            {
                _logger.LogInformation($"     name: {beer.Name},     brewery: {beer.Brewery}");
            });
            return beersInVenue;
        }

        private Task FetchBeersForVenue(int venueId)
        {
            throw new NotImplementedException();
            // Fetch combined table
        }

        private async Task AddBeersToDatabase(List<ParsedBeer> beersInVenue)
        {
            connection = new MySqlConnection(_config.GetValue<string>("DB_CONNECTION"));
            _logger.LogInformation("Establishing connection to SQL DB");
 
            beersInVenue.ForEach(async beer =>
            {
                try
                {
                    await InsertBeer(beer);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                }
            });

            await connection.CloseAsync();
        }

        private async Task InsertBeer(ParsedBeer beer)
        {
            await connection.OpenAsync();

            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = @"INSERT INTO beers (name, brewery, country, style, stronkness, rating) VALUES (@name, @brewery, @country, @style, @stronkness, @rating);";
            cmd.Prepare();
            cmd.Parameters.AddWithValue("@name", beer.Name);
            cmd.Parameters.AddWithValue("@brewery", beer.Brewery);
            cmd.Parameters.AddWithValue("@country", beer.Country);
            cmd.Parameters.AddWithValue("@style", beer.Style);
            cmd.Parameters.AddWithValue("@stronkness", beer.Stronkness);
            cmd.Parameters.AddWithValue("@rating", beer.Rating);
            int rowCount = await cmd.ExecuteNonQueryAsync();
            Console.WriteLine(String.Format("Number of rows inserted={0}", rowCount));
        }

        private async Task<List<ParsedBeer>> GetBeersFromAPI(int venueId)
        {
            string apiUrl = _config.GetValue<string>("API_URL");
            string clientIdSecret = _config.GetValue<string>("CLIENT_ID_SECRET");
            string request = apiUrl + "venue/info/" + venueId + "?" + clientIdSecret;
            RootObject payloadObject = await ExecuteAPIRequest(request);

            // Start parsing the output object
            int topBeerCount = payloadObject.response.venue.top_beers.count;
            var listOfBeers = new List<ParsedBeer>();

            var items = payloadObject.response.venue.top_beers.items;
            for (int i = 0; i < topBeerCount; i++)
			{
                var beer = (new ParsedBeer
                {
                    Name = items[i].beer.beer_name,
                    Brewery = items[i].brewery.brewery_name,
                    Country = items[i].brewery.country_name,
                    Style = items[i].beer.beer_style,
                    Rating = items[i].beer.rating_score,
                    Stronkness = items[i].beer.beer_abv
                });
                listOfBeers.Add(beer);
			}
            return listOfBeers;
        }

        private static async Task<RootObject> ExecuteAPIRequest(string request)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(request);
            response.EnsureSuccessStatusCode();
            string serializedResponse = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<RootObject>(serializedResponse);
        }
    }
}
