import { useState } from 'react';
import dynamic from 'next/dynamic';

function DynamicMapWithNoSSR (props) {
  const Map = dynamic(
    () => import('../components/map/map'),
    { ssr: false, } 
  )
  return <Map {...props}/>
} 

export default function IndexPage({defaultVenues}) {
  const [venues, setVenues] = useState(defaultVenues);

  return (
    <DynamicMapWithNoSSR venues={venues} />
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