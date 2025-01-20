import Button from "../Button";
import Card from "../Card";
import Header from "../Header";
import { useState } from "react";
import useInterval from "use-interval";
import getTimeTill from "../util";
import clsx from "clsx";

// const players = [
//   {
//     name: "John",
//     id: 1,
//     scoreSheet: {
//       roundOne: [
//         { value: "add", isValid: false },
//         { value: "blood", isValid: true },
//         { value: "task", isValid: true },
//         { value: "met", isValid: true },
//         { value: "shirt", isValid: true },
//         { value: "cow", isValid: true },
//         { value: "have", isValid: true },
//         { value: "love", isValid: false },
//         { value: "love", isValid: true },
//         { value: "love", isValid: true },
//         { value: "love", isValid: true },
//         { value: "love", isValid: true },
//         { value: "love", isValid: false },
//         { value: "love", isValid: true },
//         { value: "love", isValid: true },
//         { value: "love", isValid: true },
//       ],
//     },
//   },
//   {
//     name: "Mary",
//     id: 2,
//     scoreSheet: {
//       roundOne: [
//         { value: "add", isValid: true },
//         { value: "blood", isValid: true },
//         { value: "task", isValid: true },
//         { value: "met", isValid: true },
//         { value: "shirt", isValid: true },
//         { value: "cow", isValid: true },
//         { value: "have", isValid: true },
//         { value: "love", isValid: true },
//         { value: "love", isValid: true },
//         { value: "love", isValid: true },
//         { value: "love", isValid: true },
//         { value: "love", isValid: true },
//       ],
//     },
//   },
//   {
//     name: "Lee",
//     id: 3,
//     scoreSheet: {
//       roundOne: [
//         { value: "add", isValid: true },
//         { value: "blood", isValid: true },
//         { value: "task", isValid: true },
//         { value: "met", isValid: true },
//         { value: "shirt", isValid: true },
//         { value: "cow", isValid: true },
//         { value: "have", isValid: true },
//         { value: "love", isValid: true },
//         { value: "love", isValid: true },
//         { value: "love", isValid: true },
//         { value: "love", isValid: true },
//         { value: "love", isValid: true },
//       ],
//     },
//   },
//   {
//     name: "Bob",
//     id: 4,
//     scoreSheet: {
//       roundOne: [
//         { value: "add", isValid: true },
//         { value: "blood", isValid: true },
//         { value: "task", isValid: true },
//         { value: "met", isValid: true },
//         { value: "shirt", isValid: true },
//         { value: "cow", isValid: true },
//         { value: "have", isValid: true },
//         { value: "love", isValid: true },
//         { value: "love", isValid: true },
//         { value: "love", isValid: true },
//         { value: "love", isValid: true },
//         { value: "love", isValid: true },
//       ],
//     },
//   },
//   {
//     name: "Frank",
//     id: 5,
//     scoreSheet: {
//       roundOne: [
//         { value: "add", isValid: true },
//         { value: "blood", isValid: true },
//         { value: "task", isValid: true },
//         { value: "met", isValid: true },
//         { value: "shirt", isValid: true },
//         { value: "cow", isValid: true },
//         { value: "have", isValid: true },
//         { value: "love", isValid: true },
//         { value: "love", isValid: true },
//         { value: "love", isValid: true },
//         { value: "love", isValid: true },
//         { value: "love", isValid: true },
//       ],
//     },
//   },
//   {
//     name: "Sam",
//     id: 6,
//     scoreSheet: {
//       roundOne: [
//         { value: "add", isValid: true },
//         { value: "blood", isValid: true },
//         { value: "task", isValid: true },
//         { value: "met", isValid: true },
//         { value: "shirt", isValid: true },
//         { value: "cow", isValid: true },
//         { value: "have", isValid: true },
//         { value: "love", isValid: true },
//         { value: "love", isValid: true },
//         { value: "love", isValid: true },
//         { value: "love", isValid: true },
//         { value: "love", isValid: true },
//       ],
//     },
//   },
//   {
//     name: "Liz",
//     id: 7,
//     scoreSheet: {
//       roundOne: [
//         { value: "add", isValid: true },
//         { value: "blood", isValid: true },
//         { value: "task", isValid: true },
//         { value: "met", isValid: true },
//         { value: "shirt", isValid: true },
//         { value: "cow", isValid: true },
//         { value: "have", isValid: true },
//         { value: "love", isValid: true },
//         { value: "love", isValid: true },
//         { value: "love", isValid: true },
//         { value: "love", isValid: true },
//         { value: "love", isValid: true },
//       ],
//     },
//   },
//   {
//     name: "Jonny",
//     id: 8,
//     scoreSheet: {
//       roundOne: [
//         { value: "add", isValid: true },
//         { value: "blood", isValid: true },
//         { value: "task", isValid: true },
//         { value: "met", isValid: true },
//         { value: "shirt", isValid: true },
//         { value: "cow", isValid: true },
//         { value: "have", isValid: true },
//         { value: "love", isValid: true },
//         { value: "love", isValid: true },
//         { value: "love", isValid: true },
//         { value: "love", isValid: true },
//         { value: "love", isValid: true },
//       ],
//     },
//   },
// ];

// const gameStateDefault = {
//   letter: "C",
//   submitNextVoteTimeLimit: Date.now() + 30000,
//   categoryCard: {
//     categories: [
//       "A BOY'S NAME",
//       "U.S. CITIES",
//       "THINGS THAT ARE COLD",
//       "SCHOOL SUPPLIES",
//       "PRO SPORTS TEAMS",
//       "INSECTS",
//       "BREAKFAST FOODS",
//       "FURNITURE",
//       "TV SHOWS",
//       "THINGS FOUND IN THE OCEAN",
//       "PRESIDENTS",
//       "PRODUCT NAMES",
//     ],
//   },
// };

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

  function renderCell({ isValid, value }) {
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
          <div className="inline-block min-w-full py-2 align-middle sm:px-6 lg:px-8">
            <table className="min-w-full divide-y divide-gray-300">
              <thead>
                <tr>
                  <th
                    scope="col"
                    className="px-3 py-2 text-left text-5xl font-bold uppercase text-gray-900"
                  ></th>
                  {players.map((player) => (
                    <th
                      scope="col"
                      key={player.id}
                      className="px-3 py-2 text-left text-sm font-semibold text-gray-900"
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
                      <td className="whitespace-nowrap px-3 py-2 text-sm text-gray-500">
                        {category}
                      </td>
                      {players.map((player, wordIndex) => (
                        <td
                          key={wordIndex}
                          className="whitespace-nowrap px-3 py-2 text-sm text-gray-500"
                        >
                          {renderCell(
                            player.scoreSheet.roundOne[categoryIndex],
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
