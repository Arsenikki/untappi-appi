export default function NavButton({icon, handleButtonAction}) {
  return (
    <img src={icon} onClick={() => handleButtonAction()} className="w-16 h-16 bg-white  rounded shadow-md m-auto p-3 z-10"/>
  )
}