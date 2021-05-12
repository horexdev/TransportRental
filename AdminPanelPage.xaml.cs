using System;
using System.Windows;
using System.Windows.Controls;
using TransportRental.Database;

namespace TransportRental {
    /// <summary>
    /// Interaction logic for AdminPanelPage.xaml
    /// </summary>
    public partial class AdminPanelPage : Page {
        private static AdminPanelPage _panel;

        public static AdminPanelPage Panel {
            get {
                _panel ??= new AdminPanelPage();
                return _panel;
            }
        }

        public AdminPanelPage() {
            InitializeComponent();

            AddVehicleButton.Click += AddVehicleButton_Click;
            BackButton.Click += BackButton_Click;
        }

        private void AddVehicleButton_Click(object sender, RoutedEventArgs e) {
            try
            {
                var type = TypeTransportTextBox.Text;
                var licensePlate = LicensePlateTextBox.Text;
                var name = NameTransportTextBox.Text;
                var basicPrice = Convert.ToInt32(RentPriceTextBox.Text);

                TypeTransportTextBox.Text = "";
                LicensePlateTextBox.Text = "";
                NameTransportTextBox.Text = "";
                RentPriceTextBox.Text = "";

                if (Main.Vehicles.Exists(veh => veh.Name == name)) return;

                var transport = new Car.Car(name, licensePlate, type, basicPrice);

                Main.AddNewTransport(transport);
                DbSync.Car_Save(transport);
            }
            catch
            {
                //ignored
            }
        }

        private void BackButton_Click(object sender, System.Windows.RoutedEventArgs e) {
            NavigationService?.Navigate(Main.RentPage);
        }
    }
}
