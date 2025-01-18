using PlayScattergories.Server.Services;

namespace PlayScattergories.Server.Models.Game
{
    public class GameState
    {
        public GameState()
        {
            Letter = string.Empty;
            RoundNumber = 0;
            CategoryCard = new CategoryCard();
            UnusedCategoryCards = GameService.GetAllCategoryCards();
        }

        public string Letter { get; set; }
        public int RoundNumber { get; set; }
        public CategoryCard? CategoryCard { get; set; }
        public List<CategoryCard> UnusedCategoryCards { get; set; }
    }
}
