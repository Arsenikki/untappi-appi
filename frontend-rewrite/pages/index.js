import { useState } from 'react';
import Search from '../components/search';
import NavButton from '../components/navButton';
import DynamicMapNoSSR from '../components/map/dynamicMapNoSSR';
import infoImage from "../assets/information.svg";
import gpsImage from "../assets/gps.svg"

export default function IndexPage({defaultVenues}) {
  const [venues, setVenues] = useState(defaultVenues);

  return (
    <div>
      <div className="w-screen h-full absolute z-10">
        <DynamicMapNoSSR venues={venues}/>
      </div>
      <div className="flex flex-row justify-between h-auto w-full absolute p-2 overflow-hidden">
        <NavButton icon={infoImage} />
        <Search />
        <NavButton icon={gpsImage} />
      </div>
    </div>
  )
}

// Fetch example packages from API at build time.
export async function getStaticProps() {
  console.log('Getting venues with beers..');
  const res = await fetch('http://localhost:5000/beer');
  const defaultVenues = await res.json();

  return {
    props: {
      defaultVenues,
    },
  };
}