export default function Search() {
  return (
    <section className="pointer-events-auto h-16 justify-center w-5/12 m-2 max-w-md z-10">
      <input
        className="w-full h-full px-6 rounded focus:outline-none focus:shadow-outline text-xl shadow-md"
        type="search"
        placeholder="Search beer or venue... (TBD)"
      />
    </section>
  )
}