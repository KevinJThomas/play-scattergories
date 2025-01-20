import Avatar from "../Avatar";
import Button from "../Button";
import Card from "../Card";
import Header from "../Header";

export default function ScorePage({
  players,
  playerId,
  gameState,
  connection,
}) {
  const nextRoundNumber = gameState.roundNumber + 1;

  function nextRound() {
    connection.invoke("NextRound");
  }
  return (
    <div>
      <Header>{nextRoundNumber > 3 && "Final "}Score</Header>
      <Card>
        <ul role="list" className="divide-y divide-gray-200">
          {players.map((player, index) => (
            <li key={player.id} className="flex gap-x-4 py-1">
              <div className="flex min-w-52 items-center justify-between">
                <div className="flex items-center">
                  <Avatar name={player.name} />
                  <p className="text-sm/6 text-gray-900">
                    {player.name}
                    {index === 0 && (
                      <span className="text-xs text-gray-600"> (Host)</span>
                    )}
                  </p>
                </div>
                <p>
                  {player.points}{" "}
                  <span className="text-xs font-light text-gray-600">pts</span>
                </p>
              </div>
            </li>
          ))}
        </ul>
        {playerId === players[0].id && nextRoundNumber <= 3 && (
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
