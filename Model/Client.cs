using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TransportRental.Model {
    /// <summary>
    /// Класс с информацией о клиенте
    /// </summary>
    public class Client {
        [Key]
        [ForeignKey("RentedCar")]
        public int Id { get; set; }

        /// <summary>
        /// ФИО
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Номер телефона
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Данные паспорта
        /// </summary>
        public string PassportId { get; set; }

        /// <summary>
        /// Арендованный транспорт
        /// </summary>
        [NotMapped]
        public Car.RentedCar RentedVehicle { get; set; }

        public Client() {
            
        }

        public Client(string fullname, string phoneNumber, string passportId) {
            FullName = fullname;
            PhoneNumber = phoneNumber;
            PassportId = passportId;
        }
    }
}