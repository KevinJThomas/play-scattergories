import Button from "../Button";
import Card from "../Card";
import Header from "../Header";
import { useState } from "react";
import useInterval from "use-interval";
import getTimeTill from "../util";
import clsx from "clsx";

export default function VotingPage({ connection, gameState, players }) {
  const [voteTimeRemaining, setVoteTimeRemaining] = useState("3:00");
  const [hasVoted, setHasVoted] = useState(false);
  const [bannedWords, setBannedWords] = useState([]);

  function submitVote() {
    setHasVoted(true);
    connection.invoke("VoteSubmitted", bannedWords);
  }

  function clickCell(value, isValid) {
    if (!isValid || voteTimeRemaining === "0:00" || hasVoted) {
      return;
    }

    const isBanned = bannedWords.includes(value);

    if (isBanned) {
      setBannedWords((prev) => prev.filter((word) => word !== value));
    } else {
      setBannedWords((prev) => [...prev, value]);
    }
  }

  function renderCell({ isValid, value } = { isValid: true, value: "" }) {
    const isBanned = bannedWords.includes(value);

    return (
      <span
        onClick={() => clickCell(value, isValid)}
        className={clsx(
          isValid &&
            voteTimeRemaining !== "0:00" &&
            !hasVoted &&
            "cursor-pointer",
          !isValid && "text-red-950 line-through",
          isBanned && "line-through",
        )}
      >
        {value}
      </span>
    );
  }

  useInterval(() => {
    const timeLeft = getTimeTill(gameState.submitNextVoteTimeLimit);

    setVoteTimeRemaining(timeLeft);

    if (timeLeft === "0:00" && !hasVoted) {
      submitVote();
    }
  }, 1000);

  return (
    <div className="max-w-full px-5">
      <Header>VOTE - {voteTimeRemaining}</Header>
      <Card>
        <div className="-mx-4 -my-2 overflow-x-auto sm:-mx-6 lg:-mx-8">
          <div className="inline-block min-w-full py-1 align-middle sm:px-6 lg:px-8">
            <table className="min-w-full divide-y divide-gray-300">
              <thead>
                <tr>
                  <th
                    scope="col"
                    className="px-1 py-1 text-left text-xs font-semibold uppercase text-gray-900 sm:px-3 sm:text-sm"
                  >
                    LETTER {gameState.letter}
                  </th>
                  {players.map((player) => (
                    <th
                      scope="col"
                      key={player.id}
                      className="px-1 py-1 text-left text-xs font-semibold text-gray-900 sm:px-3 sm:text-sm"
                    >
                      {player.name}
                    </th>
                  ))}
                </tr>
              </thead>
              <tbody className="divide-y divide-gray-200">
                {gameState.categoryCard.categories.map(
                  (category, categoryIndex) => (
                    <tr key={category}>
                      <td className="whitespace-nowrap px-1 py-1 text-xs text-gray-500 sm:px-3 sm:text-sm">
                        {category}
                      </td>
                      {players.map((player, wordIndex) => (
                        <td
                          key={wordIndex}
                          className="whitespace-nowrap px-1 py-1 text-xs text-gray-500 sm:px-3 sm:text-sm"
                        >
                          {renderCell(
                            player.scoreSheet[gameState.roundNumber - 1][
                              categoryIndex
                            ],
                          )}
                        </td>
                      ))}
                    </tr>
                  ),
                )}
              </tbody>
            </table>
          </div>
        </div>
      </Card>
      <Button fullWidth onClick={submitVote} disabled={hasVoted}>
        Submit
      </Button>
    </div>
  );
}
