using System;
using Newtonsoft.Json;

namespace SeasonPredict
{
    using RestSharp;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;


    public class ApiLoader
    {
        /// <summary>The URL</summary>
        private const string baseUrl = "https://statsapi.web.nhl.com/api/v1";

        /// <summary>The rest client</summary>
        private readonly RestClient restClient;

        /// <summary>Initializes a new instance of the <see cref="ApiLoader"/> class.</summary>
        public ApiLoader()
        {
            this.restClient = new RestClient(baseUrl);
        }

        /// <summary>Fetching and deserializing all active teams</summary>
        /// <returns>The complete list of active teams (with their roster)</returns>
        public ObservableCollection<Team> loadTeams()
        {
            var teamCollection = new ObservableCollection<Team>();

            var request = new RestRequest()
            {
                Method = Method.GET,
                Resource = "teams/?expand=team.roster"
             };

            var response = this.restClient.Execute(request);

            var validTeamList = JsonConvert.DeserializeObject<TeamList>(response.Content)?.Teams;

            if (validTeamList == null)
            {
                return null;
            }

            foreach (var team in validTeamList)
            {
                if (!team.Active)
                {
                    continue;
                }

                team.PersonList = new ObservableCollection<Roster2>(team.PersonList.OrderBy(r => r.Person.FullName));

                while (team.PersonList.Any(p => p.Code.Equals("G")))
                {
                    team.PersonList.Remove(team.PersonList.First(p => p.Code.Equals("G")));
                }

                teamCollection.Add(team);
            }

            return teamCollection;
        }

        /// <summary>
        /// Fetching and deserializing player object corresponding to ID
        /// </summary>
        /// <param name="id">Player ID in the NHL's database</param>
        /// <returns>The player </returns>
        public Player loadPlayer(string id)
        {
            var recentYear = DateTime.Now.Year;

            var baseResource = "people/" + id + "/stats?stats=statsSingleSeason&season=";

            var nullSeasonCount = 0;
            var seasonList = new List<Season>();

            var restRequest = new RestRequest()
            {
                Method = Method.GET,
                Resource = baseResource
            };

            while (nullSeasonCount <= 4)
            {
                var year = $"{recentYear-1}{recentYear}";
                recentYear--;
                restRequest.Resource = baseResource+year;
                var response = this.restClient.Execute(restRequest);

                try
                {
                    var validSeason = JsonConvert.DeserializeObject<StatsList>(response.Content).Season;
                    seasonList.Add(Season.duplicate(validSeason));
                }
                catch (Exception)
                {
                    nullSeasonCount++;
                }

            }
            return new Player(seasonList);
        }
    }
}