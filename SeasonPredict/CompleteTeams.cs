using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SeasonPredict
{
    #region Objects needed for deserialization of the JSON teams/rosters coming from the NHL's API
    public class Roster2
    {
        private Person person;
        private Position position;

        public Person Person { get => person; set => person = value; }
        public Position Position { get => position; set => position = value; }
        public string Name => Person.FullName;
        public string Id => Person.Id;
        public string Code => Position.Code;

        public override string ToString() => Person.FullName;
    }

    public class RosterList
    {
        private ObservableCollection<Roster2> roster;

        public ObservableCollection<Roster2> Roster { get => roster; set => roster = value; }
    }
    public class Team
    {
        private int id;
        private bool active;
        private string abbreviation;
        private string name;
        private RosterList roster;

        public int Id { get => id; set => id = value; }
        public bool Active { get => active; set => active = value; }
        public string Abbreviation { get => abbreviation; set => abbreviation = value; }
        public string Name { get => name; set => name = value; }
        public RosterList Roster { get => roster; set => roster = value; }

        //Using an ObservalbeCollection istead of a List because of the data binding required in the GUI
        public ObservableCollection<Roster2> PersonList
        {
            get => Roster.Roster;
            set { Roster.Roster = value; }
        }

        public override string ToString() => $"{Name} ({Abbreviation})";
    }

    public class TeamList
    {
        private List<Team> teams;

        public List<Team> Teams { get => teams; set => teams = value; }
    }
    #endregion

    #region TeamCollection class used to contain all teams fetched from the API and display them in the GUI
    public class TeamCollection : ObservableCollection<Team>
    {
        //Default and only constructor
        public TeamCollection()
        {
            teamsInit();
        }

        public async void teamsInit()
        {
            ObservableCollection <Team> temp = new ObservableCollection<Team>((await ApiLoader.LoadTeams()).OrderBy(t => t.Name));//Calls function responsible for the api teams loading request
                                                                                                                                  //+ sorts collection by name
            foreach (Team t in temp)
                Add(t);
        }
    }
    #endregion
}
