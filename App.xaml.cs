using System;
using System.IO;
using System.Windows;
using TransportRental.Localization;

namespace TransportRental
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);

            LocalizationManager.Instance.LocalizationProvider = new ResxLocalizationProvider();
        }
    }
}
