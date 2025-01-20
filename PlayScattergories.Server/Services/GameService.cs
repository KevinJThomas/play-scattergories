using PlayScattergories.Server.Models;
using PlayScattergories.Server.Models.Game;
using PlayScattergories.Server.Models.Player;

namespace PlayScattergories.Server.Services
{
    public static class GameService
    {
        private static readonly List<CategoryCard> _allCategoryCards = new List<CategoryCard>
        {
            new CategoryCard
            {
                Categories = new List<string>
                {
                    "A girl’s name",
                    "Capital cities",
                    "Animals",
                    "Musical Instruments",
                    "Gemstones",
                    "Cartoon Characters",
                    "Four letter words",
                    "Brands",
                    "Things on a beach",
                    "Websites",
                    "Cars",
                    "Things that are sticky"
                },
            },
            new CategoryCard
            {
                Categories = new List<string>
                {
                    "A boy’s name",
                    "Countries",
                    "Flowers",
                    "Things that you shout",
                    "Excuses for being late",
                    "Pet peeves",
                    "Ice cream flavors",
                    "Fried foods",
                    "Bodies of water",
                    "Halloween costumes",
                    "Places to go on a date",
                    "Nicknames"
                },
            }
        };
        private static Random _random = new Random();

        #region public methods

        public static List<CategoryCard> GetAllCategoryCards()
        {
            return _allCategoryCards;
        }

        public static (List<CategoryCard>, CategoryCard) ChooseNextCategoryCard(List<CategoryCard> categoryCards)
        {
            var index = _random.Next(categoryCards.Count);
            var nextCategoryCard = categoryCards[index];
            categoryCards.RemoveAt(index);
            return (categoryCards,  nextCategoryCard);
        }

        public static string GetLetter()
        {
            // Return a random lowercase letter a-z
            var character = (char)('a' + _random.Next(0, 26));
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

        public static GameState ScoreRound(Lobby lobby)
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

            var submittedWordsArray = PopulateSubmittedWordsArray(lobby, hostsCurrentWords.Count);
            // Loop through array and create duplicates list for each
            // Loop through players, scoring their current score sheets, and not counting anything in the duplicate lists

            // continue logic here
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
                    submittedWordsArray[i].Add(GetPlayerScoreSheetByRound(player.ScoreSheet, lobby.GameState.RoundNumber)[i]);
                }
            }

            return submittedWordsArray;
        }

        #endregion
    }
}
