import {Marker, Popup } from 'react-leaflet'
import Leaflet from "leaflet";
import 'leaflet/dist/leaflet.css'

import beer from "../../assets/beer.svg";

export default function VenueMarkers({venues}) {
  let beerIcon = Leaflet.icon({
    iconUrl: beer,
    iconSize: [30, 30],
    iconAnchor: [16, 32]
  });

  return (
    <div>
      {venues.map((venue) => {
        console.log("asd")
        return (
          <Marker
            icon={beerIcon}
            key={venue.venueID}
            position={[venue.lat, venue.lng]}
          />
        );
      })};
    </div>
  )
}