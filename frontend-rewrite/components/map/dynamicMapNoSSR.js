import dynamic from 'next/dynamic'

export default function DynamicMapNoSSR(props) {
  const Map = dynamic(
    () => import('./map'),
    { ssr: false, } 
  )
  return <Map {...props}/>
}