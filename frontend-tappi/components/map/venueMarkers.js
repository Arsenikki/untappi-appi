import {Marker} from 'react-map-gl';
import beerIcon from '../../assets/beer.svg';


export default function VenueMarkers({venues}) {
  {console.log("asd")}
  return (
    venues.map((venue, index) => 
      <Marker 
          key={index}
          latitude={venue.lat}
          longitude={venue.lng}
          captureDrag={false}
          offsetLeft={-12}
          offsetTop={-12}
        >
          <img height="24px" width="24px" src={beerIcon} />
        </Marker>
    )
  )
}