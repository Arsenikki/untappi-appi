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
        private readonly ILogger<BeerController> _logger;
        private string _connectionString;
        private string _apiUrl;
        private string _clientIdSecret;
        private MySqlConnection _connection;

        public BeerController(ILogger<BeerController> logger, IConfiguration config)
        {
            _logger = logger;
            _connectionString = config.GetValue<string>("DB_CONNECTION");
            _apiUrl = config.GetValue<string>("API_URL");
            _clientIdSecret = config.GetValue<string>("CLIENT_ID_SECRET");
        }

        // GET: /Beer/189889
        [HttpGet("{venueId}", Name = "GetBeers")]
        public async Task<List<ParsedBeer>> GetAsync(int venueId)
        {
            // Search venue from database by venueId
            List<ParsedBeer> beersInVenue = FetchBeersFromDB(venueId);

            // if no venue found get beers from api by venueId
            if (beersInVenue.Count == 0)
            {
                _logger.LogInformation($"Requested beers for venue with id: {venueId}");
                beersInVenue = await GetBeersFromAPI(venueId);

                // Add beer information to personal database
                await AddBeersToDatabase(venueId, beersInVenue);
            }
            _logger.LogInformation($"Got these beers for id {venueId}:");
            beersInVenue.ForEach(beer =>
            {
                _logger.LogInformation($"     name: {beer.Name},     brewery: {beer.Brewery}");
            });
            return beersInVenue;
        }

        private List<ParsedBeer> FetchBeersFromDB(int venueId)
        {
            List<ParsedBeer> beersFromDB = new List<ParsedBeer>();
            List<int> venueBeerIDs = new List<int>();

            _connection = new MySqlConnection(_connectionString);
            _connection.Open();
            _logger.LogInformation("Established connection to SQL DB");

            // Fetch beerIDs for specific venueID
            MySqlCommand cmd = new MySqlCommand
            {
                Connection = _connection,
                CommandText = $"SELECT beerID FROM venues_beers WHERE venueID={venueId}"
            };
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    venueBeerIDs.Add(reader.GetInt32(0));
                }
            }

            // Fetch beer data for specific beerIDs
            if (venueBeerIDs.Count > 0)
            {
                MySqlCommand cmd2 = new MySqlCommand();

                var parameters = new string[venueBeerIDs.Count];
                for (int i = 0; i < venueBeerIDs.Count; i++)
                {
                    parameters[i] = string.Format("@beer{0}", i);
                    cmd2.Parameters.AddWithValue(parameters[i], venueBeerIDs[i]);
                }
                cmd2.Connection = _connection;
                cmd2.CommandText = string.Format("SELECT * FROM beers WHERE beerID IN ({0})", string.Join(", ", parameters));

                using MySqlDataReader reader2 = cmd2.ExecuteReader();
                while (reader2.Read())
                {
                    var beer = (new ParsedBeer
                    {
                        Id = reader2.GetInt32(0),
                        Name = reader2.GetValue(1).ToString(),
                        Brewery = reader2.GetValue(2).ToString(),
                        Country = reader2.GetValue(3).ToString(),
                        Style = reader2.GetValue(4).ToString(),
                        Stronkness = reader2.GetDouble(6),
                        Rating = reader2.GetDouble(5)
                    });
                    beersFromDB.Add(beer);
                }
            }
            return beersFromDB;
        }

        private async Task AddBeersToDatabase(int venueId ,List<ParsedBeer> beersInVenue)
        {
            beersInVenue.ForEach(beer =>
            {
                try
                {
                    InsertBeer(beer);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"InsertBeer failed: {ex.Message} {ex.InnerException.Message} ");
                }

                try
                {
                    InsertVenuesBeers(venueId, beer);
                }
                catch (Exception ex2)
                {
                    _logger.LogError($"InsertVenuesBeers failed: {ex2.Message} {ex2.InnerException.Message} ");
                }
            });

            await _connection.CloseAsync();
        }

        private void InsertBeer(ParsedBeer beer)
        {
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = _connection;
            cmd.CommandText = @"INSERT INTO beers (beerID, name, brewery, country, style, stronkness, rating) VALUES (@beerID, @name, @brewery, @country, @style, @stronkness, @rating);";
            cmd.Prepare();
            cmd.Parameters.AddWithValue("@beerID", beer.Id);
            cmd.Parameters.AddWithValue("@name", beer.Name);
            cmd.Parameters.AddWithValue("@brewery", beer.Brewery);
            cmd.Parameters.AddWithValue("@country", beer.Country);
            cmd.Parameters.AddWithValue("@style", beer.Style);
            cmd.Parameters.AddWithValue("@stronkness", beer.Stronkness);
            cmd.Parameters.AddWithValue("@rating", beer.Rating);
            int rowCount = cmd.ExecuteNonQuery();
            _logger.LogInformation($"Inserted beer: {beer.Name}");
        }

        private void InsertVenuesBeers(int venueId, ParsedBeer beer)
        {
            MySqlCommand cmd3 = new MySqlCommand();
            cmd3.Connection = _connection;
            cmd3.CommandText = "INSERT INTO venues_beers (venueID, beerID) VALUES (@venueID, @beerID)";
            cmd3.Parameters.AddWithValue("@venueID", venueId);
            cmd3.Parameters.AddWithValue("@beerID", beer.Id);
            cmd3.ExecuteNonQuery();
            _logger.LogInformation($"Inserted beer: {beer.Name}");
        }

        private async Task<List<ParsedBeer>> GetBeersFromAPI(int venueId)
        {
            string request = _apiUrl + "venue/info/" + venueId + "?" + _clientIdSecret;
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
                    Stronkness = items[i].beer.beer_abv,
                    Id = items[i].beer.bid
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
