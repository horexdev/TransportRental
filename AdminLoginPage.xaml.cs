using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TransportRental {
    /// <summary>
    /// Interaction logic for AdminLoginPage.xaml
    /// </summary>
    public partial class AdminLoginPage : Page {
        private static AdminLoginPage _loginPage;

        public static AdminLoginPage LoginPage {
            get {
                _loginPage ??= new AdminLoginPage();
                return _loginPage;
            }
        }

        public AdminLoginPage() {
            InitializeComponent();

            SignInButton.Click += SignInButton_Click;
            BackButton.Click += BackButton_Click;
        }

        private void SignInButton_Click(object sender, RoutedEventArgs e) {
            var login = LoginTextBox.Text;
            var password = PasswordTextBox.Text;

            if (Main.MainAdmin.Login == login && Main.MainAdmin.Password == password) {
                Main.MainAdmin.IsLogged = true;
                Main.RentPage.LogoutButton.Visibility = Visibility.Visible;
                LoginTextBox.Text = "";
                PasswordTextBox.Text = "";
                NavigationService?.Navigate(AdminPanelPage.Panel);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e) {
            NavigationService?.Navigate(Main.RentPage);
        }
    }
}
