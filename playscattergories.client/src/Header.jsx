export default function Header({ children }) {
  return (
    <h1 className="pb-2 text-5xl font-extrabold uppercase text-green-300">
      {children}
    </h1>
  );
}
