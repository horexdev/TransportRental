using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TransportRental.Model {
    public class Admin {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Логин администратора
        /// </summary>
        public string Login { get; set; }

        /// <summary>
        /// Пароль
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Авторизован ли администратор в данный момент
        /// </summary>
        [NotMapped]
        public bool IsLogged { get; set; }

        public Admin() {

        }

        public Admin(string login, string password) {
            Login = login;
            Password = password;
        }
    }
}