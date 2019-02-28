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
            DataContext = this;
            teamsList.ItemsSource = TeamListObjects;
        }
        public async void sendRequest_Click(object sender, RoutedEventArgs e)
        {
            if(playersList.SelectedItem != null)
            {
                //Check if the player hasn't already been loaded (avoiding to call the api again)
                if (!PlayersMemory.Any(p => p.Id.Equals((playersList.SelectedItem as Roster2).Id)))
                {
                    //When api is called, all GUI components are disabled to prevent user based problems
                    SetComponentsAvailability(false);

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

        private void SetComponentsAvailability(bool enabled)
        {
            teamsList.IsEnabled = enabled;
            chooseTeam.IsEnabled = enabled;
            playersList.IsEnabled = enabled;
            sendRequest.IsEnabled = enabled;
        }

        private void chooseTeam_Click(object sender, RoutedEventArgs e)
        {
            playersList.ItemsSource = (teamsList.SelectedItem as Team).PersonList;
        }
    }
}
