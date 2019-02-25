using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SeasonPredict
{
    //Objects needed for deserialization of the JSON teams/rosters coming from the NHL's API

    public class Roster2
    {
        public Person Person { get; set; }
        public Position Position { get; set; }
        public string Name => Person.FullName;
        public string Id => Person.Id;
        public string Code => Position.Code;

        public override string ToString() => Person.FullName;
    }

    public class RosterList
    {
        public ObservableCollection<Roster2> Roster { get; set; }
    }
    public class Team
    {
        public int Id { get; set; }
        public bool Active { get; set; }
        public string Abbreviation { get; set; }
        public string Name { get; set; }
        public RosterList Roster { get; set; }
        public ObservableCollection<Roster2> PersonList
        {
            get => Roster.Roster;
            set { Roster.Roster = value; }
        }


        public override string ToString() => $"{Name} ({Abbreviation})";
    }

    public class TeamCollection : ObservableCollection<Team>
    {
        public TeamCollection()
        {
            teamsInit();
        }

        public async void teamsInit()
        {
            ObservableCollection <Team> temp = new ObservableCollection<Team>((await ApiLoader.LoadTeams()).OrderBy(t => t.Name));//Calls function responsible for the api teams loading request
                                                                                                                                  //+ sorts collection by name
            foreach (Team t in temp)
            {
                Add(t);
            }
        }
    }
    public class TeamList
    {
        public List<Team> Teams { get; set; }
    }
}
