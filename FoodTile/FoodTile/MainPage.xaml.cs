using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using NotificationsExtensions.Tiles;
using NotificationsExtensions.Toasts;
using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;
using Windows.ApplicationModel.Background;
using Windows.UI.Popups;
using Windows.Storage;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace FoodTile
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            
        }

        private void Button_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var dining = App.MainViewModel.HfsData.resident_dining;
            var term = App.MainViewModel.TermInfo;
            var bindingContent = new TileBindingContentAdaptive();
            var items = bindingContent.Children;

            items.Add(new TileText()
            {
                Text = $"Balance: ${dining.balance:0.00}",
                Style = TileTextStyle.Body
            });

            items.Add(new TileText()
            {
                Text = $"Days: {term.FullDaysRemaining}",
                Style = TileTextStyle.Caption
            });

            items.Add(new TileText()
            {
                Text = $"Avg/Day: ${dining.balance/term.FullDaysRemaining:0.00}",
                Style = TileTextStyle.Caption
            });

            var tileBinding = new TileBinding();

            tileBinding.Content = bindingContent;
            tileBinding.DisplayName = "FoodTile";
            tileBinding.Branding = TileBranding.NameAndLogo;

            var content = new TileContent()
            {
                Visual = new TileVisual()
                {
                    TileSmall = tileBinding,
                    TileMedium = tileBinding,
                    TileWide = tileBinding,
                    TileLarge = tileBinding
                }
            };

            XmlDocument doc = content.GetXml();

            var note = new TileNotification(doc);
            var updater = TileUpdateManager.CreateTileUpdaterForApplication();

            updater.Update(note);
        }

        private void Button_Tapped_1(object sender, TappedRoutedEventArgs e)
        {
            var toastContent = new ToastContent()
            {
                Visual = new ToastVisual()
                {
                    TitleText = new ToastText()
                    {
                        Text = "Balance Updated"
                    },
                    BodyTextLine1 = new ToastText()
                    {
                        Text = "New balance: $568.12"
                    },
                    BodyTextLine2 = new ToastText()
                    {
                        Text = "Average spend over 47 days: $12.67"
                    }
                },
                Scenario = ToastScenario.Default
            };

            var toast = new ToastNotification(toastContent.GetXml());

            var note = ToastNotificationManager.CreateToastNotifier();
            note.Show(toast);
        }

        private async void Button_Tapped_2(object sender, TappedRoutedEventArgs e)
        {
            const string TASK_NAME = "balanceCheck";
            if (!IsTaskRegistered(TASK_NAME))
            {
                var builder = new BackgroundTaskBuilder();
                builder.Name = TASK_NAME;
                builder.TaskEntryPoint = "BackgroundUpdater.BalanceCheck";
                builder.SetTrigger(new TimeTrigger(15, false));
                //builder.AddCondition(new SystemCondition(SystemConditionType.InternetAvailable)); //this doesn't work...

                await BackgroundExecutionManager.RequestAccessAsync();

                BackgroundTaskRegistration task = builder.Register();

            }
        }

        private bool IsTaskRegistered(string taskName)
        {
            foreach (var task in BackgroundTaskRegistration.AllTasks)
            {
                if (task.Value.Name == taskName)
                {
                    return true;
                }
            }

            return false;
        }
        
        private async void Button_Tapped_4(object sender, TappedRoutedEventArgs e)
        {
            await App.MainViewModel.GetData();

            var huskyCard = App.MainViewModel.HfsData.resident_dining;
            var term = App.MainViewModel.TermInfo;

            var md = new MessageDialog($"${huskyCard.balance:0.00} Dining Balance. \n{term.FullDaysRemaining} days remaining. ${huskyCard.balance/term.FullDaysRemaining:0.00} average spend");
            await md.ShowAsync();
        }

        private void Button_Tapped_3(object sender, TappedRoutedEventArgs e)
        {
            var appdata = ApplicationData.Current.LocalSettings;
            bool tiles = (bool)(appdata.Values["TileUpdates"] ?? false);
            new MessageDialog($"tiles {tiles}").ShowAsync() ;
        }

        private void ToggleSwitch_Toggled(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var toggle = sender as ToggleSwitch;
            var appdata = ApplicationData.Current.LocalSettings.Values;
            appdata["UseToasts"] =  toggle.IsOn;
        }

        private void ToggleSwitch_Toggled_1(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var toggle = sender as ToggleSwitch;
            var appdata = ApplicationData.Current.LocalSettings.Values;
            appdata["TileUpdates"] = toggle.IsOn;
        }

        private void StartApp_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof (Views.MainPage));
        }
    }
}
