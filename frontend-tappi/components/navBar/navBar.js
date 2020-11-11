import { useState } from 'react';
import Search from './search';
import InfoScreen from '../../components/infoScreen'
import NavButton from "../../components/navBar/navButton";
import infoImage from "../../assets/information.svg";
import gpsImage from "../../assets/gps.svg";

export default function NavBar({handleAllowLocation}) {
  const [showInfo, setShowInfo] = useState(false)

  const handleShowInfo = () => {
    console.log("info nappii painettu")
    setShowInfo(!showInfo)
    console.log(showInfo)
  };

  return (
    <div className="w-full h-full">
      <div className="flex justify-center mx-2 px-4">
        <Search />
      </div>
      <div className="absolute right-0 top-0 flex flex-col justify-end m-2">
        <NavButton icon={infoImage} handleClick={handleShowInfo} />
      </div>
      <div className="absolute right-0 m-2">
        <NavButton icon={gpsImage} handleClick={handleAllowLocation} />
      </div>
      <InfoScreen showInfo={showInfo} handleShowInfo={handleShowInfo}/>
    </div>
  )
}