using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SeasonPredict
{
    public class ApiLoader
    {
        /// <summary>
        ///     Fetching and deserializing all active teams
        /// </summary>
        /// <param name="id">Player ID in the NHL's database</param>
        /// <returns>The complete list of active teams (with their roster)</returns>
        public static async Task<ObservableCollection<Team>> loadTeams()
        {
            var teamList = new ObservableCollection<Team>();

            var url = "https://statsapi.web.nhl.com/api/v1/teams/";
            TeamList allTeams;

            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    allTeams = JsonConvert.DeserializeObject<TeamList>(
                        await client.GetStringAsync(url + "?expand=team.roster"));

                    foreach (var team in allTeams.Teams)
                    {
                        if (team.Active)
                        {
                            team.PersonList =
                                new ObservableCollection<Roster2>(team.PersonList.OrderBy(r => r.Person.FullName));

                            while (team.PersonList.Any(p => p.Code.Equals("G")))
                            {
                                team.PersonList.Remove(team.PersonList.First(p => p.Code.Equals("G")));
                            }

                            teamList.Add(team);
                        }
                    }
                }
            }

            ;
            return teamList;
        }

        /// <summary>
        ///     Fetching and deserializing player object corresponding to ID
        /// </summary>
        /// <param name="id">Player ID in the NHL's database</param>
        /// <returns>The player </returns>
        public static async Task<Player> loadPlayer(string id)
        {
            var nullSeasonCount = 0;
            var stop = false;
            var year = "20172018";
            var url = "https://statsapi.web.nhl.com/api/v1/people/" + id + "/stats?stats=statsSingleSeason&season=";
            string objectString;
            StatsList seasonObject;
            var seasonList = new List<Season>();

            do
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync(url + year);

                    if (response.IsSuccessStatusCode)
                    {
                        objectString = await client.GetStringAsync(url + year);
                        if (objectString.Contains("\"season\"")) //Means there is an active season at that year
                        {
                            seasonObject = JsonConvert.DeserializeObject<StatsList>(objectString);
                            seasonList.Add(Season.Duplicate(seasonObject.Season));
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
                    {
                        stop = true;
                    }

                    //Subtracting one season year
                    year = int.Parse(year.Substring(0, 4)) - 1
                           + (int.Parse(year.Substring(4, 4)) - 1).ToString();
                }
            } while (!stop);

            return new Player(seasonList);
        }
    }
}