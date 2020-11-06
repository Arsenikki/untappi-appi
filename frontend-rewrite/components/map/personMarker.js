import { useState, useEffect } from 'react';
import {Marker, Popup, useMapEvents } from 'react-leaflet'
import Leaflet from "leaflet";
import 'leaflet/dist/leaflet.css'

import student from "../../assets/student.svg";

export default function PersonMarker() {
  const [position, setPosition] = useState(null)
  const map = useMapEvents({
    click() {
      map.locate({watch: true})
    },
    locationfound(e) {
      setPosition(e.latlng)
      map.flyTo(e.latlng, 12)
    },
  })

  let personIcon = Leaflet.icon({
    iconUrl: student,
    iconSize: [35, 35],
    iconAnchor: [12, 36],
    popupAnchor: [0, -25]
  });

  return position === null ? null : (
    <Marker icon={personIcon} position={position}>
      <Popup>You are here</Popup>
    </Marker>
  )  
}