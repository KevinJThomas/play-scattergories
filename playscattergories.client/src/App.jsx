import { useEffect, useState } from "react";
import * as signalR from "@microsoft/signalr";
import NamePage from "./NamePage";
import ErrorPage from "./ErrorPage";

function App() {
  const [gameStatus, setGameStatus] = useState("namePage");
  const [connection, setConnection] = useState();
  const [error, setError] = useState();

  useEffect(() => {
    const connection = new signalR.HubConnectionBuilder()
      .configureLogging(signalR.LogLevel.Debug) // add this for diagnostic clues
      .withUrl("http://localhost:5288/chatHub", {
        skipNegotiation: true, // skipNegotiation as we specify WebSockets
        transport: signalR.HttpTransportType.WebSockets, // force WebSocket transport
      })
      .build();

    setConnection(connection);
  }, []);

  useEffect(() => {
    if (!connection) {
      return;
    }

    connection
      .start()
      .then(() => {
        connection.invoke("SendMessage", "gotem chief").catch((err) => {
          setGameStatus("errorPage");
          setError(err);
        });
      })
      .catch((err) => console.error("Error connecting to hub:", err));

    connection.on("ReceiveMessage", (message) => {
      console.log("Received message:", message);
    });
  }, [connection]);

  return (
    <div className="flex items-center justify-center h-screen bg-gradient-to-r from-green-400 to-blue-500">
      {gameStatus === "namePage" && (
        <NamePage
          setGameStatus={setGameStatus}
          setError={setError}
          connection={connection}
        />
      )}
      {gameStatus === "errorPage" && <ErrorPage error={error} />}
    </div>
  );
}

export default App;
