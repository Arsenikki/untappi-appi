export default function Search() {
  return (
    <section className="w-7/12 bg-indigo-dark max-w-md mx-auto z-10">
      <input
        className="w-full h-16 px-8 rounded focus:outline-none focus:shadow-outline text-xl shadow-md"
        type="search"
        placeholder="Search beer or venue... (TBD)"
      />
    </section>
  )
}