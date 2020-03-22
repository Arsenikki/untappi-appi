import "bootstrap/dist/css/bootstrap.min.css";
import React, { useState, useEffect } from "react";
import MapView from "./components/MapView";
import BeerList from "./components/BeerList";
import styled from "styled-components";

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
      `/api/venue/${myLocation.lat}&${myLocation.lng}`
    );
    const data = await response.json();
    console.log("tas saatu venue daatta", data);
    setVenueLocations(data);
    setVenuesLoadedBool(true);
  };

  const populateBeerData = async () => {
    let allBeers = await Promise.all(
      venueLocations.map(async venue => {
        const beerResponse = await fetch(`/api/beer/${venue.venueID}`);
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

  const StyledMap = styled.div`
    width: 100%;
    height: 100%;
    position: absolute;
    position: absolute;
    left: 50%;
    top: 50%;
    transform: translate(-50%, -50%);
    z-index: 1;
  `;

  const StyledBeersOverlap = styled.div`
    width: 100%;
    height: 20%;
    z-index: 2;
  `;

  const StyledContainer = styled.div`
    display: flex;
    alignitems: center;
    justifycontent: center;
  `;

  return (
    <StyledContainer>
      <StyledMap>
        <MapView
          myLocation={myLocation}
          venueLocations={venueLocations}
          locationsLoaded={venuesLoadedBool}
          handleVenueSelection={handleVenueSelection}
        />
      </StyledMap>
      <StyledBeersOverlap>
        {beersLoadedBool ? (
          <BeerList
            selectedBeers={selectedBeers}
            selectedVenue={selectedVenue}
          />
        ) : null}
      </StyledBeersOverlap>
    </StyledContainer>
  );
};

export default App;
