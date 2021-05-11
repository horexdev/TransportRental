using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TransportRental.Car
{
    public class Car
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Номера машины
        /// </summary>
        public string LicensePlate { get; set; }

        /// <summary>
        /// Тип
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Базовая цена за аренду
        /// </summary>
        public int BasicRent { get; set; }

        /// <summary>
        /// Конечная цена за аренду
        /// </summary>
        public int RentPrice { get; set; }

        /// <summary>
        /// Арендован ли транспорт
        /// </summary>
        public bool IsRented { get; set; }

        public Car() {

        }

        public Car(string name, string licensePlate, string type, int basicRent)
        {
            Name = name;
            LicensePlate = licensePlate;
            Type = type;
            BasicRent = basicRent;
        }
    }
}