namespace SeasonPredict
{
    #region Season Class representing the offensive production of a Player for a given year

    public class Season
    {
        public int Assists { get; set; }
        public int Goals { get; set; }
        public int Points { get; set; }
        public int GamesPlayed { get; set; }

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
            calculatePoints();
        }

        public static Season duplicate(Season s)
        {
            return new Season(s.Assists, s.Goals, s.GamesPlayed);
        }

        public void calculatePoints()
        {
            Points = Assists + Goals;
        }
    }
}

#endregion
