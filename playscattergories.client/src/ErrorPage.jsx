export default function ErrorPage({ error }) {
  console.log(error, typeof error);
  return (
    <>
      <main className="grid min-h-full place-items-center  px-6 py-24 sm:py-32 lg:px-8">
        <div className="bg-white shadow sm:rounded-lg">
          <div className="px-4 py-5 sm:p-6">
            <div className="text-center">
              {/* <p className="text-base font-semibold text-indigo-600">500</p> */}
              <h1 className="mt-4 text-balance text-5xl font-semibold tracking-tight text-gray-900 sm:text-7xl">
                Something went wrong.
              </h1>
              {/* <p className="mt-6 text-pretty text-lg font-medium text-gray-500 sm:text-xl/8">
                Sorry, we encountered an error.
              </p> */}
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
          </div>
        </div>
      </main>
    </>
  );
}
