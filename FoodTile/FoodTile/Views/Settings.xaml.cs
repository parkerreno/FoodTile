using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Background;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.Credentials;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace FoodTile.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Settings : Page
    {
        private readonly IPropertySet localSettings = ApplicationData.Current.LocalSettings.Values;

        public Settings()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is int)
            {
                Pivot.SelectedIndex = (int) e.Parameter;
            }
            LiveTileToggle.IsEnabled = App.MainViewModel.SignOnSaved;
            NotificationsToggle.IsEnabled = App.MainViewModel.SignOnSaved;
            VerifyIdToggle.IsEnabled = App.MainViewModel.SignOnSaved;

            LiveTileToggle.IsOn = (bool) (localSettings[App.RL.GetString("TileSettingString")] ?? false);
            NotificationsToggle.IsOn = (bool) (localSettings[App.RL.GetString("NotificationSettingString")] ?? false);
            VerifyIdToggle.IsOn = (bool)(localSettings[App.RL.GetString("VerifyIdSettingString")] ?? false);

            if (App.MainViewModel.SignOnSaved)
            {
                string taskName = App.RL.GetString("BackgroundTaskName");
                if (!IsTaskRegistered(taskName))
                {
                    var builder = new BackgroundTaskBuilder();
                    builder.Name = taskName;
                    builder.TaskEntryPoint = "BackgroundUpdater.BalanceCheck";
                    builder.SetTrigger(new TimeTrigger(15, false));
                    //builder.AddCondition(new SystemCondition(SystemConditionType.InternetAvailable)); //this doesn't work...

                    await BackgroundExecutionManager.RequestAccessAsync();

                    BackgroundTaskRegistration task = builder.Register();

                }
            }

            base.OnNavigatedTo(e);
        }

        private void LiveTileToggle_OnToggled(object sender, RoutedEventArgs e)
        {
            var toggled = sender as ToggleSwitch;
            localSettings[App.RL.GetString("TileSettingString")] = toggled.IsOn;
        }

        private void NotificationsToggle_OnToggled(object sender, RoutedEventArgs e)
        {
            var toggled = sender as ToggleSwitch;
            localSettings[App.RL.GetString("NotificationSettingString")] = toggled.IsOn;
        }

        private void VerifyIdToggle_OnToggled(object sender, RoutedEventArgs e)
        {
            var toggled = sender as ToggleSwitch;
            localSettings[App.RL.GetString("VerifyIdSettingString")] = toggled.IsOn;
        }

        private bool IsTaskRegistered(string taskName)
        {
            return BackgroundTaskRegistration.AllTasks.Any(task => task.Value.Name == taskName);
        }

        private void LogOut_Tapped(object sender, TappedRoutedEventArgs e)
        {
            PasswordVault vault = new PasswordVault();

            var creds = vault.FindAllByResource(App.RL.GetString("CredResName"));
            foreach (var cred in creds)
            {
                vault.Remove(cred);
            }

            App.Current.Exit();
        }

        private async void Email_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var emailUri = new Uri(@"mailto:apps@parkerreno.net?subject=FoodTile");
            await Windows.System.Launcher.LaunchUriAsync(emailUri);
        }

        private async void Donate_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var donateUri = new Uri(@"http://paypal.me/parkerreno");
            await Windows.System.Launcher.LaunchUriAsync(donateUri);
        }

        private int taps = 0;
        private void Debug_Tapped(object sender, TappedRoutedEventArgs e)
        {
            taps++;
            if (taps > 10)
            {
                Frame.Navigate(typeof (FoodTile.MainPage));
            }
        }
    }
}
