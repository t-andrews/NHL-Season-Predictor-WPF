using System;
using System.Collections.Generic;
using System.Linq;

namespace SeasonPredict
{
    #region Player class
    //Player class representing a hockey player
    public class Player : Person
    {
        private Season _expectedSeason;
        private List<Season> _seasonList;

        public List<Season> SeasonList { get => _seasonList; private set => _seasonList = value; }
        public Season ExpectedSeason { get => _expectedSeason; private set => _expectedSeason = value; }

        public void add(Season s) => SeasonList.Add(s);
        public void remove(Season s) => SeasonList.Remove(s);

        public Player(List<Season> seasonsToDuplicate)
        {
            SeasonList = new List<Season>();
            ExpectedSeason = new Season();
            FullName = "";

            foreach (var s in seasonsToDuplicate)
            {
                add(Season.duplicate(s));
            }
            calculateExpectedSeason();
        }


        public Player(Player p, string name, string id) : this(p.SeasonList)
        {
            Id = id;
            FullName = name;
        }

        /// <summary>
        /// Complete player duplicator: calls the 3-parameter constructor
        /// </summary>
        /// <param name="p"></param>
        /// <returns>The duplication of the player passed as a parameter</returns>
        public static Player duplicate(Player p) => new Player(p, p.FullName, p.Id);

        /// <summary>
        /// Calculates an estimation of the player's next season by computing a weighted average with the most important season being the most recent
        /// </summary>
        public void calculateExpectedSeason()
        {
            var total = 0.0;
            var weightsList = new List<double>();
            int i = 0, averageGames = (int)SeasonList.Average(p => p.GamesPlayed);

            for (i = 0; i < SeasonList.Count; i++)
            {
                if (SeasonList.Count > 5)//If there are enough seasons to eliminate the ones below games played average
                {
                    if (SeasonList[i].GamesPlayed >= averageGames)//If above games played average
                    {
                        addWeight(weightsList, i);
                    }
                    else//Eliminate season with below average games played
                    {
                        remove(SeasonList[i]);
                        i--;//Stay at the same index since the next one is moved back
                    }
                }
                else
                {
                    addWeight(weightsList, i);
                }
            }
            //Total of all absolute weights used to calculate relative weight of each season in next step
            total = weightsList.Sum();

            for (i = 0; i < weightsList.Count; i++)
            {
                incrementSeasonWeight(weightsList, i, total);
            }

            if (ExpectedSeason.GamesPlayed > 82)
            {
                ExpectedSeason.GamesPlayed = 82;
            }
                

            ExpectedSeason.calculatePoints();
        }

        /// <summary>
        /// Adds the wanted season stats to the totals used later for averages
        /// </summary>
        /// <param name="weightList">Current list of weights each season has on the overall calculation</param>
        /// <param name="i">Current season index</param>
        /// <param name="total">Sum of all season weights</param>
        private void incrementSeasonWeight(List<double> weightList, int i, double total)
        {
            weightList[i] /= total;//Making this season's weight into percentage
            ExpectedSeason.Assists += (int)Math.Round((SeasonList[i].Assists * weightList[i]));
            ExpectedSeason.Goals += (int)Math.Round((SeasonList[i].Goals * weightList[i]));
            ExpectedSeason.GamesPlayed += (int)Math.Round((SeasonList[i].GamesPlayed * weightList[i]));
        }

        private void addWeight(List<double> weightsList, int i)
        {
            if (i == 0)
            {
                weightsList.Add((double)(SeasonList.Count - i) * 0.4f);
            }
            else if (i == 1)
            {
                weightsList.Add((double)(SeasonList.Count - i) / (double)(SeasonList.Count) * 1.1f);

            }
            else
            {
                weightsList.Add((double)(SeasonList.Count - i) / (double)(SeasonList.Count * (i + 1)));
            }
        }

        public override string ToString()
        {
            if (SeasonList.Count > 2)
            {
                return FullName
                       + "\nAssists: " + ExpectedSeason.Assists
                       + "\nGoals: " + ExpectedSeason.Goals
                       + "\nPoints: " + ExpectedSeason.Points
                       + "\nGames played: " + ExpectedSeason.GamesPlayed;

            }
            return "Insufficient number of seasons.";
        }
    }
    #endregion

    #region Objects needed for deserialization of the JSON persons/stats coming from the NHL's API
    public class Stat2
    {
        private int _games;
        private int _goals;
        private int _assists;

        public int Assists { get => _assists; set => _assists = value; }
        public int Goals { get => _goals; set => _goals = value; }
        public int Games { get => _games; set => _games = value; }
    }
    public class Split
    {
        //private string season;
        private Stat2 _stat;

        public Stat2 Stat { get => _stat; set => _stat = value; }
        //public string Season { get => season; set => season = value; }
    }
    public class Stat
    {
        private List<Split> _splits;

        public List<Split> Splits { get => _splits; set => _splits = value; }
    }
    public class StatsList
    {
        private List<Stat> _stats;

        public List<Stat> Stats { get => _stats; set => _stats = value; }
        private int Assists => Stats[0].Splits[0].Stat.Assists;
        private int Goals => Stats[0].Splits[0].Stat.Goals;
        private int Games => Stats[0].Splits[0].Stat.Games;
        public Season Season => new Season(Assists, Goals, Games);
    }

    public class Position
    {
        private string _code;

        public string Code { get => _code; set => _code = value; }
    }
    public class Person
    {
        private string _id;
        private string _fullName;
        private bool _active;

        public string Id { get => _id; set => _id = value; }
        public string FullName { get => _fullName; set => _fullName = value; }
        public bool Active { get => _active; set => _active = value; }

        public Person()
        {
            Active = true;
        }
    }
    public class PersonList
    {
        private List<Person> _people;

        public List<Person> People { get => _people; set => _people = value; }
    }
}
#endregion