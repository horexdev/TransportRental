using System.Windows.Controls;
using TransportRental.Model;

namespace TransportRental {
    /// <summary>
    /// Логика взаимодействия для InfoPage.xaml
    /// </summary>
    public partial class InfoPage : Page {
        public InfoPage() {
            InitializeComponent();

            BackButton.Click += BackButton_Click;
        }

        public InfoPage(Client client) {
            InitializeComponent();

            BackButton.Click += BackButton_Click;

            VehicleName.Content += $": {client.RentedVehicle.Car.Name}";
            RentPrice.Content += $": {client.RentedVehicle.Car.RentPrice}";
            DateTime.Content += $": {client.RentedVehicle.RentalTime}";

            ClientFullname.Content += $": {client.FullName}";
            ClientPhoneNumber.Content += $": {client.PhoneNumber}";
            ClientPassportId.Content += $": {client.PassportId}";
        }

        private void BackButton_Click(object sender, System.Windows.RoutedEventArgs e) {
            NavigationService?.Navigate(Main.MainPage);
        }
    }
}
