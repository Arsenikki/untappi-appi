import React, { useState } from 'react';
import dynamic from 'next/dynamic';
import { GetServerSideProps } from 'next';

interface Venue {
  address: string,
  category: string,
  lat: number,
  lng: number,
  venueID: number,
  venueName: string,
}

interface Props {
  venues: Venue[];
}

interface PreloadedVenues {
  preloadedVenues: Venue[];
}

const DynamicMapWithNoSSR: React.FC<Props> = (props): JSX.Element => {
  const Map = dynamic(
    () => import('../components/map/map'),
    { ssr: false },
  );
  return <Map {...props} />;
};

const IndexPage: React.FC<PreloadedVenues> = ({ preloadedVenues }): JSX.Element => {
  const [venues, setVenues] = useState(preloadedVenues);

  return (
    <DynamicMapWithNoSSR venues={venues} />
  );
};

// Fetch example packages from API at build time.
export const getServerSideProps: GetServerSideProps = async () => {
  console.log('Getting venues with beers..');
  const res = await fetch('http://localhost:5000/beer');
  const preloadedVenues = await res.json();

  return {
    props: {
      preloadedVenues,
    },
  };
};

export default IndexPage;
