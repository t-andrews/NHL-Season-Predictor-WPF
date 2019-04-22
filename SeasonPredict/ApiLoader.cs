

namespace SeasonPredict
{
    using RestSharp;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;


    public class ApiLoader
    {
        /// <summary>The URL</summary>
        const string url = "https://statsapi.web.nhl.com/api/v1/";


        /// <summary>The rest client</summary>
        private readonly RestClient restClient;

        /// <summary>Initializes a new instance of the <see cref="ApiLoader"/> class.</summary>
        public ApiLoader()
        {
            this.restClient = new RestClient()
                                  {
                                      BaseHost = url
                                  };
        }

        /// <summary>Fetching and deserializing all active teams</summary>
        /// <returns>The complete list of active teams (with their roster)</returns>
        public ObservableCollection<Team> loadTeams()
        {
            var teamList = new ObservableCollection<Team>();

            var restRequest = new RestRequest()
                                  {
                                      Method = Method.GET,
                                      Resource = "teams/?expand=team.roster"
            };

            var responseDaFirst = this.restClient.Execute<TeamList>(restRequest);
            var successFull = responseDaFirst.Data?.Teams;

            if (successFull == null)
            {
                return null;
            }

            foreach (var team in successFull)
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

            return teamList;
        }

        /// <summary>
        ///     Fetching and deserializing player object corresponding to ID
        /// </summary>
        /// <param name="id">Player ID in the NHL's database</param>
        /// <returns>The player </returns>
        public Player loadPlayer(string id)
        {
            var nullSeasonCount = 0;
            var stop = false;
            var year = "20172018";
            var url = "https://statsapi.web.nhl.com/api/v1/people/" + id + "/stats?stats=statsSingleSeason&season=";
            var seasonList = new List<Season>();

            var restRequest = new RestRequest()
            {
                Method = Method.GET,
                Resource = "teams/?expand=team.roster"
            };

            do
            {
                for (int i = 10; i > 0; i--)
                {
                    var myIntYear = 9;
                    var currentYear = $"201{myIntYear}";

                    if (returnValue == null)
                    {
                        break;
                    }
                }
                
                    var response = await client.GetAsync(url + year);

                    if (response.IsSuccessStatusCode)
                    {
                        var objectString = await client.GetStringAsync(url + year);
                        if (objectString.Contains("\"season\"")) //Means there is an active season at that year
                        {
                            var seasonObject = JsonConvert.DeserializeObject<StatsList>(objectString);
                            seasonList.Add(Season.duplicate(seasonObject.Season));
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

                    //Subtracting one season year
                    year = int.Parse(year.Substring(0, 4)) - 1
                           + (int.Parse(year.Substring(4, 4)) - 1).ToString();
                
            } while (nullSeasonCount <= 4);

            return new Player(seasonList);
        }
    }
}