using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SeasonPredict
{
    #region Objects needed for deserialization of the JSON teams/rosters coming from the NHL's API
    public class Roster2
    {
        private Person _person;
        private Position _position;

        public Person Person { get => _person; set => _person = value; }
        public Position Position { get => _position; set => _position = value; }
        public string Name => Person.FullName;
        public string Id => Person.Id;
        public string Code => Position.Code;

        public override string ToString() => Person.FullName;
    }

    public class RosterList
    {
        private ObservableCollection<Roster2> _roster;

        public ObservableCollection<Roster2> Roster { get => _roster; set => _roster = value; }
    }
    public class Team
    {
        private int _id;
        private bool _active;
        private string _abbreviation;
        private string _name;
        private RosterList _roster;

        public int Id { get => _id; set => _id = value; }
        public bool Active { get => _active; set => _active = value; }
        public string Abbreviation { get => _abbreviation; set => _abbreviation = value; }
        public string Name { get => _name; set => _name = value; }
        public RosterList Roster { get => _roster; set => _roster = value; }

        //Using an ObservalbeCollection instead of a List because of the data binding required in the GUI
        public ObservableCollection<Roster2> PersonList
        {
            get => Roster.Roster;
            set => Roster.Roster = value;
        }

        public override string ToString() => $"{Name} ({Abbreviation})";
    }

    public class TeamList
    {
        private List<Team> _teams;

        public List<Team> Teams { get => _teams; set => _teams = value; }
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
            var temp = new ObservableCollection<Team>((await ApiLoader.loadTeams()).OrderBy(t => t.Name));//Calls function responsible for the api teams loading request
                                                                                                                                  //+ sorts collection by name
            foreach (var t in temp)
                Add(t);
        }
    }
    #endregion
}
