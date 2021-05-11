using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Accessibility;
using TransportRental.Model;

namespace TransportRental.Car {
    public class RentedCar {
        [Key]
        public int Id { get; set; }

        public string LicensePlate { get; set; }

        public int ClientId { get; set; }

        public Client Client { get; set; }

        [NotMapped]
        public Car Car { get; set; }

        /// <summary>
        /// Дата и время аренды
        /// </summary>
        public DateTime RentalTime { get; set; }

        /// <summary>
        /// Дата и время окончания аренды
        /// </summary>
        public DateTime LeaseTerm { get; set; }

        public TimeSpan TimeLeft { get; set; }

        public RentedCar() {

        }

        public RentedCar(Client client, int days, int hours, int minutes, int seconds = 0) {
            Client = client;
            RentalTime = DateTime.Now;
            LeaseTerm = DateTime.Now.AddDays(days).AddHours(hours).AddMinutes(minutes).AddSeconds(seconds);
        }
    }
}