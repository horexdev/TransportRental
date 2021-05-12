using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using TransportRental.Car;
using TransportRental.Database;
using TransportRental.Model;

namespace TransportRental {
    public class Main {
        public static InfoPage InfoPage;
        public static RentPage RentPage;
        public static MainPage MainPage;

        public static Admin MainAdmin { get; private set; }

        /// <summary>
        /// Список доступного траспорта для аренды
        /// </summary>
        public static List<Car.Car> Vehicles = new List<Car.Car>();

        public static List<RentedCar> RentedCars = new List<RentedCar>();

        /// <summary>
        /// Клиенты подготовленные к аренде транспорта
        /// </summary>
        public static Queue<Client> ExpectedClients = new Queue<Client>();

        /// <summary>
        /// Список клиентов
        /// </summary>
        public static List<Client> Clients = new List<Client>();

        /// <summary>
        /// Список активных клиентов
        /// </summary>
        public static List<Client> ActiveClients = new List<Client>();

        /// <summary>
        /// Загружает все возможное из БД
        /// </summary>
        public static void LoadAllData() {
            try
            {
                var cars = DbSync.Car_Get();
                var clients = DbSync.Client_Get();
                var rentedCars = DbSync.RentedCar_Get().Where(CheckTimeLeft).ToList();

                foreach (var car in cars)
                {
                    Vehicles.Add(car);

                    AddItemToVehicleComboBox(new ComboBoxItem {Content = car.Name, IsEnabled = !car.IsRented});
                }

                foreach (var rentCar in rentedCars)
                {
                    rentCar.Client = clients.FirstOrDefault(c => c.Id == rentCar.ClientId);

                    RentedCars.Add(rentCar);

                    rentCar.Car = cars.FirstOrDefault(c => c.LicensePlate == rentCar.LicensePlate);

                    var listBoxItem = new ListBoxItem
                    {
                        Content = $"{rentCar.Car?.Name}[{rentCar.Car?.LicensePlate}]"
                    };

                    MainPage.VehiclesListBox.Items.Add(listBoxItem);
                }

                foreach (var client in clients)
                {
                    client.RentedVehicle = RentedCars.FirstOrDefault(c => c.Client.PassportId == client.PassportId);

                    Clients.Add(client);

                    AddItemToClientsComboBox(new ComboBoxItem {Content = client.FullName});

                    if (client.RentedVehicle == null)
                        continue;

                    ActiveClients.Add(client);
                }

                // Обновляем количество клиентов
                UpdateClientsCount();
            }
            catch(Exception e)
            {
                var result =
                    MessageBox.Show(
                        $"Отсутствует подключение к базе данных или ошибка при загрузке данных! \r\r\nОбратитесь к системному администратору. \r\r\n{e.Message}", "",
                        MessageBoxButton.OK, MessageBoxImage.Error);


                if (result == MessageBoxResult.OK || result == MessageBoxResult.None) 
                    Process.GetCurrentProcess().Kill();
            }
        }

        /// <summary>
        /// Инициализирует админ аккаунт программы
        /// </summary>
        public static void InitializeAdminAccount() {
            var admin = DbSync.Admin_Get();

            if(admin == null) {
                DbSync.Admin_Save(new Admin("admin", ""));
                admin = DbSync.Admin_Get("admin");
            }

            MainAdmin = admin;
        }

        /// <summary>
        /// Пытается получить выбранный Item в ListBox и если Item не null, то возвращает объект Vehicle
        /// </summary>
        /// <returns></returns>
        public static Car.Car TryGetSelectedItem(ListBox listBox) {
            var selectedItem = (ListBoxItem)listBox?.SelectedItem;

            if (selectedItem == null)
                return null;

            var vehicle = Vehicles.Find(veh => $"{veh.Name}[{veh.LicensePlate}]" == selectedItem.Content.ToString());

            return vehicle;
        }

        /// <summary>
        /// Получить траспорт по его имени. Вернёт null если ничего не найдено
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Car.Car TryGetVehicle(string name) {
            return name.Length <= 0 ? null : Vehicles.FirstOrDefault(veh => veh.Name == name);
        }

        /// <summary>
        /// Инициализирует конфиг с настройками программы
        /// </summary>
        public static void InitializeConfig(string language) {
            using var streamWriter = new StreamWriter(Environment.CurrentDirectory + "\\settings.cfg");

            var xDocument = new XDocument();
            var settingsElement = new XElement("Language", language);

            xDocument.Add(settingsElement);
            xDocument.Save(streamWriter);
        }

