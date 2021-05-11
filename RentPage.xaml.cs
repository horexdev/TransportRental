using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TransportRental.Database;

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

            Main.LoadAllData();
            Main.InitializeAdminAccount();

            VehiclesComboBox.SelectedItem = VehiclesComboBox.Items[0];

            MainPageButton.Click += MainPageButton_Click;
            RentVehicleButton.Click += RentVehicleButton_Click;
            AdminPanelButton.Click += AdminPanelButton_Click;
            VehiclesComboBox.SelectionChanged += VehiclesComboBox_SelectionChanged;
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
            if (Main.MainAdmin.IsLogged) {
                Main.MainAdmin.IsLogged = false;
                LogoutButton.Visibility = Visibility.Hidden;
            }
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
            var comboBoxItem = GetSelectedComboBoxItem();

            if (comboBoxItem == null) return;

            if (comboBoxItem.Content.ToString() == "-") return;

            comboBoxItem.IsEnabled = false;

            var vehicle = Main.TryGetVehicle(comboBoxItem.Content.ToString());

            if (vehicle == null) return;
                 
            vehicle.RentPrice = RentPriceTextBox.Text != vehicle.BasicRent.ToString()
                ? Convert.ToInt32(RentPriceTextBox.Text)
                : vehicle.BasicRent;

            VehiclesComboBox.SelectedItem = VehiclesComboBox.Items[0];

            Main.MainPage.AddRentVehicle(vehicle);

            vehicle.IsRented = true;

            Main.Clients.Add(Main.ExpectedClients.Dequeue());
            Main.RentPage.ClientCount.Content = Main.Clients.Count.ToString();

            RentVehicleButton.IsEnabled = false;
            CreateClientButton.IsEnabled = true;

            DbSync.Car_Update(vehicle);

            var timerName = vehicle.Name + "_" + vehicle.LicensePlate;

            Timer.Timer.StartTimer(timerName, 900, () =>
            {
                var rentedCar = Main.RentedCars.FirstOrDefault(c => c.LicensePlate == vehicle.LicensePlate);

                if (rentedCar == null)
                {
                    Timer.Timer.StopTimer(timerName);
                    MessageBox.Show("Таймер прекратил свою работу. \n\rNull Reference Exception", "", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var leaseTerm = rentedCar.LeaseTerm;
                rentedCar.TimeLeft = leaseTerm.Subtract(DateTime.Now);

                if (rentedCar.TimeLeft.Days > 0
                    || rentedCar.TimeLeft.Hours > 0
                    || rentedCar.TimeLeft.Minutes > 0
                    || rentedCar.TimeLeft.Seconds > 0) return;

                DbSync.RentedCar_Update(rentedCar);
                Main.RemoveRentedTransport(rentedCar);

                Timer.Timer.StopTimer(timerName);
                MessageBox.Show($"Аренда автомобиля {vehicle.Name} с номерами {vehicle.LicensePlate} закончилась!", "Информация");
                if (Main.CheckIsCarBack(vehicle))
                {
                    MainPage.SetRentedFlag(vehicle, false);
                }
            });
        }

        /// <summary>
        /// Срабатывает когда меняется выбранный элемент в выпадающем списке
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VehiclesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            var comboxBoxItem = GetSelectedComboBoxItem();

            if (comboxBoxItem == null) return;

            if(comboxBoxItem.Content.ToString() == "-") {
                if (EditRentPriceCheckBox.IsEnabled) {
                    EditRentPriceCheckBox.IsEnabled = false;
                    EditRentPriceCheckBox.IsChecked = false;
                    RentPriceTextBox.Text = "";
                    return;
                }
            }

            var vehicle = Main.TryGetVehicle(comboxBoxItem.Content.ToString());

            if (vehicle == null) return;

            EditRentPriceCheckBox.IsEnabled = true;
            RentPriceTextBox.Text = vehicle.BasicRent.ToString();
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
        private ComboBoxItem GetSelectedComboBoxItem() {
            return (ComboBoxItem) VehiclesComboBox.SelectedItem;
        }
    }
}