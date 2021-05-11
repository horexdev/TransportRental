using System;
using System.Windows;
using System.Windows.Controls;
using TransportRental.Car;
using TransportRental.Database;
using TransportRental.Model;

namespace TransportRental {
    /// <summary>
    /// Interaction logic for ClientCreatePage.xaml
    /// </summary>
    public partial class ClientCreatePage : Page {
        public ClientCreatePage() {
            InitializeComponent();

            BackButton.Click += BackButton_Click;
            CreateClientButton.Click += CreateClientButton_Click;
        }

        private void CreateClientButton_Click(object sender, RoutedEventArgs e) {
            var fullName = FullNameTextBox.Text;
            var phoneNumber = PhoneNumberTextBox.Text;
            var passportId = PassportIdTextBox.Text;

            if (fullName.Length <= 0 || phoneNumber.Length <= 0 || passportId.Length <= 0) return;

            var client = new Client(fullName, phoneNumber, passportId);

            if (!(Main.RentPage.VehiclesComboBox.SelectedItem is ComboBoxItem comboBoxItem)) return;

            var transport = Main.TryGetVehicle(comboBoxItem.Content.ToString());

            if (transport == null) return;

            var rentedTransport = new RentedCar(client, 0, 0, 0, 1);

            client.RentedVehicle = rentedTransport;
            client.RentedVehicle.LicensePlate = transport.LicensePlate;
            client.RentedVehicle.Car = transport;

            Main.ExpectedClients.Enqueue(client);
            Main.RentedCars.Add(rentedTransport);

            DbSync.RentedCar_Save(rentedTransport);

            Main.RentPage.RentVehicleButton.IsEnabled = true;
            Main.RentPage.CreateClientButton.IsEnabled = false;
            NavigationService?.Navigate(Main.RentPage);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e) {
            NavigationService?.Navigate(Main.RentPage);
        }
    }
}
