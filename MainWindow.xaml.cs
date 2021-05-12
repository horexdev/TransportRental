using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Navigation;
using System.Xml.Linq;
using TransportRental.Localization;

namespace TransportRental
{
    public partial class MainWindow : NavigationWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new MainViewModel();

            if (File.Exists(Environment.CurrentDirectory + "\\settings.cfg")) {
                var xDocument = XDocument.Load(Environment.CurrentDirectory + "\\settings.cfg");

                var languageValue = xDocument.Element("Language")?.Value;

                if (languageValue == null) {
                    var result = MessageBox.Show("Ошибка при загрузке языковых настроек!  \r\r\nОбратитесь к системному администратору. \r\r\nValue null", "", MessageBoxButton.OK, MessageBoxImage.Error);

                    if (result == MessageBoxResult.OK || result == MessageBoxResult.None)
                        Process.GetCurrentProcess().Kill();

                    return;
                }

                LocalizationManager.Instance.CurrentCulture = new CultureInfo(languageValue);

                Source = new Uri("RentPage.xaml", UriKind.Relative);
            }
            else {
                Source = new Uri("ChangeLanguage.xaml", UriKind.Relative);
            }
        }
    }
}
