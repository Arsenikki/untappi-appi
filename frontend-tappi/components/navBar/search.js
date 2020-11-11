export default function Search() {
  return (
    <section className="justify-center w-5/12 max-w-md z-10">
      <input
        className="w-full h-16 px-6 rounded focus:outline-none focus:shadow-outline text-xl shadow-md"
        type="search"
        placeholder="Search beer or venue... (TBD)"
      />
    </section>
  )
}