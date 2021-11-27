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
            PhoneNumberTextBox.PreviewTextInput += PhoneNumberTextBox_PreviewTextInput;
        }

        private void PhoneNumberTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (PhoneNumberTextBox.Text.Contains('+') == false & e.Text[0] != '+')
                e.Handled = true;
            else if (PhoneNumberTextBox.Text.Contains('+') && e.Text[0] == '+')
                e.Handled = true;

            if (char.IsLetter(e.Text[0]))
                e.Handled = true;

            if (e.Text.Length > 11)
                e.Handled = true;

            if (PhoneNumberTextBox.Text.Length > 11)
                e.Handled = true;
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

            DbSync.Client_Save(client);

            NavigationService?.Navigate(Main.RentPage);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e) {
            NavigationService?.Navigate(Main.RentPage);
        }
    }
}
