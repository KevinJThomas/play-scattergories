namespace PlayScattergories.Server.Models.Player
{
    public class ScoreSheet
    {
        public ScoreSheet()
        {
            RoundOne = new List<string>
            {
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty
            };
            RoundTwo = new List<string>
            {
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty
            };
            RoundThree = new List<string>
            {
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty
            };
        }

        public List<string> RoundOne { get; set; }
        public List<string> RoundTwo { get; set; }
        public List<string> RoundThree { get; set; }
    }
}
