using PlayScattergories.Server.Helpers;
using PlayScattergories.Server.Models;
using PlayScattergories.Server.Models.Game;
using PlayScattergories.Server.Models.Player;

namespace PlayScattergories.Server.Services
{
    public static class GameService
    {
        private static Random _random = new Random();
        private static readonly List<string> _articles = ConfigurationHelper.config.GetSection("CategoryLists").GetChildren().Select(x => x.Value).ToList();

        #region public methods

        public static List<CategoryCard> GetAllCategoryCards()
        {
            var categoryLists = ConfigurationHelper.config.GetSection("CategoryLists").GetChildren().Select(x => x.Value).ToList();

            if (categoryLists != null && categoryLists.Any())
            {
                var cards = new List<CategoryCard>();
                foreach (var list in categoryLists)
                {
                    cards.Add(new CategoryCard
                    {
                        Categories = list.Split(',').ToList()
                    });
                }

                return cards;
            }

            return null;
        }

        public static (List<CategoryCard>, CategoryCard) ChooseNextCategoryCard(List<CategoryCard> categoryCards)
        {
            var index = _random.Next(categoryCards.Count);
            var nextCategoryCard = categoryCards[index];
            var newCardsList = categoryCards;
            newCardsList.RemoveAt(index);
            return (newCardsList, nextCategoryCard);
        }

        public static string GetLetter(List<string> usedLetters)
        {
            // Return a random lowercase letter a-z
            var character = (char)('a' + _random.Next(0, 26));
            if (usedLetters.Contains(character.ToString()))
            {
                // If it's a letter that has already been used this game, try again
                GetLetter(usedLetters);
            }

            return character.ToString();
        }

        public static ScoreSheet PopulateScoreSheet(List<string> words, ScoreSheet scoreSheet, int roundNumber)
        {
            switch (roundNumber)
            {
                case 1:
                    scoreSheet.RoundOne = words;
                    return scoreSheet;
                case 2:
                    scoreSheet.RoundTwo = words;
                    return scoreSheet;
                case 3:
                    scoreSheet.RoundThree = words;
                    return scoreSheet;
                default:
                    return scoreSheet;
            }
        }

        public static Lobby ScoreRound(Lobby lobby)
        {
            if (lobby == null || lobby.GameState == null || lobby.Players == null)
            {
                return null;
            }

            var hostsCurrentWords = GetPlayerScoreSheetByRound(lobby.Players[0].ScoreSheet, lobby.GameState.RoundNumber);

            if (hostsCurrentWords == null)
            {
                return null;
            }

            // Create array of lists with all submitted words
            var submittedWordsArray = PopulateSubmittedWordsArray(lobby, hostsCurrentWords.Count);

            // Create array of lists for all duplicate words
            var duplicateWordsArray = new List<string>[hostsCurrentWords.Count];

            // Loop through array and populate duplicates lists
            for (var i = 0; i < submittedWordsArray.Length; i++)
            {
                var duplicates = submittedWordsArray[i].GroupBy(x => x).Where(g => g.Count() > 1).Select(y => y.Key.ToLower()).ToList();
                if (duplicates == null)
                {
                    duplicateWordsArray[i] = new List<string>();
                }
                else
                {
                    duplicateWordsArray[i] = duplicates;
                }
            }

            // Loop through players and give out points for scoring words
            foreach (var player in lobby.Players)
            {
                player.RoundPoints = 0;
                // Get player words for the round
                var currentWords = GetPlayerScoreSheetByRound(player.ScoreSheet, lobby.GameState.RoundNumber);

                // Loop through words to score them
                for (var i = 0; i < currentWords.Count; i++)
                {
                    // If the word isn't null, and the duplicates array doesn't contain the word, and the word starts with the correct letter
                    if (!string.IsNullOrWhiteSpace(currentWords[i]))
                    {
                        var wordSplit = currentWords[i].Split(' ');
                        var wordToScore = string.Empty;

                        // Remove starting article if present
                        if (wordSplit.Length > 1 && _articles.Contains(wordSplit[0]))
                        {
                            var listWithoutArticle = new List<string>(wordSplit);
                            listWithoutArticle.RemoveAt(0);
                            wordSplit = listWithoutArticle.ToArray();
                        }

                        wordToScore = string.Join(" ", wordSplit);

                        if (!duplicateWordsArray[i].Contains(wordToScore.ToLower()) && wordToScore.Substring(0, 1).ToLower() == lobby.GameState.Letter.ToLower())
                        {
                            player.RoundPoints += 1;
                            player.TotalPoints += 1;

                            if (ConfigurationHelper.config.GetValue<bool>("App:ExtraPointsEnabled"))
                            {
                                // Check for multi word answers
                                if (wordSplit != null && wordSplit.Length > 1)
                                {
                                    // Start at index 1 here, because we've already given out one point for the starting letter
                                    for (var j = 1; j < wordSplit.Length; j++)
                                    {
                                        if (wordSplit[j].Substring(0, 1) == lobby.GameState.Letter)
                                        {
                                            player.RoundPoints += 1;
                                            player.TotalPoints += 1;
                                        }
                                    }
                                }
                            }
                        }                        
                    }
                }
            }

            return lobby;
        }

        public static Player MarkPlayerAsRoundComplete(Player player, int roundNumber)
        {
            if (player != null)
            {
                switch (roundNumber)
                {
                    case 1:
                        player.RoundOneSubmitted = true;
                        return player;
                    case 2:
                        player.RoundTwoSubmitted = true;
                        return player;
                    case 3:
                        player.RoundThreeSubmitted = true;
                        return player;
                }
            }

            return null;
        }

        #endregion

        #region private methods

        private static List<string> GetPlayerScoreSheetByRound(ScoreSheet scoreSheet, int roundNumber)
        {
            if (scoreSheet != null)
            {
                switch (roundNumber)
                {
                    case 1:
                        return scoreSheet.RoundOne;
                    case 2:
                        return scoreSheet.RoundTwo;
                    case 3:
                        return scoreSheet.RoundThree;
                }
            }

            return null;
        }

        private static List<string>[] PopulateSubmittedWordsArray(Lobby lobby, int arraySize)
        {
            var submittedWordsArray = new List<string>[arraySize];
            for (var i = 0; i < arraySize; i++)
            {
                foreach (var player in lobby.Players)
                {
                    if (submittedWordsArray[i] == null)
                    {
                        submittedWordsArray[i] = new List<string>();
                    }
                    submittedWordsArray[i].Add(GetPlayerScoreSheetByRound(player.ScoreSheet, lobby.GameState.RoundNumber)[i]);
                }
            }

            return submittedWordsArray;
        }

        #endregion
    }
}
