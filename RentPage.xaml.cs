using System;
using System.Linq;
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using TransportRental.Car;
using TransportRental.Database;
using TransportRental.Model;

namespace TransportRental {
    /// <summary>
    /// Interaction logic for RentPage.xaml
    /// </summary>
    public partial class RentPage : Page {
        public RentPage() {
            InitializeComponent();

            Main.RentPage ??= this;
            Main.MainPage ??= new MainPage();

            VehiclesComboBox.Items.Add(new ComboBoxItem { Content = "-" });
            ClientsComboBox.Items.Add(new ComboBoxItem { Content = "-" });

            Main.LoadAllData();
            Main.InitializeAdminAccount();

            VehiclesComboBox.SelectedItem = VehiclesComboBox.Items[0];
            ClientsComboBox.SelectedItem = ClientsComboBox.Items[0];

            MainPageButton.Click += MainPageButton_Click;
            RentVehicleButton.Click += RentVehicleButton_Click;
            AdminPanelButton.Click += AdminPanelButton_Click;
            VehiclesComboBox.SelectionChanged += VehiclesComboBox_SelectionChanged;
            ClientsComboBox.SelectionChanged += ClientsComboBox_SelectionChanged;
            EditRentPriceCheckBox.Checked += EditRentPriceCheckBox_Checked;
            EditRentPriceCheckBox.Unchecked += EditRentPriceCheckBox_Unchecked;
            LogoutButton.Click += LogoutButton_Click;
            SettingsButton.Click += SettingsButton_Click;
            CreateClientButton.Click += CreateClientButton_Click;
        }

        private void CreateClientButton_Click(object sender, RoutedEventArgs e) {
            if (VehiclesComboBox.SelectedItem == VehiclesComboBox.Items[0]) return;

            NavigationService?.Navigate(new ClientCreatePage());
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e) {
            NavigationService?.Navigate(new ChangeLanguage());
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e) {
            if (!Main.MainAdmin.IsLogged) return;

            Main.MainAdmin.IsLogged = false;

            LogoutButton.Visibility = Visibility.Hidden;
        }

        private void AdminPanelButton_Click(object sender, RoutedEventArgs e) {
            if (Main.MainAdmin.IsLogged) {
                NavigationService?.Navigate(AdminPanelPage.Panel);
                return;
            }

            NavigationService?.Navigate(AdminLoginPage.LoginPage);
        }

        private void EditRentPriceCheckBox_Unchecked(object sender, RoutedEventArgs e) {
            RentPriceTextBox.IsEnabled = false;
        }

        private void EditRentPriceCheckBox_Checked(object sender, RoutedEventArgs e) {
            RentPriceTextBox.IsEnabled = true;
        }

        /// <summary>
        /// Срабатывает когда нажимается кнопка аренды транспорта
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RentVehicleButton_Click(object sender, RoutedEventArgs e) {
            var comboBoxItem = GetSelectedComboBoxItem(VehiclesComboBox);
            var clientsComboBoxItem = GetSelectedComboBoxItem(ClientsComboBox);

            if (comboBoxItem == null) return;

            if (comboBoxItem.Content.ToString() == "-") return;

            comboBoxItem.IsEnabled = false;
            clientsComboBoxItem.IsEnabled = false;

            var vehicle = Main.TryGetVehicle(comboBoxItem.Content.ToString());

            if (vehicle == null) return;
                 
            vehicle.RentPrice = RentPriceTextBox.Text != vehicle.BasicRent.ToString()
                ? Convert.ToInt32(RentPriceTextBox.Text)
                : vehicle.BasicRent;

            VehiclesComboBox.SelectedItem = VehiclesComboBox.Items[0];
            ClientsComboBox.SelectedItem = ClientsComboBox.Items[0];

            Main.MainPage.AddRentVehicle(vehicle);

            vehicle.IsRented = true;

            Client client;

            if (Main.ExpectedClients.Count == 0) {
                client = Main.Clients.FirstOrDefault(c => c.FullName == clientsComboBoxItem.Content.ToString());

                if (client == null)
                {
                    MessageBox.Show("Техническая ошибка. \r\r\nОбратитесь к системному администратору \r\r\nClient null", "", MessageBoxButton.OK, MessageBoxImage.Error);

                    return;
                }

                var rentedTransport = new RentedCar(client, 0, 0, 0, 5);

                client.RentedVehicle = rentedTransport;
                client.RentedVehicle.LicensePlate = vehicle.LicensePlate;
                client.RentedVehicle.Car = vehicle;

                Main.RentedCars.Add(rentedTransport);

                DbSync.RentedCar_Update(rentedTransport);
            }
            else {
                client = Main.ExpectedClients.Dequeue();

                Main.Clients.Add(client);
            }

            Main.ActiveClients.Add(client);

            Main.UpdateClientsCount();

            RentVehicleButton.IsEnabled = false;
            CreateClientButton.IsEnabled = true;

            DbSync.Car_Update(vehicle);

            var timerName = vehicle.Name + "_" + vehicle.LicensePlate;

            // Запускаем таймер
            Main.StartCarTimer(timerName, vehicle);
        }

        /// <summary>
        /// Срабатывает когда меняется выбранный элемент в выпадающем списке
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VehiclesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            var comboBoxItem = GetSelectedComboBoxItem(VehiclesComboBox);

            if (comboBoxItem == null) 
                return;

            if(comboBoxItem.Content.ToString() == "-") {
                RentVehicleButton.IsEnabled = false;

                if (EditRentPriceCheckBox.IsEnabled) {
                    EditRentPriceCheckBox.IsEnabled = false;
                    EditRentPriceCheckBox.IsChecked = false;

                    RentPriceTextBox.Text = "";

                    return;
                }
            }
            else {
                RentVehicleButton.IsEnabled = GetSelectedComboBoxItem(ClientsComboBox).Content.ToString() != "-";
            }

            var vehicle = Main.TryGetVehicle(comboBoxItem.Content.ToString());

            if (vehicle == null) 
                return;

            EditRentPriceCheckBox.IsEnabled = true;
            RentPriceTextBox.Text = vehicle.BasicRent.ToString();
        }

        private void ClientsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            var comboBoxItem = GetSelectedComboBoxItem(ClientsComboBox);

            if (comboBoxItem == null) 
                return;

            if (comboBoxItem.Content.ToString() != "-") {
                CreateClientButton.IsEnabled = false;

                RentVehicleButton.IsEnabled = GetSelectedComboBoxItem(VehiclesComboBox).Content.ToString() != "-";
            }
            else {
                RentVehicleButton.IsEnabled = false;
                CreateClientButton.IsEnabled = true;
            }
        }

        /// <summary>
        /// Срабатывает когда нажимается кнопка перехода на главную страницу
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainPageButton_Click(object sender, RoutedEventArgs e) {
            NavigationService?.Navigate(Main.MainPage);
        }

        /// <summary>
        /// Получить выбранный элемент в выпадающем списке
        /// </summary>
        /// <returns></returns>
        private ComboBoxItem GetSelectedComboBoxItem(Selector comboBox) {
            return (ComboBoxItem) comboBox.SelectedItem;
        }
    }
}