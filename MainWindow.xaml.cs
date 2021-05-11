using System;
using System.Globalization;
using System.IO;
using System.Windows.Navigation;
using System.Xml.Linq;
using TransportRental.Localization;

namespace TransportRental
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : NavigationWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new MainViewModel();

            if (File.Exists(Environment.CurrentDirectory + "\\settings.cfg")) {
                var xDocument = XDocument.Load(Environment.CurrentDirectory + "\\settings.cfg");

                LocalizationManager.Instance.CurrentCulture = new CultureInfo(xDocument.Element("Language").Value);

                Source = new Uri("RentPage.xaml", UriKind.Relative);
            }
            else {
                Source = new Uri("ChangeLanguage.xaml", UriKind.Relative);
            }
        }
    }
}
