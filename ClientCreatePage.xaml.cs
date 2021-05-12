using System;
using System.Linq;
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

            if (Main.Clients.Count(c =>
                c.FullName == fullName || c.PassportId == passportId || c.PhoneNumber == phoneNumber) > 0) {

                MessageBox.Show("Клиент с такими данными уже существует!", "", MessageBoxButton.OK, MessageBoxImage.Error);

                return;
            }

            var client = new Client(fullName, phoneNumber, passportId);

            Main.RentPage.ClientsComboBox.Items.Add(new ComboBoxItem { Content = client.FullName});

            Main.Clients.Add(client);
            Main.UpdateClientsCount();

            NavigationService?.Navigate(Main.RentPage);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e) {
            NavigationService?.Navigate(Main.RentPage);
        }
    }
}
