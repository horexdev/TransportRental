using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TransportRental.Database;

namespace TransportRental {
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Page {
        public MainPage() {
            InitializeComponent();

            InfoPageButton.Click += InfoPageButton_Click;
            RemoveTransportButton.Click += RemoveTransportButton_Click;
            BackButton.Click += BackButton_Click;
        }

        public static void SetRentedFlag(Car.Car veh, bool flag)
        {
            foreach (var item in Main.RentPage.VehiclesComboBox.Items)
            {
                var comboBoxItem = (ComboBoxItem)item;

                if (comboBoxItem.Content.ToString() != veh.Name || comboBoxItem.IsEnabled) continue;

                comboBoxItem.IsEnabled = !flag;
                veh.IsRented = flag;
                DbSync.Car_Update(veh);
                break;
            }
        }

        private void RemoveTransportButton_Click(object sender, RoutedEventArgs e) {
            var veh = Main.TryGetSelectedItem(VehiclesListBox);

            if (veh == null) return;

            var client = Main.TryGetClient(veh.Name);

            if (client == null) return;

            SetRentedFlag(veh, false);

            //Main.Clients.Remove(client);
            //Main.RentPage.ClientCount.Content = Main.Clients.Count.ToString();
            Main.RemoveRentedTransport(veh);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e) {
            NavigationService?.Navigate(Main.RentPage);
        }

        public void AddRentVehicle(Car.Car vehicle) {
            if (vehicle.IsRented) return;

            vehicle.IsRented = true;

            var listBoxItem = new ListBoxItem {
                Content = vehicle.Name + $"[{vehicle.LicensePlate}]"
            };

            VehiclesListBox.Items.Add(listBoxItem);
        }

        private void InfoPageButton_Click(object sender, RoutedEventArgs e) {
            var veh = Main.TryGetSelectedItem(VehiclesListBox);

            if (veh == null) return;

            var client = Main.TryGetClient(veh.Name);

            if (client == null) return;

            Main.InfoPage = new InfoPage(client);

            NavigationService?.Navigate(Main.InfoPage);
        }
    }
}
