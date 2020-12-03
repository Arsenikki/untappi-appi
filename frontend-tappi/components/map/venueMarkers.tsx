import React from 'react';
import { Marker } from 'react-map-gl';
import beerIcon from '../../assets/beer.svg';

interface Venue {
  address: string;
  category: string;
  lat: number;
  lng: number;
  venueID: number;
  venueName: string;
}

interface Props {
  venues: Venue[];
}

const VenueMarkers: React.FC<Props> = ({ venues }): JSX.Element => (
  <>
    {venues.map((venue) => (
      <Marker
        key={venue.venueID}
        latitude={venue.lat}
        longitude={venue.lng}
        captureDrag={false}
        offsetLeft={-12}
        offsetTop={-12}
      >
        <img height="24px" width="24px" src={beerIcon} alt="VenueLocation" />
      </Marker>
    ))}
  </>
);

export default VenueMarkers;
