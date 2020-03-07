using backend_tappi.MenuModel;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace backend_tappi.VenueModel
{
    [JsonObject]
    public class ParsedVenue
    {
        [Key]
        public int VenueID { get; set; }
        public string VenueName { get; set; }
        public string Address{ get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
        public string Category { get; set; }
        [JsonIgnore]
        public ICollection<Menu> Menus { get; set; }
    }

    public class ResponseTime
    {
        public double time { get; set; }
        public string measure { get; set; }
    }

    public class InitTime
    {
        public int time { get; set; }
        public string measure { get; set; }
    }

    public class Meta
    {
        public int code { get; set; }
        public ResponseTime response_time { get; set; }
        public InitTime init_time { get; set; }
    }

    public class Pagination
    {
        public string next_url { get; set; }
        public string max_id { get; set; }
        public string since_url { get; set; }
    }

    public class User
    {
        public int uid { get; set; }
        public string user_name { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string relationship { get; set; }
        public string location { get; set; }
        public int is_supporter { get; set; }
        public string url { get; set; }
        public string bio { get; set; }
        public string user_avatar { get; set; }
        public int is_private { get; set; }
    }

    public class Beer
    {
        public int bid { get; set; }
        public string beer_name { get; set; }
        public string beer_label { get; set; }
        public string beer_label_hd { get; set; }
        public string beer_style { get; set; }
        public double beer_abv { get; set; }
        public bool has_had { get; set; }
        public int beer_active { get; set; }
    }

    public class Contact
    {
        public string twitter { get; set; }
        public string facebook { get; set; }
        public string instagram { get; set; }
        public string url { get; set; }
    }

    public class Location
    {
        public string brewery_city { get; set; }
        public string brewery_state { get; set; }
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class Brewery
    {
        public int brewery_id { get; set; }
        public string brewery_name { get; set; }
        public string brewery_slug { get; set; }
        public string brewery_page_url { get; set; }
        public string brewery_type { get; set; }
        public string brewery_label { get; set; }
        public string country_name { get; set; }
        public Contact contact { get; set; }
        public Location location { get; set; }
        public int brewery_active { get; set; }
    }

    public class Item2
    {
        public string category_key { get; set; }
        public string category_name { get; set; }
        public string category_id { get; set; }
        public bool is_primary { get; set; }
    }

    public class Categories
    {
        public int count { get; set; }
        public List<Item2> items { get; set; }
    }

    public class Location2
    {
        public string venue_address { get; set; }
        public string venue_city { get; set; }
        public string venue_state { get; set; }
        public string venue_country { get; set; }
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class Contact2
    {
        public string twitter { get; set; }
        public string venue_url { get; set; }
    }

    public class Foursquare
    {
        public string foursquare_id { get; set; }
        public string foursquare_url { get; set; }
    }

    public class VenueIcon
    {
        public string sm { get; set; }
        public string md { get; set; }
        public string lg { get; set; }
    }

    public class Venue
    {
        public int venue_id { get; set; }
        public string venue_name { get; set; }
        public string venue_slug { get; set; }
        public string primary_category_key { get; set; }
        public string primary_category { get; set; }
        public string parent_category_id { get; set; }
        public Categories categories { get; set; }
        public Location2 location { get; set; }
        public Contact2 contact { get; set; }
        public Foursquare foursquare { get; set; }
        public VenueIcon venue_icon { get; set; }
        public bool is_verified { get; set; }
    }

    public class Comments
    {
        public int total_count { get; set; }
        public int count { get; set; }
        public List<object> items { get; set; }
    }

    public class Toasts
    {
        public int total_count { get; set; }
        public int count { get; set; }
        public bool? auth_toast { get; set; }
        public List<object> items { get; set; }
    }

    public class Media
    {
        public int count { get; set; }
        public List<object> items { get; set; }
    }

    public class Source
    {
        public string app_name { get; set; }
        public string app_website { get; set; }
    }

    public class Badges
    {
        public int count { get; set; }
        public List<object> items { get; set; }
        public bool? retro_status { get; set; }
    }

    public class Item
    {
        public int checkin_id { get; set; }
        public string created_at { get; set; }
        public double rating_score { get; set; }
        public string checkin_comment { get; set; }
        public User user { get; set; }
        public Beer beer { get; set; }
        public Brewery brewery { get; set; }
        public Venue venue { get; set; }
        public Comments comments { get; set; }
        public Toasts toasts { get; set; }
        public Media media { get; set; }
        public Source source { get; set; }
        public Badges badges { get; set; }
    }

    public class Checkins
    {
        public int count { get; set; }
        public List<Item> items { get; set; }
    }

    public class Response
    {
        public int limit { get; set; }
        public int offset { get; set; }
        public string type { get; set; }
        public Pagination pagination { get; set; }
        public int radius { get; set; }
        public string dist_pref { get; set; }
        public Checkins checkins { get; set; }
    }

    [JsonObject]
    public class RootObject
    {
        public Meta meta { get; set; }
        public List<object> notifications { get; set; }
        public Response response { get; set; }
    }
}
