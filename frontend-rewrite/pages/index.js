import Search from '../components/search'
import DynamicMapNoSSR from '../components/dynamicMapNoSSR'

export default function IndexPage() {
  return (
    <div>
      <div className="w-screen h-full absolute z-10">
        <DynamicMapNoSSR />
      </div>
      <div className="flex flex-col justify-between h-full w-full absolute p-2 overflow-hidden">
        <Search />
      </div>
    </div>
  )
}
