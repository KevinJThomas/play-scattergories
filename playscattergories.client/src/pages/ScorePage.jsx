import Avatar from "../Avatar";
import Button from "../Button";
import Card from "../Card";
import Header from "../Header";

export default function ScorePage({
  players,
  playerId,
  gameState,
  connection,
  hostId,
}) {
  const nextRoundNumber = gameState.roundNumber + 1;

  function nextRound() {
    connection.invoke("NextRound");
  }

  const header =
    nextRoundNumber > 3
      ? "Final Score"
      : "Round " + gameState.roundNumber + " Score";

  return (
    <div>
      <Header>{header}</Header>
      <Card>
        <p className="pb-2 text-sm">
          <span className="font-bold">Vetoed:</span>{" "}
          {gameState.bannedWords.join(", ")}
        </p>
        <ul role="list" className="divide-y divide-gray-200">
          {players
            .sort(
              (playerA, playerB) => playerB.totalPoints - playerA.totalPoints,
            )
            .map((player) => (
              <li key={player.id} className="flex gap-x-4 py-1">
                <div className="flex w-full min-w-52 items-center justify-between">
                  <div className="flex items-center">
                    <Avatar name={player.name} />
                    <p className="text-sm text-gray-900">
                      {player.name}
                      {player.id === hostId && (
                        <span className="text-xs text-gray-600"> (Host)</span>
                      )}
                    </p>
                  </div>
                  <p>
                    <span className="pr-2 text-sm text-green-500 sm:pr-8">
                      +{player.roundPoints}
                      <span className="text-xs font-light">pts</span>
                    </span>
                    {player.totalPoints}{" "}
                    <span className="text-xs font-light text-gray-600">
                      pts
                    </span>
                  </p>
                </div>
              </li>
            ))}
        </ul>
        {playerId === hostId && nextRoundNumber <= 3 && (
          <Button fullWidth disabled={players.length <= 1} onClick={nextRound}>
            Start Round #{nextRoundNumber}
          </Button>
        )}
        {nextRoundNumber > 3 && (
          <Button fullWidth onClick={() => window.location.reload()}>
            Play Again
          </Button>
        )}
      </Card>
    </div>
  );
}
