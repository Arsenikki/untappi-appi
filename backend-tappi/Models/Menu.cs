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
        public int VenueID { get; set; }
        public ParsedVenue ParsedVenue{ get; set; }
        public int BeerID { get; set; }
        public ParsedBeer ParsedBeer{ get; set; }
    }
}