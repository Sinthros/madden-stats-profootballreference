namespace MaddenImporter.Models.Player
{
    public abstract class Player
    {
        public string Name { get; set; }
        public object Team { get; set; }
        public string Position { get; set; }
        public int GamesPlayed { get; set; }
        public int GamesStarted { get; set; }
        public object PlayerLink { get; set; }
        public object Age { get; set; }

        public override string ToString()
        {
            return $"Player {Name}, from team {Team}, position {Position}";
        }
    }
}
