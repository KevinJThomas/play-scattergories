using PlayScattergories.Server.Helpers;
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
            RoundOneSubmitted = false;
            RoundTwoSubmitted = false;
            RoundThreeSubmitted = false;
            UsedLetters = ConfigurationHelper.config.GetSection("ExcludedLetters").GetChildren().Select(x => x.Value.ToLower()).ToList();
        }

        public string Letter { get; set; }
        public int RoundNumber { get; set; }
        public CategoryCard? CategoryCard { get; set; }
        public List<CategoryCard> UnusedCategoryCards { get; set; }
        public bool RoundOneSubmitted { get; set; }
        public bool RoundTwoSubmitted { get; set; }
        public bool RoundThreeSubmitted { get; set; }
        public long SubmitNextRoundTimeLimit { get; set; }
        public long SubmitNextVoteTimeLimit { get; set; }
        public List<string> UsedLetters { get; set; }
    }
}
