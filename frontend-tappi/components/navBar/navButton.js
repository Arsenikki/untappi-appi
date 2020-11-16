export default function NavButton({icon, handleClick}) {
  return (
    <button 
    className="flex w-16 h-16 justify-end bg-white rounded shadow hover:shadow-lg focus:outline-none p-3 z-10" 
      type="button"
      style={{ transition: "all .15s ease" }}
      onClick={() => handleClick()}
    >
      <img height={64} width={64} src={icon} />
    </button>
  )
}