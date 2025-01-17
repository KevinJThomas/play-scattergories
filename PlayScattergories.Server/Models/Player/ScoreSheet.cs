namespace PlayScattergories.Server.Models.Player
{
    public class ScoreSheet
    {
        public ScoreSheet()
        {
            RoundOne = new List<string>();
            RoundTwo = new List<string>();
            RoundThree = new List<string>();
        }

        public List<string> RoundOne { get; set; }
        public List<string> RoundTwo { get; set; }
        public List<string> RoundThree { get; set; }
    }
}
