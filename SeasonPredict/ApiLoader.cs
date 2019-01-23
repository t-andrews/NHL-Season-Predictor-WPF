using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;

namespace SeasonPredict
{
    public class ApiLoader
    {
        public static async Task<ObservableCollection<Team>> LoadTeams()
        {
            ObservableCollection<Team> teamList = new ObservableCollection<Team>();

            string url = "https://statsapi.web.nhl.com/api/v1/teams/";
            string objectString;
            TeamList teamsObject;

            using (var client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    objectString = await client.GetStringAsync(url + "?expand=team.roster");
                    teamsObject = JsonConvert.DeserializeObject<TeamList>(objectString);

                    foreach (Team t in teamsObject.Teams)
                    {
                        if (t.Active)
                        {
                            t.PersonList = new ObservableCollection<Roster2>(t.PersonList.OrderBy(r => r.Person.FullName));

                            //Removing all goaltenders from the teams
                            for (int i = 0; i < t.PersonList.Count; i++)
                            {
                                if(t.PersonList[i].Code.Equals("G"))
                                {
                                    t.PersonList.Remove(t.PersonList[i]);
                                    i--;
                                }  
                            }
                            teamList.Add(t);
                        }
                            
                    }
                }
            };
            return teamList;
        }

        //public static async Task<int> FindId(string fullName, string abbreviation)
        //{
        //    string url = "https://statsapi.web.nhl.com/api/v1/teams/";
        //    int id = 0;
        //    string objectString;
        //    TeamList teamsObject;

        //    using (var client = new HttpClient())
        //    {
        //        HttpResponseMessage response = await client.GetAsync(url);

        //        if(response.IsSuccessStatusCode)
        //        {
        //            Team team = AllTeams.Single(t => t.Active && t.Abbreviation.ToUpper().Equals(abbreviation.ToUpper()));

        //            response = await client.GetAsync(url + team.Id + "?expand=team.roster");
        //            if (response.IsSuccessStatusCode)
        //            {
        //                objectString = await client.GetStringAsync(url + team.Id + "?expand=team.roster");
        //                teamsObject = JsonConvert.DeserializeObject<TeamList>(objectString);

        //                id = teamsObject.Teams[0].Roster.Roster.Single(p => p.Person.FullName.ToLower().Equals(fullName.ToLower())).Person.Id;

        //                //foreach (Roster2 r in teamsObject.Teams[0].Roster.Roster)
        //                //{
        //                //    if (r.Person.FullName.ToLower().Equals(fullName.ToLower()))
        //                //    {
        //                //        id = r.Person.Id;
        //                //        return id;
        //                //    }
        //                //}
        //            }
        //        }
        //    }

        //    return id;
        //}

        public static async Task<Player> LoadPlayer(string id)
        {
            int nullSeasonCount = 0;
            bool stop = false;
            string year = "20172018";
            string url = "https://statsapi.web.nhl.com/api/v1/people/" + id + "/stats?stats=statsSingleSeason&season=";
            string objectString;
            StatsList playerObject;
            List<Season> seasonList = new List<Season>();

            do
            {
                using (var client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(url + year);

                    if (response.IsSuccessStatusCode)
                    {
                        objectString = await client.GetStringAsync(url + year);
                        if (objectString.Contains("\"season\""))
                        {
                            playerObject = JsonConvert.DeserializeObject<StatsList>(objectString);
                            seasonList.Add(new Season(playerObject.Assists, playerObject.Goals, playerObject.Games));
                        } 
                        else
                        {
                            nullSeasonCount++;
                        }
                    }
                    else
                    {
                        nullSeasonCount++;
                    }

                    if (nullSeasonCount > 4)
                        stop = true;

                    year = (int.Parse(year.Substring(0, 4)) - 1).ToString() 
                         + (int.Parse(year.Substring(4, 4)) - 1).ToString();

                }
            }
            while (!stop);

            return new Player(seasonList);
        }
    }
}