        /// <summary>
        /// Добавить новый транспорт
        /// </summary>
        /// <param name="vehicle"></param>
        public static void AddNewTransport(Car.Car vehicle) {
            var comboBoxItem = new ComboBoxItem {
                Content = vehicle.Name
            };

            RentPage.VehiclesComboBox.Items.Add(comboBoxItem);

            Vehicles.Add(vehicle);
        }

        public static void AddItemToVehicleComboBox(ComboBoxItem item) {
            if (item == null) return;

            RentPage.VehiclesComboBox.Items.Add(item);
        }

        public static void AddItemToClientsComboBox(ComboBoxItem item) {
            if (item == null) return;

            RentPage.ClientsComboBox.Items.Add(item);
        }

        public static void RemoveRentedTransport(RentedCar rentedCar)
        {
            if (rentedCar == null) 
                return;

            var listBoxItem = new ListBoxItem();

            foreach (var item in MainPage.VehiclesListBox.Items)
            {
                var listItem = (ListBoxItem) item;

                if (listItem.Content.ToString() == $"{rentedCar.Car.Name}[{rentedCar.Car.LicensePlate}]") listBoxItem = listItem;
            }

            ActiveClients.Remove(rentedCar.Client);

            rentedCar.Client.RentedVehicle = null;

            DbSync.RentedCar_Update(rentedCar);

            UpdateClientsCount();

            RentedCars.Remove(rentedCar);
            MainPage.VehiclesListBox.Items.Remove(listBoxItem);
        }

        public static void RemoveRentedTransport(Car.Car veh)
        {
            var rentedCar = RentedCars.FirstOrDefault(c => c.LicensePlate == veh.LicensePlate);

            if (rentedCar == null) return;

            if (CheckTimeLeft(rentedCar)) {
                MessageBox.Show("Машина находится в аренде, ее нельзя убрать", "", MessageBoxButton.OK, MessageBoxImage.Warning);

                return;
            }

            RentedCars.Remove(rentedCar);
            MainPage.VehiclesListBox.Items.Remove((ListBoxItem)MainPage.VehiclesListBox.SelectedItem);
        }

        public static bool CheckIsCarBack(Car.Car car)
        {
            var result = MessageBox.Show($"Машину {car.Name}[{car.LicensePlate}] вернули назад?", "", MessageBoxButton.YesNo, MessageBoxImage.Question);

            return result == MessageBoxResult.Yes;
        }

        public static void StartCarTimer(string timerName, Car.Car vehicle) {
            Timer.Timer.StartTimer(timerName, 1000, () =>
            {
                var rentedCar = RentedCars.FirstOrDefault(c => c.LicensePlate == vehicle.LicensePlate);

                if (rentedCar == null)
                {
                    Timer.Timer.StopTimer(timerName);
                    MessageBox.Show("Таймер прекратил свою работу. \n\rCar Null Reference Exception", "", MessageBoxButton.OK, MessageBoxImage.Error);

                    return;
                }

                var leaseTerm = rentedCar.LeaseTerm;
                rentedCar.TimeLeft = leaseTerm.Subtract(DateTime.Now);

                if (CheckTimeLeft(rentedCar)) 
                    return;

                MessageBox.Show($"Аренда автомобиля {vehicle.Name} с номерами {vehicle.LicensePlate} закончилась!", "Информация");

                if (CheckIsCarBack(vehicle))
                {
                    Timer.Timer.StopTimer(timerName);

                    DbSync.RentedCar_Update(rentedCar);

                    RemoveRentedTransport(rentedCar);

                    MainPage.SetRentedFlag(vehicle, rentedCar.Client, false);
                }
                else {
                    rentedCar.LeaseTerm = DateTime.Now.AddSeconds(5);

                    DbSync.RentedCar_Update(rentedCar);

                    MessageBox.Show($"Автомобилю {rentedCar.Car.Name}[{rentedCar.Car.LicensePlate}] дано доп.время в 3 часа.", "", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            });
        }

        public static bool CheckTimeLeft(RentedCar rentedCar) {
            return rentedCar.TimeLeft.Days > 0
                || rentedCar.TimeLeft.Hours > 0
                || rentedCar.TimeLeft.Minutes > 0
                || rentedCar.TimeLeft.Seconds > 0;
        }

        public static void UpdateClientsCount(int activeClients = -1, int clients = -1) {
            RentPage.ClientCount.Content = clients == -1 ? Clients.Count.ToString() : clients.ToString();
            RentPage.ActiveClientCount.Content = activeClients == -1 ? ActiveClients.Count.ToString() : activeClients.ToString();
        }
    }
}