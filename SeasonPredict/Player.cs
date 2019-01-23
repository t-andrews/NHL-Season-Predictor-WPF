using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeasonPredict
{
    public class Player
    {
        public List<Season> SeasonList { get; private set; }
        public Season ExpectedSeason { get; private set; }

        public void Add(Season s) => SeasonList.Add(s);

        public Player()
        {
            SeasonList = new List<Season>();
            ExpectedSeason = new Season();
        }

        public Player(List<Season> seasonsToDuplicate) : this()
        {
            foreach (Season s in seasonsToDuplicate)
            {
                SeasonList.Add(s.Duplicate(s));
            }
            CalculateExpectedSeason();
        }

        public Player Duplicate(Player p) => new Player(p.SeasonList);

        public void CalculateExpectedSeason()
        {
            double total = 0.0;
            List<double> weightList = new List<double>();
            int i = 0, average = (int)SeasonList.Average(p => p.GamesPlayed);

            for (i = 0; i < SeasonList.Count; i++)
            {
                if(SeasonList.Count > 5)//If there are enough seasons to eliminate the ones below games played average
                {
                    if (SeasonList[i].GamesPlayed >= average)//If above games played average
                    {
                        
                    }
                    else//Eliminate season with below average games played
                    {
                        SeasonList.Remove(SeasonList[i]);
                        i--;//Stay at the same index since the next one is moved back
                    }
                }
                else
                {
                    if (i == 0)
                        weightList.Add((double)(SeasonList.Count - i) * 0.5f);//Making the most recent season the most heavy on the final speculation
                    else if (i == 1)
                        weightList.Add((double)(SeasonList.Count - i) / (double)(SeasonList.Count * i));
                    else
                        weightList.Add((double)(SeasonList.Count - i) / (double)(SeasonList.Count * (i + 1)));
                }
                    
            }
            total = weightList.Sum();
            for (i = 0; i < weightList.Count;i++)
            {
                weightList[i] /= total;
                ExpectedSeason.Assists += (int)Math.Round((SeasonList[i].Assists * weightList[i]));
                ExpectedSeason.Goals += (int)Math.Round((SeasonList[i].Goals * weightList[i]));
                ExpectedSeason.GamesPlayed += (int)Math.Round((SeasonList[i].GamesPlayed * weightList[i]));
            }
            ExpectedSeason.CalculatePoints();
        }

        public void AddWeight(ref List<double> list, int i)
        {
            if (i == 0)
                list.Add((double)(SeasonList.Count - i));//Making the most recent season the most heavy on the final speculation
            else if (i == 1)
                list.Add((double)(SeasonList.Count - i) / (double)(SeasonList.Count));
            else
                list.Add((double)(SeasonList.Count - i) / (double)(SeasonList.Count * (i + 1)));
        }

        public override string ToString()
        {
            if (SeasonList.Count > 2)
            {
                return "Assists: " + ExpectedSeason.Assists
                + "\nGoals: " + ExpectedSeason.Goals
                + "\nPoints: " + ExpectedSeason.Points
                + "\nGames played: " + ExpectedSeason.GamesPlayed;
            }
            else
            {
                return "Insufficient number of seasons.";
            }
        }
    }

    public class Stat2
    {
        public int Assists { get; set; }
        public int Goals { get; set; }
        public int Games { get; set; }
    }
    public class Split
    {
        public Stat2 Stat { get; set; }
        public string Season { get; set; }
    }
    public class Stat
    {
        public List<Split> Splits { get; set; }
    }
    public class StatsList
    {
        public List<Stat> Stats { get; set; }
        public int Assists => Stats[0].Splits[0].Stat.Assists;
        public int Goals => Stats[0].Splits[0].Stat.Goals;
        public int Games => Stats[0].Splits[0].Stat.Games;
    }

    public class Position
    {
        public string Code { get; set; }
    }
    public class Person
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public bool Active { get; set; }

        public Person()
        {
            Active = true;
        }
    }
    public class PersonList
    {
        public List<Person> People { get; set; }
    }
}
