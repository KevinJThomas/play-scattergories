import { useEffect, useState } from "react";
import * as signalR from "@microsoft/signalr";
import NamePage from "./pages/NamePage";
import ErrorPage from "./pages/ErrorPage";
import LobbyPage from "./pages/LobbyPage";
import GamePage from "./pages/GamePage";
import ScorePage from "./pages/ScorePage";
import VotingPage from "./pages/VotingPage";
import ChatSideBar from "./ChatSideBar";
import ChatButton from "./ChatButton";

function App() {
  const [gameStatus, setGameStatus] = useState("namePage");
  const [connection, setConnection] = useState();
  const [error, setError] = useState();
  const [players, setPlayers] = useState([]);
  const [playerId, setPlayerId] = useState("");
  const [hostId, setHostId] = useState("");
  const [gameState, setGameState] = useState();
  const [chatOpen, setChatOpen] = useState(false);
  const [name, setName] = useState("");

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
    <div className="flex min-h-screen justify-center overflow-auto bg-blue-900 py-4 sm:py-12 md:bg-gradient-to-r md:from-green-700 md:to-blue-800">
      <div>
        {gameStatus === "namePage" && (
          <NamePage
            setGameStatus={setGameStatus}
            setError={setError}
            connection={connection}
            name={name}
            setName={setName}
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
        {gameStatus !== "namePage" && gameStatus !== "errorPage" && (
          <>
            <ChatSideBar
              name={name}
              open={chatOpen}
              setOpen={setChatOpen}
              connection={connection}
            />
            <ChatButton onClick={() => setChatOpen(true)} />
          </>
        )}
      </div>
    </div>
  );
}

export default App;
