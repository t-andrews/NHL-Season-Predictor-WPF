using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Documents;

namespace SeasonPredict
{
    public partial class MainWindow : Window
    {
        public TeamCollection TeamListObjects;
        public List<Player> PlayersMemory;

        public MainWindow()
        {
            TeamListObjects = new TeamCollection();
            PlayersMemory = new List<Player>();
            InitializeComponent();
            teamsList.ItemsSource = TeamListObjects;
        }
        public async void SendRequest_Click(object sender, RoutedEventArgs e)
        {
            if(playersList.SelectedItem != null)
            {
                //Check if the player hasn't already been loaded (avoiding to call the api again)
                if (!PlayersMemory.Any(p => p.Id.Equals((playersList.SelectedItem as Roster2).Id)))
                {
                    //When api is called, all GUI components are disabled to prevent user based problems
                    SetComponentsAvailability(false);

                    expectedSeasonBox.Text = "Calculating...";

                    Player p = new Player(await ApiLoader.LoadPlayer((playersList.SelectedItem as Roster2).Id), (playersList.SelectedItem as Roster2).Name, (playersList.SelectedItem as Roster2).Id);
                    
                    PlayersMemory.Add(Player.Duplicate(p));

                    expectedSeasonBox.Text = p.ToString();

                    //GUI components are enabled for the user
                    SetComponentsAvailability(true);
                }
                else
                    expectedSeasonBox.Text = PlayersMemory.First(p => p.Id.Equals((playersList.SelectedItem as Roster2).Id)).ToString();
            }
          }

        private void SetComponentsAvailability(bool availability)
        {
            teamsList.IsEnabled = availability;
            chooseTeam.IsEnabled = availability;
            playersList.IsEnabled = availability;
            sendRequest.IsEnabled = availability;
        }

        private void ChooseTeam_Click(object sender, RoutedEventArgs e)
        {
            playersList.ItemsSource = (teamsList.SelectedItem as Team).PersonList;
        }
    }
}
