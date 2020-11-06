import dynamic from 'next/dynamic'

export default function DynamicMapNoSSR() {
  const Map = dynamic(
    () => import('./map'),
    { ssr: false }
  )
  return <Map />
}