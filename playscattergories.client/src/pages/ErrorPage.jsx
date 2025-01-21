import Card from "../Card";

export default function ErrorPage({ error }) {
  return (
    <Card>
      <div className="text-center">
        <h1 className="mt-4 text-balance text-5xl font-semibold tracking-tight text-gray-900 sm:text-7xl">
          Something went wrong.
        </h1>

        <p className="mt-4">{error.name}</p>
        <p>{error.message}</p>
        <code>{error.stack}</code>
        <div className="mt-10 flex items-center justify-center gap-x-6">
          <a
            href="/"
            className="rounded-md bg-indigo-600 px-3.5 py-2.5 text-sm font-semibold text-white shadow-sm hover:bg-indigo-500 focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-indigo-600"
          >
            Retry
          </a>
        </div>
      </div>
    </Card>
  );
}
