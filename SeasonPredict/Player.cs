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
        }

        public override string ToString()
        {
            return "A:" + ExpectedSeason.Assists
                + " G:" + ExpectedSeason.Goals
                + " P:" + ExpectedSeason.Points
                + " GP:" + ExpectedSeason.GamesPlayed;
        }

        public Player Duplicate(Player p) => new Player(p.SeasonList);

        public void CalculateExpectedSeason()
        {
            double total = 0.0;
            List<double> weightList = new List<double>();
            int i;

            for (i = 0; i < SeasonList.Count; i++)
            {
                if (SeasonList[i].GamesPlayed >= SeasonList.Average(p => p.GamesPlayed))
                {
                    if(i==0)
                        weightList.Add((double)(SeasonList.Count - i) * 0.8f);//Making the most recent season the more heavy on the final speculation
                    else
                        weightList.Add((double)(SeasonList.Count - i) / (double)(SeasonList.Count * (i + 1)));
                }
                    
                else
                {
                    SeasonList.Remove(SeasonList[i]);
                    i--;
                }
            }
            total = weightList.Sum();
            for (i = 0; i < weightList.Count;i++)
            {
                weightList[i] /= total;
                ExpectedSeason.Assists += (int)(SeasonList[i].Assists * weightList[i]);
                ExpectedSeason.Goals += (int)(SeasonList[i].Goals * weightList[i]);
                ExpectedSeason.GamesPlayed += (int)(SeasonList[i].GamesPlayed * weightList[i]);
            }
            ExpectedSeason.CalculatePoints();
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
