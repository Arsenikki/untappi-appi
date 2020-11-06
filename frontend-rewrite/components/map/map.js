import { MapContainer, TileLayer} from 'react-leaflet'
import 'leaflet/dist/leaflet.css'
import PersonMarker from './personMarker';
import VenueMarkers from './venueMarkers'

export default function Map({ venues }) {
  return (
    <MapContainer center={{lat: 48.697339, lng: 14.302189 }} zoom={3} zoomControl={false} style={{height: "100%"}}>
      <TileLayer
        attribution='&copy; <a href="http://osm.org/copyright">OpenStreetMap</a> contributors'
        url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
      />
      <PersonMarker />
      <VenueMarkers venues={venues} />
    </MapContainer>
  )
}