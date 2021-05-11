using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace TransportRental {
    /// <summary>
    /// Логика взаимодействия для ChangeLanguage.xaml
    /// </summary>
    public partial class ChangeLanguage : Page {
        public ChangeLanguage() {
            InitializeComponent();

            BackButton.Visibility = File.Exists(Environment.CurrentDirectory + "\\settings.cfg")
                ? Visibility.Visible
                : Visibility.Hidden;

            RussianLanguageButton.Click += RussianLanguageButton_Click;
            UsaLanguageButton.Click += UsaLanguageButton_Click;
            BackButton.Click += BackButton_Click;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e) {
            NavigationService?.Navigate(Main.RentPage);
        }

        private void UsaLanguageButton_Click(object sender, System.Windows.RoutedEventArgs e) {
            SetLanguage("en-US");
        }

        private void RussianLanguageButton_Click(object sender, System.Windows.RoutedEventArgs e) {
            SetLanguage("ru-RU");
        }

        private void SetLanguage(string language) {
            Main.InitializeConfig(language);
            Localization.LocalizationManager.Instance.CurrentCulture = new CultureInfo(language);
            Main.RentPage ??= new RentPage();
            NavigationService?.Navigate(Main.RentPage);
        }
    }
}
