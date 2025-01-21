import { useEffect, useState } from "react";
import * as signalR from "@microsoft/signalr";
import NamePage from "./pages/NamePage";
import ErrorPage from "./pages/ErrorPage";
import LobbyPage from "./pages/LobbyPage";
import GamePage from "./pages/GamePage";
import ScorePage from "./pages/ScorePage";
import VotingPage from "./pages/VotingPage";

function App() {
  const [gameStatus, setGameStatus] = useState("namePage");
  const [connection, setConnection] = useState();
  const [error, setError] = useState();
  const [players, setPlayers] = useState([]);
  const [playerId, setPlayerId] = useState("");
  const [hostId, setHostId] = useState("");
  const [gameState, setGameState] = useState();

  useEffect(() => {
    const url =
      import.meta.env.MODE === "development"
        ? "http://localhost:5288/chatHub"
        : "/chatHub";

    const connection = new signalR.HubConnectionBuilder()
      .configureLogging(signalR.LogLevel.Debug) // add this for diagnostic clues
      .withUrl(url, {
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

    connection.start().catch((error) => {
      setError(error);
      setGameStatus("errorPage");
    });

    connection.on("LobbyUpdated", (lobby, playerId) => {
      if (playerId) {
        setGameStatus("lobbyPage");
        setPlayerId(playerId);
      }
      setPlayers(lobby.players);
      setHostId(lobby.hostId);
    });

    connection.on("GameError", (error) => {
      setError(error);
      setGameStatus("errorPage");
    });

    connection.on("ConfirmNextRound", (lobby) => {
      setGameStatus("gamePage");
      setGameState(lobby.gameState);
    });

    connection.on("RoundComplete", (lobby) => {
      setGameStatus("votePage");
      setGameState(lobby.gameState);
      setPlayers(lobby.players);
    });

    connection.on("VoteComplete", (lobby) => {
      setGameStatus("scorePage");
      setGameState(lobby.gameState);
      setPlayers(lobby.players);
    });
  }, [connection]);

  return (
    <div className="flex min-h-screen justify-center overflow-auto bg-gradient-to-r from-green-400 to-blue-500 py-4 sm:py-12">
      {gameStatus === "namePage" && (
        <NamePage
          setGameStatus={setGameStatus}
          setError={setError}
          connection={connection}
        />
      )}
      {gameStatus === "errorPage" && <ErrorPage error={error} />}
      {gameStatus === "lobbyPage" && (
        <LobbyPage
          players={players}
          playerId={playerId}
          connection={connection}
          hostId={hostId}
        />
      )}
      {gameStatus === "gamePage" && (
        <GamePage gameState={gameState} connection={connection} />
      )}
      {gameStatus === "scorePage" && (
        <ScorePage
          players={players}
          playerId={playerId}
          gameState={gameState}
          connection={connection}
          hostId={hostId}
        />
      )}
      {gameStatus === "votePage" && (
        <VotingPage
          gameState={gameState}
          connection={connection}
          players={players}
        />
      )}
    </div>
  );
}

export default App;
