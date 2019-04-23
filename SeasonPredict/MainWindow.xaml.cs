using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SeasonPredict
{
    public partial class MainWindow : Window
    {
        public TeamCollection TeamsCollection;

        public List<Player>
            PlayersMemory; //Program instance players memory: stores players whose next season is already estimated during this instance of the program

        public static ApiLoader loader;
        public MainWindow()
        {
            loader = new ApiLoader();
            TeamsCollection = new TeamCollection();
            PlayersMemory = new List<Player>();
            InitializeComponent();
            teamsListbox.ItemsSource = TeamsCollection;
            sendRequestButton.IsEnabled = false;
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void SendRequest_Click(object sender, RoutedEventArgs e)
        {
            if (!playersListbox.SelectedItem.Equals(null))
            {
                //Check if the player hasn't already been loaded (avoiding to call the api again)
                if (!PlayersMemory.Any(p => p.Id.Equals((playersListbox.SelectedItem as Roster2).Id)))
                {
                    //When api is called, all GUI components are disabled to prevent user selection change problems
                    setComponentsAvailability(false);

                    expectedSeasonBox.Text = "Calculating...";

                    var player = new Player(loader.loadPlayer((playersListbox.SelectedItem as Roster2).Id),(playersListbox.SelectedItem as Roster2).Name, (playersListbox.SelectedItem as Roster2).Id);

                    PlayersMemory.Add(Player.duplicate(player));

                    expectedSeasonBox.Text = player.ToString();

                    //GUI components are enabled for the user
                    setComponentsAvailability(true);
                }
                else
                {
                    expectedSeasonBox.Text = PlayersMemory.First(p => p.Id.Equals((playersListbox.SelectedItem as Roster2).Id)).ToString();
                }
            }
        }

        /// <summary>
        ///     Makes GUI elements enabled or not according to availability parameter
        /// </summary>
        /// <param name="availability">Boolean value assigned to the IsEnabled property of graphical interface elements</param>
        private void setComponentsAvailability(bool availability)
        {
            teamsListbox.IsEnabled = availability;
            playersListbox.IsEnabled = availability;
            sendRequestButton.IsEnabled = availability;
        }

        /// <summary>
        ///     teamsListbox SelectionChanged event handling method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TeamsList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            playersListbox.ItemsSource = (teamsListbox.SelectedItem as Team).Roster.Roster;
            sendRequestButton.IsEnabled = false; //Since playersListbox isn't focused, the calculation button is disabled
        }

        /// <summary>
        ///     playersListbox SelectionChanged event handling method
        ///     When selection is changed and the button is disabled, it gets enabled
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlayersListbox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!sendRequestButton.IsEnabled)
            {
                sendRequestButton.IsEnabled = true;
            }
        }
    }
}