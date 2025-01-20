import { useEffect, useState } from "react";
import * as signalR from "@microsoft/signalr";
import NamePage from "./pages/NamePage";
import ErrorPage from "./pages/ErrorPage";
import LobbyPage from "./pages/LobbyPage";

function App() {
  const [gameStatus, setGameStatus] = useState("namePage");
  const [connection, setConnection] = useState();
  const [error, setError] = useState();
  const [players, setPlayers] = useState([]);
  const [playerId, setPlayerId] = useState("");

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
      .catch((err) => console.error("Error connecting to hub:", err));

    connection.on("LobbyUpdated", (players, playerId) => {
      if (playerId) {
        setGameStatus("lobbyPage");
        setPlayerId(playerId);
      }
      setPlayers(players);
    });
  }, [connection]);

  console.log(connection);

  return (
    <div className="flex h-screen items-center justify-center bg-gradient-to-r from-green-400 to-blue-500">
      {gameStatus === "namePage" && (
        <NamePage
          setGameStatus={setGameStatus}
          setError={setError}
          connection={connection}
        />
      )}
      {gameStatus === "errorPage" && <ErrorPage error={error} />}
      {gameStatus === "lobbyPage" && (
        <LobbyPage players={players} playerId={playerId} />
      )}
    </div>
  );
}

export default App;
