import { useState } from 'react';
import ReactMapGL, {GeolocateControl, FlyToInterpolator, Marker} from "react-map-gl";
import NavBar from '../navBar/navBar'
import VenueMarkers from './venueMarkers'

export default function Map({ venues }) {
  const [darkMode, setDarkMode] = useState(false)
  const [viewport, setViewport] = useState({
    latitude: 34.347850, 
    longitude: 20.406802,
    zoom: 1,
  });

  const geolocateStyle = {
    position: 'absolute',
    top: 72,
    right: 0,
    margin: 8,
    width: 64,
    height: 64,
    display: "flex",
    alignItems: "center",
    justifyContent: "center",
    zIndex: 10
  };

  const handleToggleDarkMode = () => {
    console.log("dark mode toggled")
    setDarkMode(!darkMode)
  };

  const updateViewport = (newViewport) => {
    const viewportOptions = {
      ...newViewport,
      zoom: 14,
      transitionDuration: "auto",
      transitionInterpolator: new FlyToInterpolator()
    };
    setViewport(viewportOptions)
  }

  return (
    <div className="flex flex-col h-screen">
      <ReactMapGL
        {...viewport}
        width="100%"
        height="100%"
        mapStyle={darkMode ? "mapbox://styles/mapbox/dark-v10?optimize=true" : "mapbox://styles/mapbox/streets-v11?optimize=true"}
        onViewportChange={nextViewport => setViewport(nextViewport)}
        mapboxApiAccessToken="pk.eyJ1IjoiYXJzZW5pa2tpIiwiYSI6ImNraGIzaW51cjAyOXQyem1peHRpaHR3cHUifQ.axEOvzZzc4nm9T_CWdoWAQ"
        reuseMaps={true}
      >
        <GeolocateControl
            style={geolocateStyle}
            positionOptions={{enableHighAccuracy: true}}
            trackUserLocation={true}
            onViewportChange={newViewport => updateViewport(newViewport)}
        />
        <VenueMarkers venues={venues} />
      </ReactMapGL>
      <NavBar darkMode={darkMode} handleToggleDarkMode={handleToggleDarkMode} />
    </div>
  )
}