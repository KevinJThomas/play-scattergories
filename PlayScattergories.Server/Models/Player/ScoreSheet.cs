namespace PlayScattergories.Server.Models.Player
{
    public class ScoreSheet
    {
        public ScoreSheet()
        {
            RoundOne = new List<Word>
            {
                new Word { Value = string.Empty, IsValid = false },
                new Word { Value = string.Empty, IsValid = false },
                new Word { Value = string.Empty, IsValid = false },
                new Word { Value = string.Empty, IsValid = false },
                new Word { Value = string.Empty, IsValid = false },
                new Word { Value = string.Empty, IsValid = false },
                new Word { Value = string.Empty, IsValid = false },
                new Word { Value = string.Empty, IsValid = false },
                new Word { Value = string.Empty, IsValid = false },
                new Word { Value = string.Empty, IsValid = false },
                new Word { Value = string.Empty, IsValid = false },
                new Word { Value = string.Empty, IsValid = false }
            };
            RoundTwo = new List<Word>
            {
                new Word { Value = string.Empty, IsValid = false },
                new Word { Value = string.Empty, IsValid = false },
                new Word { Value = string.Empty, IsValid = false },
                new Word { Value = string.Empty, IsValid = false },
                new Word { Value = string.Empty, IsValid = false },
                new Word { Value = string.Empty, IsValid = false },
                new Word { Value = string.Empty, IsValid = false },
                new Word { Value = string.Empty, IsValid = false },
                new Word { Value = string.Empty, IsValid = false },
                new Word { Value = string.Empty, IsValid = false },
                new Word { Value = string.Empty, IsValid = false },
                new Word { Value = string.Empty, IsValid = false }
            };
            RoundThree = new List<Word>
            {
                new Word { Value = string.Empty, IsValid = false },
                new Word { Value = string.Empty, IsValid = false },
                new Word { Value = string.Empty, IsValid = false },
                new Word { Value = string.Empty, IsValid = false },
                new Word { Value = string.Empty, IsValid = false },
                new Word { Value = string.Empty, IsValid = false },
                new Word { Value = string.Empty, IsValid = false },
                new Word { Value = string.Empty, IsValid = false },
                new Word { Value = string.Empty, IsValid = false },
                new Word { Value = string.Empty, IsValid = false },
                new Word { Value = string.Empty, IsValid = false },
                new Word { Value = string.Empty, IsValid = false }
            };
        }

        public List<Word> RoundOne { get; set; }
        public List<Word> RoundTwo { get; set; }
        public List<Word> RoundThree { get; set; }
    }
}
