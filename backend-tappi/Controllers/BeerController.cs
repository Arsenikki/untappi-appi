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
using backend_tappi.MenuModel;
using backend_tappi.VenueWithBeerModel;
using backend_tappi.Utilities;

namespace backend_tappi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BeerController : ControllerBase
    {
        private readonly ILogger<BeerController> _logger;
        private MenuContext beerContext;
        public BeerController(ILogger<BeerController> logger, MenuContext context)
        {
            _logger = logger;
            beerContext = context;
        }

        [HttpGet( Name = "GetAllBeers")]
        public async Task<List<VenueWithBeers>> GetAsync() {
            // GET ALL FROM DB
            List<VenueWithBeers> allVenuesWithBeers = await DatabaseHandler.GetAllMenus(beerContext);
            return allVenuesWithBeers;
        }

        // GET: /Beer/189889
        [HttpGet("{venueId}", Name = "GetBeersForSingleVenue")]
        public async Task<List<ParsedBeer>> GetAsync(int venueId)
        {
            // GET FROM DB
            List<ParsedBeer> beersFromDB = await DatabaseHandler.GetBeersFromDbForVenue(beerContext, venueId);

            // if no venue found get beers from api by venueId
            if (beersFromDB.Count == 0)
            {
                _logger.LogInformation($"No beers found from DB with venue id: {venueId}");

                // GET FROM API
                List<ParsedBeer> beersFromAPI = await UntappdApiCaller.GetBeersFromAPI(venueId);

                if (beersFromAPI.Count != 0)
                {
                    _logger.LogInformation($"Adding these beers to DB:");
                    beersFromAPI.ForEach(beer =>
                    {
                        _logger.LogInformation($"     name: {beer.BeerName},     id: {beer.BeerID}");
                    });

                    // PUT BEER TO DB
                    DatabaseHandler.PutBeersToDatabase(beerContext, venueId, beersFromAPI);
                }
                return beersFromAPI;
            }
            return beersFromDB;
        }
    }
}
