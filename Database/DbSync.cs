using System.Collections.Generic;
using System.Linq;
using TransportRental.Model;

namespace TransportRental.Database {
    public class DbSync {
        #region Admin

        public static void Admin_Save(Admin admin)
        {
            using var db = new ServerDbContext();

            if (admin == null) return;

            db.Admins.Add(admin);

            db.SaveChanges();
        }

        public static Admin Admin_Get(string login)
        {
            using var db = new ServerDbContext();

            return db.Admins.FirstOrDefault(a => a.Login == login);
        }

        /// <summary>
        /// Возвращает самый первый админ аккаунт
        /// </summary>MySql.Data.MySqlClient.MySqlException
        /// <returns></returns>
        public static Admin Admin_Get() {
            try {
                using var db = new ServerDbContext();

                var admins = db.Admins.ToList();

                return admins.Count == 0 ? null : admins[0];
            }
            catch(MySql.Data.MySqlClient.MySqlException) {
                return new Admin("admin", "");
            }
        }

        #endregion

        #region Client

        public static void Client_Save(Client client) {
            using var db = new ServerDbContext();

            if (client == null) return;

            db.Clients.Add(client);

            db.SaveChanges();
        }

        public static Client Client_Get(string fullName) {
            using var db = new ServerDbContext();

            return db.Clients.FirstOrDefault(c => c.FullName == fullName);
        }

        public static void Client_Remove(Client client)
        {
            using var db = new ServerDbContext();

            if (client == null) return;

            db.Clients.Remove(client);
            db.SaveChanges();
        }

        /// <summary>
        /// Возвращает список клиентов
        /// </summary>
        /// <returns></returns>
        public static ICollection<Client> Client_Get() {
            using var db = new ServerDbContext();

            return db.Clients.ToList();
        }

        #endregion

        #region Car

        public static void Car_Save(Car.Car transport) {
            using var db = new ServerDbContext();

            if (transport == null) return;

            db.Transports.Add(transport);

            db.SaveChanges();
        }

        public static void RentedCar_Save(Car.RentedCar rentedCar) {
            using var db = new ServerDbContext();

            if(rentedCar == null) return;

            db.RentedTransports.Add(rentedCar);

            db.SaveChanges();
        }

        public static void Car_Update(Car.Car car) {
            using var db = new ServerDbContext();

            if (car == null) return;

            db.Transports.Update(car);
            db.SaveChanges();
        }

        public static void RentedCar_Update(Car.RentedCar car)
        {
            using var db = new ServerDbContext();

            if (car == null) return;

            db.RentedTransports.Update(car);
            db.SaveChanges();
        }

        public static void RentedCar_Remove(Car.RentedCar car) {
            using var db = new ServerDbContext();

            if (car == null) return;

            db.RentedTransports.Remove(car);
            db.SaveChanges();
        }

        public static Car.Car Car_Get(string name) {
            using var db = new ServerDbContext();

            return db.Transports.FirstOrDefault(t => t.Name == name);
        }

        /// <summary>
        /// Возвращает список автомобилей
        /// </summary>
        /// <returns></returns>
        public static ICollection<Car.Car> Car_Get() {
            using var db = new ServerDbContext();

            return db.Transports.ToList();
        }

        /// <summary>
        /// Возвращает список арендованных авто
        /// </summary>
        /// <returns></returns>
        public static ICollection<Car.RentedCar> RentedCar_Get() {
            using var db = new ServerDbContext();

            return db.RentedTransports.ToList();
        }

        #endregion
    }
}