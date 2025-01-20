export default function getTimeTill(unixTime) {
  const timeLeft = Math.max(Math.round((unixTime - Date.now()) / 1000), 0);
  const secondsLeft = timeLeft % 60;
  const minutesLeft = Math.floor(timeLeft / 60);
  const formattedSecondsLeft =
    secondsLeft < 10 ? "0" + secondsLeft : secondsLeft;

  return minutesLeft + ":" + formattedSecondsLeft;
}
