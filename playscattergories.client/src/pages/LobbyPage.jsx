import Avatar from "../Avatar";
import Button from "../Button";
import Card from "../Card";
import Header from "../Header";

export default function Lobby({ players, playerId, connection }) {
  function startGame() {
    connection.invoke("NextRound");
  }

  return (
    <div>
      <Header>Lobby</Header>
      <Card>
        <ul role="list" className="divide-y divide-gray-200">
          {players.map((player, index) => (
            <li key={player.id} className="flex gap-x-4 py-1">
              <div className="flex w-full min-w-52 items-center">
                <Avatar name={player.name} />
                <p className="text-sm/6 text-gray-900">
                  {player.name}
                  {index === 0 && (
                    <span className="text-xs text-gray-600"> (Host)</span>
                  )}
                </p>
              </div>
            </li>
          ))}
        </ul>
        {playerId === players[0].id && (
          <Button fullWidth disabled={players.length <= 1} onClick={startGame}>
            Start Game
          </Button>
        )}
      </Card>
    </div>
  );
}
