namespace SeasonPredict
{
    //Season class representing the offensive production of a Player for a given year

    public class Season
    {
        private int _assists;
        private int _goals;
        private int _points;
        private int _gamesPlayed;

        public int Assists { get => _assists; set => _assists = value; }
        public int Goals { get => _goals; set => _goals = value; }
        public int Points { get => _points; set => _points = value; }
        public int GamesPlayed { get => _gamesPlayed; set => _gamesPlayed = value; }

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
