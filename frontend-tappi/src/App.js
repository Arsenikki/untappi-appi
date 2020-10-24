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
  const [selectedBeer, setSelectedBeer] = useState();
  const [showSelectedBeer, setShowSelectedBeer] = useState(false);

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
  }, [myLocation, populateVenueData]);

  useEffect(() => {
    if (venueLocations.length > 0) {
      populateBeerData();
    }
  }, [populateBeerData, venueLocations]);

  useEffect(() => {
    if (
      selectedVenue !== null &&
      selectedVenue[0] !== null &&
      selectedVenue[1] !== null
    ) {
      giveSelectedVenueBeers();
    }
  }, [giveSelectedVenueBeers, selectedVenue]);

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

  const handleBeerSelection = (event, beer) => {
    if (beer === selectedBeer) {
      setShowSelectedBeer(false);
      setSelectedBeer(null);
    } else {
      setSelectedBeer(beer);
      setShowSelectedBeer(true);
    }
  };

  const handleBlur = event => {
    setShowSelectedBeer(false);
    setSelectedBeer(null);
  };

  const populateVenueData = async () => {
    const response = await fetch(
      `https://untappiappi.westeurope.cloudapp.azure.com//api/venue/${myLocation.lat}&${myLocation.lng}` // change back to
    );
    const data = await response.json();
    console.log("tas saatu venue daatta", data);
    setVenueLocations(data);
    setVenuesLoadedBool(true);
  };

  const populateBeerData = async () => {
    let allBeers = await Promise.all(
      venueLocations.map(async venue => {
        const beerResponse = await fetch(`https://untappiappi.westeurope.cloudapp.azure.com//api/beer/${venue.venueID}`); // change back to
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

  const scaleRatingBar = () => {
    let scaling = `${((selectedBeer.rating / 5) * 50).toFixed(0)}/50`;
    console.log(scaling);
    return scaling;
  };

  return (
    <div>
      <div className="w-screen h-full absolute z-10">
        <MapView
          myLocation={myLocation}
          venueLocations={venueLocations}
          locationsLoaded={venuesLoadedBool}
          handleVenueSelection={handleVenueSelection}
        />
      </div>
      <div className="flex flex-col justify-between h-full w-full absolute p-2 overflow-hidden">
        <section class="w-7/12 bg-indigo-dark max-w-md mx-auto  z-10">
          <input
            class="w-full h-16 px-3 rounded focus:outline-none focus:shadow-outline text-xl px-8 shadow-md"
            type="search"
            placeholder="Search beer or venue... (To be implemented)"
          />
        </section>

        {selectedBeer && showSelectedBeer ? (
          <div class="w-7/12 bg-gray-100 max-w-md mx-auto my-auto rounded overflow-hidden shadow-md p-2 z-10">
            <img class="h-auto w-full object-cover" src={beer} alt="beeriä" />
            <div class="px-2 py-4">
              <div class="font-bold text-xl mb-2 text-center">
                {selectedBeer.beerName}
              </div>
            </div>
            <div class="p-2">
              <div class=" items-center bg-white leading-none text-pink-600 rounded-full p-2 shadow text-teal text-sm">
                <span
                  class={`inline-flex w-${scaleRatingBar()} bg-pink-600 text-white rounded-full h-6 px-3 justify-center items-center`}
                >
                  {selectedBeer.rating.toFixed(2)}/5
                </span>
              </div>
            </div>
            <div class="px-2 py-2 text-xs sm:text-xs md:text-md lg:text-lg">
              <span class="inline-block bg-gray-200 rounded-full px-2 py-1 font-semibold text-gray-700 mr-2">
                #{selectedBeer.brewery}
              </span>
              <span class="inline-block bg-gray-200 rounded-full px-2 py-1 font-semibold text-gray-700 mr-2">
                #{selectedBeer.style}
              </span>
              <span class="inline-block bg-gray-200 rounded-full px-2 py-1 font-semibold text-gray-700">
                #{selectedBeer.country}
              </span>
            </div>
          </div>
        ) : null}

        <div className="flex justify-evenly bottom-0 z-10 text-xs sm:text-xs md:text-md lg:text-lg">
          {selectedBeers.slice(1).map(beer => (
            <div key={beer.beerID} className="w-1/3 text-center">
              <button
                onClick={event => handleBeerSelection(event, beer)}
                onBlur={event => handleBlur(event)}
                className="w-11/12 rounded bg-gray-900 hover:bg-gray-800 focus:bg-gray-600 focus:outline-none text-white truncate font-bold py-2 px-2 border border-gray-700"
              >
                {beer.beerName}
              </button>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
};

export default App;
