import "bootstrap/dist/css/bootstrap.min.css";
import React, { useState, useEffect } from "react";
import MapView from "./components/MapView";
import "./styles/app.css";
import beer from "./icons/beer.svg";

const App = () => {
  const [myLocation, setMyLocation] = useState({ lat: null, lng: null });
  const [venueLocations, setVenueLocations] = useState([]);
  const [beersInVenues, setBeersInVenue] = useState([]);

  const [venuesLoadedBool, setVenuesLoadedBool] = useState(false);
  const [beersLoadedBool, setBeersLoadedBool] = useState(false);

  const [selectedVenue, setSelectedVenue] = useState([]);
  const [selectedBeers, setSelectedBeers] = useState([]);

  useEffect(() => {
    if (navigator.geolocation) {
      navigator.geolocation.getCurrentPosition(geoLocation =>
        handleMyLocationChange(geoLocation)
      );
    } else {
      console.log("Your browser doesn't support HTML5 :S");
    }
  }, []);

  useEffect(() => {
    if (myLocation.lat !== null && myLocation.lng !== null) {
      populateVenueData();
    }
  }, [myLocation]);

  useEffect(() => {
    if (venueLocations.length > 0) {
      populateBeerData();
    }
  }, [venueLocations]);

  useEffect(() => {
    if (
      selectedVenue !== null &&
      selectedVenue[0] !== null &&
      selectedVenue[1] !== null
    ) {
      giveSelectedVenueBeers();
    }
  }, [selectedVenue]);

  const handleMyLocationChange = geoLocation => {
    const { latitude, longitude } = geoLocation.coords;
    setMyLocation({ lat: latitude, lng: longitude });
  };

  const handleVenueSelection = event => {
    setSelectedVenue([
      event.target.options.position.venueName,
      event.target.options.position.venueID
    ]);
  };

  const populateVenueData = async () => {
    const response = await fetch(
      `http://localhost:8000/venue/${myLocation.lat}&${myLocation.lng}` // change back to /api/venue/${myLocation.lat}&${myLocation.lng}
    );
    const data = await response.json();
    console.log("tas saatu venue daatta", data);
    setVenueLocations(data);
    setVenuesLoadedBool(true);
  };

  const populateBeerData = async () => {
    let allBeers = await Promise.all(
      venueLocations.map(async venue => {
        const beerResponse = await fetch(
          `http://localhost:8000/beer/${venue.venueID}`
        ); // change back to /api/beer/${venue.venueID}
        const json = beerResponse.json();
        return json;
      })
    );

    for (var i = 0; i < venueLocations.length; i++) {
      allBeers[i].unshift({
        name: venueLocations[i].venueName,
        id: venueLocations[i].venueID
      });
    }
    console.log(allBeers);
    setBeersInVenue(allBeers);
    setBeersLoadedBool(true);
  };

  const giveSelectedVenueBeers = () => {
    for (var i = 0; i < beersInVenues.length; i++) {
      const venueNameIdObject = beersInVenues[i][0];
      if (Object.values(venueNameIdObject).includes(selectedVenue[0])) {
        setSelectedBeers(beersInVenues[i]);
      }
    }
  };

  // <div className="w-screen h-screen absolute z-0">
  //   <MapView
  //     myLocation={myLocation}
  //     venueLocations={venueLocations}
  //     locationsLoaded={venuesLoadedBool}
  //     handleVenueSelection={handleVenueSelection}
  //   />
  // </div>

  return (
    <div className="p-4 flex flex-column">
      <section class="bg-indigo-dark h-50 ">
        <div class="w-10/12 max-w-md mx-auto h-50">
          <input
            class="w-full h-16 px-3 rounded mb-8 focus:outline-none focus:shadow-outline text-xl px-8 shadow-lg"
            type="search"
            placeholder="Search beer or venue..."
          />
        </div>
      </section>

      <div class="w-10/12 max-w-md mx-auto rounded overflow-hidden shadow-lg h-50 p-4">
        <img class="h-auto w-full object-cover" src={beer} alt="beeriä" />
        <div class="px-6 py-4">
          <div class="font-bold text-xl mb-2 text-center">Koff</div>
        </div>
        <div class="p-2">
          <div class=" items-center bg-white leading-none text-pink-600 rounded-full p-2 shadow text-teal text-sm">
            <span class="inline-flex w-8/12 bg-pink-600 text-white rounded-full h-6 px-3 justify-center items-center">
              3,5/5
            </span>
            <span class=" px-2"></span>
          </div>
        </div>
        <div class="px-2 py-4 text-xs sm:text-xs md:text-md lg:text-lg">
          <span class="inline-block bg-gray-200 rounded-full px-2 py-1 font-semibold text-gray-700 mr-2">
            #Hartwall
          </span>
          <span class="inline-block bg-gray-200 rounded-full px-2 py-1 font-semibold text-gray-700 mr-2">
            #Bulk
          </span>
          <span class="inline-block bg-gray-200 rounded-full px-2 py-1 font-semibold text-gray-700">
            #winter
          </span>
        </div>
      </div>

      <div className="flex bottom-0 z-10 text-xs sm:text-xs md:text-md lg:text-lg">
        <div className="w-1/3 text-left">
          <button className="w-11/12 bg-gray-900 hover:bg-gray-800 focus:bg-gray-600 focus:outline-none text-white truncate font-bold py-2 px-4 border border-gray-700 rounded-full">
            Huppendorfer Vollbier
          </button>
        </div>
        <div className="w-1/3 text-center">
          <button className="w-11/12 bg-gray-900 hover:bg-gray-800 focus:bg-gray-600 focus:outline-none text-white truncate font-bold py-2 px-4 border border-gray-700 rounded-full">
            Tropical Sweat
          </button>
        </div>
        <div className="w-1/3 text-right ">
          <button className="w-11/12 bg-gray-900 hover:bg-gray-800 focus:bg-gray-600 focus:outline-none text-white truncate font-bold py-2 px-4 border border-blue-700 rounded-full">
            Koff
          </button>
        </div>
      </div>
    </div>
  );
};

export default App;
