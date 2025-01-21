import Button from "../Button";
import Card from "../Card";
import Header from "../Header";
import { useState } from "react";
import useInterval from "use-interval";
import getTimeTill from "../util";

// const gameState = {
//   letter: "c",
//   roundNumber: 1,
//   submitNextRoundDateTime: Date.now(),
//   categoryCard: {
//     categories: [
//       "A boy’s name",
//       "Countries",
//       "Flowers",
//       "Things that you shout",
//       "Excuses for being late",
//       "Pet peeves",
//       "Ice cream flavors",
//       "Fried foods",
//       "Bodies of water",
//       "Halloween costumes",
//       "Places to go on a date",
//       "Nicknames",
//     ],
//   },
//   unusedCategoryCards: [],
// };

export default function GamePage({ gameState, connection }) {
  const [roundTimeRemaining, setRoundTimeRemaining] = useState("3:00");
  const [hasSubmitted, setHasSubmitted] = useState(false);

  function submitWords() {
    setHasSubmitted(true);
    connection.invoke("WordsSubmitted", words);
  }

  useInterval(() => {
    const timeLeft = getTimeTill(gameState.submitNextRoundTimeLimit);

    setRoundTimeRemaining(timeLeft);

    if (timeLeft === "0:00" && !hasSubmitted) {
      submitWords();
    }
  }, 1000);

  const [words, setWords] = useState([
    "",
    "",
    "",
    "",
    "",
    "",
    "",
    "",
    "",
    "",
    "",
    "",
  ]);

  function setWord(text, i) {
    const newWords = words.map((word, index) => {
      if (index === i) {
        return text;
      } else {
        return word;
      }
    });

    setWords(newWords);
  }

  return (
    <div>
      <Header>
        LETTER {gameState.letter} - {roundTimeRemaining}
      </Header>
      <Card>
        <div className="-mx-4 -my-2 overflow-x-auto sm:-mx-6 lg:-mx-8">
          <div className="inline-block min-w-full py-2 align-middle sm:px-6 lg:px-8">
            <table className="min-w-full divide-y divide-gray-300">
              <tbody className="divide-y divide-gray-200">
                {gameState.categoryCard.categories.map((category, index) => (
                  <tr key={category}>
                    <td className="whitespace-nowrap py-1 pl-2 text-xs font-medium text-gray-900 sm:pl-0 sm:pr-3 sm:text-sm">
                      {index + 1 + "."}
                    </td>
                    <td className="whitespace-nowrap py-1 text-sm text-gray-500 sm:px-3">
                      <input
                        disabled={hasSubmitted}
                        spellCheck={true}
                        onChange={(e) => setWord(e.target.value, index)}
                        className="block w-full rounded-md bg-white px-3 py-0.5 text-base text-gray-900 outline outline-1 -outline-offset-1 outline-gray-300 placeholder:text-gray-400 focus:outline focus:outline-2 focus:-outline-offset-2 focus:outline-indigo-600 sm:text-sm/6"
                      />
                    </td>
                    <td className="whitespace-nowrap py-1 text-xs text-gray-500 sm:px-3 sm:text-sm">
                      {category}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      </Card>

      {import.meta.env.MODE === "development" && (
        <Button fullWidth onClick={submitWords} disabled={hasSubmitted}>
          Submit
        </Button>
      )}
    </div>
  );
}
