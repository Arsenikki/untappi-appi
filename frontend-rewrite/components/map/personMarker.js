import React, { useState, useEffect } from 'react';
import {Marker, Popup, useMapEvents } from 'react-leaflet'
import Leaflet from "leaflet";
import 'leaflet/dist/leaflet.css'

import student from "../../assets/student.svg";

export default function PersonMarker() {
  const [userLocation, setUserLocation] = useState(null)

  useEffect(() => {
    console.log("hommattii lokaatio")
    if (navigator.geolocation) {
      navigator.geolocation.watchPosition(geoLocation => {
        if (userLocation === null) {
          console.log("ajithjotaihjoitjoihato", userLocation)
          map.flyTo([geoLocation.coords.latitude, geoLocation.coords.longitude], 12)
        }
        handleUserLocationChange(geoLocation)
      });
    } else {
      console.log("Your browser doesn't support HTML5 :S");
    }
  }, []);
  
  const map = useMapEvents({})

  const handleUserLocationChange = geoLocation => {
    const { latitude, longitude } = geoLocation.coords;
    console.log("haha,", { lat: latitude, lng: longitude })
    setUserLocation({ lat: latitude, lng: longitude });
  };


  let personIcon = Leaflet.icon({
    iconUrl: student,
    iconSize: [35, 35],
    iconAnchor: [12, 36],
    popupAnchor: [0, -25]
  });

  return userLocation === null ? null : (
    <Marker icon={personIcon} position={userLocation}>
      <Popup>You are here</Popup>
    </Marker>
  )  
}