namespace PlayScattergories.Server.Models.Player
{
    public class Player
    {
        public Player(string id, string name)
        {
            Id = id;
            Name = name;
            Points = 0;
            ScoreSheet = new ScoreSheet();
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public int Points { get; set; }
        public ScoreSheet? ScoreSheet { get; set; }
        public bool RoundOneSubmitted { get; set; }
        public bool RoundTwoSubmitted { get; set; }
        public bool RoundThreeSubmitted { get; set; }
    }
}
