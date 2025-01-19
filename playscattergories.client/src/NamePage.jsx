import { useState } from "react";

export default function NamePage({ connection, setGameStatus, setError }) {
  const [name, setName] = useState("");

  function onSubmit(e) {
    e.preventDefault();

    connection.invoke("PlayerJoined", name).catch((err) => {
      setGameStatus("errorPage");
      setError(err);
    });
  }
  return (
    <div>
      <h1 className="text-5xl font-extrabold text-green-300 pb-2">
        SCATTEGORIES
      </h1>
      <div className="bg-white shadow sm:rounded-lg">
        <div className="px-4 py-5 sm:p-6">
          <h3 className="text-base font-semibold text-gray-900">
            Enter your name.
          </h3>
          <form className="mt-5 sm:flex sm:items-center" onSubmit={onSubmit}>
            <div className="w-full sm:max-w-xs">
              <input
                id="name"
                name="name"
                placeholder="Johnny"
                autoFocus
                required
                onChange={(e) => setName(e.target.value)}
                className="block w-full rounded-md bg-white px-3 py-1.5 text-base text-gray-900 outline outline-1 -outline-offset-1 outline-gray-300 placeholder:text-gray-400 focus:outline focus:outline-2 focus:-outline-offset-2 focus:outline-indigo-600 sm:text-sm/6"
              />
            </div>
            <button
              type="submit"
              className="mt-3 inline-flex w-full items-center justify-center rounded-md bg-pink-600 px-3 py-2 text-sm font-semibold text-white shadow-sm hover:bg-indigo-500 focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-indigo-600 sm:ml-3 sm:mt-0 sm:w-auto"
            >
              Play
            </button>
          </form>
        </div>
      </div>
    </div>
  );
}
