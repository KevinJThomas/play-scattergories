export default function Header({ children }) {
  return (
    <h1 className="pb-2 text-3xl font-extrabold uppercase text-green-300 md:text-5xl">
      {children}
    </h1>
  );
}
