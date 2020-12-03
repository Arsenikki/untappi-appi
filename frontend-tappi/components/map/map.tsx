import React, { CSSProperties, useState } from 'react';
import ReactMapGL, { GeolocateControl, FlyToInterpolator, ViewportProps } from 'react-map-gl';
import NavBar from '../navBar/navBar';
import VenueMarkers from './venueMarkers';

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

const Map: React.FC<Props> = ({ venues }): JSX.Element => {
  const [darkMode, setDarkMode] = useState(false);
  const [viewport, setViewport] = useState({
    latitude: 34.34785,
    longitude: 20.406802,
    zoom: 1,
  });

  const geolocateStyle: CSSProperties = {
    position: 'absolute',
    top: 72,
    right: 0,
    margin: 8,
    width: 64,
    height: 64,
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'center',
    zIndex: 10,
  };

  const handleToggleDarkMode = () => {
    console.log('dark mode toggled');
    setDarkMode(!darkMode);
  };

  const updateViewport = (newViewport: ViewportProps) => {
    const viewportOptions = {
      ...newViewport,
      zoom: 14,
      transitionDuration: 'auto',
      transitionInterpolator: new FlyToInterpolator(),
    };
    setViewport(viewportOptions);
  };

  return (
    <div className="flex flex-col h-screen">
      <ReactMapGL
        {...viewport}
        width="100%"
        height="100%"
        mapStyle={
          darkMode
            ? 'mapbox://styles/mapbox/dark-v10?optimize=true'
            : 'mapbox://styles/mapbox/streets-v11?optimize=true'
        }
        onViewportChange={(nextViewport: ViewportProps) => setViewport(nextViewport)}
        mapboxApiAccessToken="pk.eyJ1IjoiYXJzZW5pa2tpIiwiYSI6ImNraGIzaW51cjAyOXQyem1peHRpaHR3cHUifQ.axEOvzZzc4nm9T_CWdoWAQ"
        reuseMaps
      >
        <GeolocateControl
          style={geolocateStyle}
          positionOptions={{ enableHighAccuracy: true }}
          trackUserLocation
          onViewportChange={(newViewport: ViewportProps) => updateViewport(newViewport)}
        />
        <VenueMarkers venues={venues} />
      </ReactMapGL>
      <NavBar darkMode={darkMode} handleToggleDarkMode={handleToggleDarkMode} />
    </div>
  );
};

export default Map;
