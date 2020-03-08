import 'leaflet/dist/leaflet.css';
import React from 'react';
import { Map, Marker, Popup, TileLayer } from 'react-leaflet'
import Leaflet from 'leaflet'
import { popupContent, popupHead } from '../popupStyles';

import student from '../icons/student.svg';
import beer from '../icons/beer.svg'

let studentIcon = Leaflet.icon({
    iconUrl: student,
    iconSize: [35, 35],
    iconAnchor: [12, 36],
    popupAnchor: [0, -25]
});

let BeerIcon = Leaflet.icon({
    iconUrl: beer,
    iconSize: [35, 35],
    iconAnchor: [16, 32]
});

const MapView = ({ myLocation, venueLocations, locationsLoaded, handleVenueSelection }) => {
    return (
        <Map style={{ "height": "100%" }} center={myLocation} zoom={14}>
            <TileLayer
                attribution='&amp;copy <a href="http://osm.org/copyright">OpenStreetMap</a> contributors'
                url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
            />
            {myLocation ?
                <Marker icon={studentIcon} position={myLocation}>
                <Popup className="request-popup">
                    <div style={popupContent}>
                        <img
                        src={studentIcon}
                        alt="Large person icon"
                        width="150"
                        height="150"
                        />
                        <div style={popupHead}>
                        I am you
                        </div>
                    </div>
                </Popup>
                </Marker>
                : null
            }
            {locationsLoaded
                ? venueLocations.map(venue => {
                    console.log("venueee", venue)
                    return (
                        <Marker icon={BeerIcon} key={venue.venueName} position={venue} onClick={handleVenueSelection} />
                    )
                })
                : null
            }
        </Map>
    )
}

export default MapView;