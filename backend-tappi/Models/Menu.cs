using backend_tappi.BeerModel;
using backend_tappi.VenueModel;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace backend_tappi.MenuModel
{
    public class Menu
    {
        [Key]
        public int VenueId { get; set; }
        public int BeerId { get; set; }
        public ParsedBeer ParsedBeer{ get; set; }
        public ParsedVenue ParsedVenue { get; set; }
    }
}