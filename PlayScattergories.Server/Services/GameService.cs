using PlayScattergories.Server.Helpers;
using PlayScattergories.Server.Models;
using PlayScattergories.Server.Models.Game;
using PlayScattergories.Server.Models.Player;

namespace PlayScattergories.Server.Services
{
    public static class GameService
    {
        private static Random _random = new Random();
        private static readonly List<string> _articles = ConfigurationHelper.config.GetSection("Articles").GetChildren().Select(x => x.Value).ToList();

        #region public methods

        public static List<CategoryCard> GetAllCategoryCards()
        {
            var categoryList = ConfigurationHelper.config.GetValue<string>("CategoryList");

            if (categoryList != null && categoryList.Any())
            {
                var allCategories = categoryList.Split(',').ToList();
                var categoryCardLength = ConfigurationHelper.config.GetValue<int>("App:CategoryCardLength");
                var cards = new List<CategoryCard>();
                while (allCategories.Any() && allCategories.Count() >= categoryCardLength)
                {
                    var card = new CategoryCard();
                    for (var i = 0; i < categoryCardLength; i++)
                    {
                        var index = _random.Next(allCategories.Count);
                        card.Categories.Add(allCategories[index]);
                        allCategories.RemoveAt(index);
                    }
                    cards.Add(card);
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
            if (scoreSheet == null || words == null)
            {
                return null;
            }

            switch (roundNumber)
            {
                case 1:
                    scoreSheet.RoundOne = new List<Word>();
                    foreach (var word in words)
                    {
                        scoreSheet.RoundOne.Add(new Word { Value = word, IsValid = false });
                    }
                    return scoreSheet;
                case 2:
                    scoreSheet.RoundTwo = new List<Word>();
                    foreach (var word in words)
                    {
                        scoreSheet.RoundTwo.Add(new Word { Value = word, IsValid = false });
                    }
                    return scoreSheet;
                case 3:
                    scoreSheet.RoundThree = new List<Word>();
                    foreach (var word in words)
                    {
                        scoreSheet.RoundThree.Add(new Word { Value = word, IsValid = false });
                    }
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

            var categoryCardLength = ConfigurationHelper.config.GetValue<int>("App:CategoryCardLength");

            // Create array of lists with all submitted words
            var submittedWordsArray = PopulateSubmittedWordsArray(lobby, categoryCardLength);

            // Create array of lists for all duplicate words
            var duplicateWordsArray = new List<string>[categoryCardLength];

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
                    // If the word isn't null
                    if (!string.IsNullOrWhiteSpace(currentWords[i].Value))
                    {
                        var wordSplit = currentWords[i].Value.Split(' ');
                        var wordToScore = string.Empty;

                        // Remove starting article if present
                        if (wordSplit.Length > 1 && _articles.Contains(wordSplit[0]))
                        {
                            var listWithoutArticle = new List<string>(wordSplit);
                            listWithoutArticle.RemoveAt(0);
                            wordSplit = listWithoutArticle.ToArray();
                        }

                        wordToScore = string.Join(" ", wordSplit);

                        // If the duplicates array doesn't contain the word, and the word starts with the correct letter
                        if (!duplicateWordsArray[i].Contains(wordToScore.ToLower()) && wordToScore.Substring(0, 1).ToLower() == lobby.GameState.Letter.ToLower())
                        {
                            currentWords[i].IsValid = true;
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
                                        if (wordSplit[j].Substring(0, 1) == lobby.GameState.Letter && !_articles.Contains(wordSplit[j]))
                                        {
                                            player.RoundPoints += 1;
                                            player.TotalPoints += 1;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            currentWords[i].IsValid = false;
                        }
                    }
                }

                player.ScoreSheet = UpdatePlayerScoreSheetByRound(player.ScoreSheet, currentWords, lobby.GameState.RoundNumber);
            }

            var time = DateTime.Now.AddMinutes(ConfigurationHelper.config.GetValue<int>("App:VoteLengthInMinutes")).ToUniversalTime() - new DateTime(1970, 1, 1);
            lobby.GameState.SubmitNextVoteTimeLimit = (long)(time.TotalMilliseconds + 0.5);
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

        public static Player MarkPlayerAsVotingComplete(Player player, int roundNumber)
        {
            if (player != null)
            {
                switch (roundNumber)
                {
                    case 1:
                        player.RoundOneVoted = true;
                        return player;
                    case 2:
                        player.RoundTwoVoted = true;
                        return player;
                    case 3:
                        player.RoundThreeVoted = true;
                        return player;
                }
            }

            return null;
        }

        public static Lobby ScoreVotes(Lobby lobby)
        {
            if (lobby == null || lobby.GameState == null)
            {
                return null;
            }

            var numberOfVotesToBan = (lobby.Players.Count + 1) / 2;

            // Loop through votes and add words with enough votes to the banned list
            foreach (var vote in lobby.GameState.Votes)
            {
                if (vote.Count >= numberOfVotesToBan)
                {
                    lobby.GameState.BannedWords.Add(vote.Word.ToLower());
                }
            }

            // Loop through players and remove points for each word on the banned list
            foreach (var player in lobby.Players)
            {
                var words = GetPlayerScoreSheetByRound(player.ScoreSheet, lobby.GameState.RoundNumber);

                if (words != null && words.Any())
                {
                    foreach (var word in words)
                    {
                        if (lobby.GameState.BannedWords.Contains(word.Value.ToLower()))
                        {
                            word.IsValid = false;
                            if (player.RoundPoints > 0)
                            {
                                player.RoundPoints -= 1;
                            }
                            if (player.TotalPoints > 0)
                            {
                                player.TotalPoints -= 1;
                            }

                            if (ConfigurationHelper.config.GetValue<bool>("App:ExtraPointsEnabled"))
                            {
                                var wordSplit = word.Value.Split(' ');

                                // Remove starting article if present
                                if (wordSplit.Length > 1 && _articles.Contains(wordSplit[0]))
                                {
                                    var listWithoutArticle = new List<string>(wordSplit);
                                    listWithoutArticle.RemoveAt(0);
                                    wordSplit = listWithoutArticle.ToArray();
                                }

                                // Check for multi word answers
                                if (wordSplit != null && wordSplit.Length > 1)
                                {
                                    // Start at index 1 here, because we've already taken away one point for the starting letter
                                    for (var j = 1; j < wordSplit.Length; j++)
                                    {
                                        if (wordSplit[j].Substring(0, 1) == lobby.GameState.Letter && !_articles.Contains(wordSplit[j]))
                                        {
                                            if (player.RoundPoints > 0)
                                            {
                                                player.RoundPoints -= 1;
                                            }
                                            if (player.TotalPoints > 0)
                                            {
                                                player.TotalPoints -= 1;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    player.ScoreSheet = UpdatePlayerScoreSheetByRound(player.ScoreSheet, words, lobby.GameState.RoundNumber);
                }
            }

            return lobby;
        }

        #endregion

        #region private methods

        private static List<Word> GetPlayerScoreSheetByRound(ScoreSheet scoreSheet, int roundNumber)
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
                    submittedWordsArray[i].Add(GetPlayerScoreSheetByRound(player.ScoreSheet, lobby.GameState.RoundNumber)[i].Value);
                }
            }

            return submittedWordsArray;
        }

        private static ScoreSheet UpdatePlayerScoreSheetByRound(ScoreSheet scoreSheet, List<Word> words, int roundNumber)
        {
            if (scoreSheet == null || words == null)
            {
                return null;
            }

            switch (roundNumber)
            {
                case 1:
                    scoreSheet.RoundOne = words;
                    break;
                case 2:
                    scoreSheet.RoundTwo = words;
                    break;
                case 3:
                    scoreSheet.RoundThree = words;
                    break;
            }

            return scoreSheet;
        }

        #endregion
    }
}
