namespace PlayScattergories.Server.Models.Player
{
    public class Player
    {
        public Player(string id, string name)
        {
            Id = id;
            Name = name;
            IsActive = true;
            RoundPoints = 0;
            TotalPoints = 0;
            ScoreSheet = new ScoreSheet();
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public int RoundPoints { get; set; }
        public int TotalPoints { get; set; }
        public ScoreSheet? ScoreSheet { get; set; }
        public bool RoundOneSubmitted { get; set; }
        public bool RoundTwoSubmitted { get; set; }
        public bool RoundThreeSubmitted { get; set; }
        public bool RoundOneVoted { get; set; }
        public bool RoundTwoVoted { get; set; }
        public bool RoundThreeVoted { get; set; }
    }
}
