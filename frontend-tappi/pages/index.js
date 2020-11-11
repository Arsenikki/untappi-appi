import { useState } from 'react';
import DynamicMapNoSSR from '../components/map/dynamicMapNoSSR';
import NavBar from '../components/navBar/navBar'


export default function IndexPage({defaultVenues}) {
  const [venues, setVenues] = useState(defaultVenues);
  const [allowLocation, setAllowLocation] = useState(false)

  const handleAllowLocation = () => {
    console.log("gps nappii painettu")
    var asd = Math.random() * 10
    console.log(asd)
    setAllowLocation(asd > 5 ? true : false)
  };

  return (
    <div className="flex flex-col">
      <div className="w-screen h-full absolute z-0">
        <DynamicMapNoSSR venues={venues} />
      </div>
      <div className="flex h-auto w-full p-2 overflow-hidden z-10">
        <NavBar handleAllowLocation={handleAllowLocation}/>
      </div>
    </div>
  )
}

// Fetch example packages from API at build time.
export async function getServerSideProps() {
  console.log('Getting venues with beers..');
  const res = await fetch('http://localhost:5000/beer');
  const defaultVenues = await res.json();

  return {
    props: {
      defaultVenues,
    },
  };
}