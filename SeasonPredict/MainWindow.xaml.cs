using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace SeasonPredict
{
    public partial class MainWindow : Window
    {
        public TeamCollection TeamListObjects;

        public MainWindow()
        {
            TeamListObjects = new TeamCollection();
            InitializeComponent();
            DataContext = this;
            teamsList.ItemsSource = TeamListObjects;

            Team obj = teamsList.SelectedItem as Team;

        }
        public async void sendRequest_Click(object sender, RoutedEventArgs e)
        {
            if(playersList.SelectedItem != null)
            {
                if (!string.IsNullOrEmpty(expectedSeasonBox.Text))
                    expectedSeasonBox.Text = string.Empty;


                Player p = await ApiLoader.LoadPlayer((playersList.SelectedItem as Roster2).Id);

                expectedSeasonBox.Text = (playersList.SelectedItem as Roster2).Name + "\n" + p;
            }
          }

        private void chooseTeam_Click(object sender, RoutedEventArgs e)
        {
            playersList.ItemsSource = (teamsList.SelectedItem as Team).Roster.Roster;
        }
    }
}
