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
  const [chatOpen, setOpen] = useState(false);
  const [name, setName] = useState("");
  const [hasUnreadMessage, setHasUnreadMessage] = useState(false);
  const [messages, setMessages] = useState([]);

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

    connection.start().catch((error) => {
      setError(error);
      setGameStatus("errorPage");
    });

    setConnection(connection);
  }, []);

  useEffect(() => {
    if (!connection) {
      return;
    }

    connection.on("LobbyUpdated", (lobby, playerId) => {
      if (playerId) {
        setGameStatus("lobbyPage");
        setPlayerId(playerId);
      }
      setPlayers(lobby.players);
      setHostId(lobby.hostId);
      setMessages(lobby.messages);
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

    connection.on("ChatReceived", (message) => {
      setMessages((prev) => [...prev, message]);
      setHasUnreadMessage(true);
    });

    return () => {
      connection.off("LobbyUpdated");
      connection.off("GameError");
      connection.off("ConfirmNextRound");
      connection.off("RoundComplete");
      connection.off("VoteComplete");
      connection.off("ChatReceived");
    };
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
              setOpen={setOpen}
              connection={connection}
              setHasUnreadMessage={setHasUnreadMessage}
              messages={messages}
            />
            <ChatButton
              onClick={() => {
                setHasUnreadMessage(false);
                setOpen(true);
              }}
              hasUnreadMessage={hasUnreadMessage}
            />
          </>
        )}
      </div>
    </div>
  );
}

export default App;
