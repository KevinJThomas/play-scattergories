namespace PlayScattergories.Server.Models.Player
{
    public class Player
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Points {  get; set; }
        public ScoreSheet? ScoreSheet { get; set; }
    }
}
