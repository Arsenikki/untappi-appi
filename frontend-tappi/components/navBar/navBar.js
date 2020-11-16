import { useState } from 'react';
import Search from './search';
import InfoScreen from '../../components/infoScreen'
import NavButton from "../../components/navBar/navButton";
import infoImage from "../../assets/information.svg";
import gpsImage from "../../assets/gps.svg";

export default function NavBar({darkMode, handleToggleDarkMode}) {
  const [showInfo, setShowInfo] = useState(false)

  const handleShowInfo = () => {
    console.log("info nappii painettu")
    setShowInfo(!showInfo)
  };

  return (
    <div className="w-screen absolute pointer-events-none z-20">
      <div className="pointer-events-none w-screen h-auto flex justify-center z-20">
        <Search />
      </div>
      <div className="pointer-events-auto absolute right-0 top-0 flex flex-col justify-end m-2 z-20">
        <NavButton icon={infoImage} handleClick={handleShowInfo} />
      </div>
      <InfoScreen showInfo={showInfo} handleShowInfo={handleShowInfo} darkMode={darkMode} handleToggleDarkMode={handleToggleDarkMode}/>
    </div>
  )
}