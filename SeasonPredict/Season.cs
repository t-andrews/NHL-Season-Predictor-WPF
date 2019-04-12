namespace SeasonPredict
{
    //Season class representing the offensive production of a Player for a given year

    public class Season
    {
        private int assists;
        private int goals;
        private int points;
        private int gamesPlayed;

        public int Assists { get => assists; set => assists = value; }
        public int Goals { get => goals; set => goals = value; }
        public int Points { get => points; set => points = value; }
        public int GamesPlayed { get => gamesPlayed; set => gamesPlayed = value; }

        public Season()
        {
            Assists = 0;
            Goals = 0;
            Points = 0;
            GamesPlayed = 0;
        }

        public Season(int assists, int goals, int gamesPlayed)
        {
            Assists = assists;
            Goals = goals;
            GamesPlayed = gamesPlayed;
            CalculatePoints();
        }

        public static Season Duplicate(Season s)
        {
            return new Season(s.Assists, s.Goals, s.GamesPlayed);
        }

        public void CalculatePoints()
        {
            Points = Assists + Goals;
        }
    }
}
