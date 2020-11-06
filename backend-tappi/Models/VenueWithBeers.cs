using backend_tappi.BeerModel;
using backend_tappi.VenueModel;
using System;
using System.Collections.Generic;

namespace backend_tappi.VenueWithBeerModel
{
    public class VenueWithBeers : ParsedVenue
    {
        public List<ParsedBeer> ParsedBeer { get; set; }
    }
}
