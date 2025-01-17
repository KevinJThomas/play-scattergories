using PlayScattergories.Server.Models.Game;

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
    }
}
