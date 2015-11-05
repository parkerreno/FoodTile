using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using NotificationsExtensions.Tiles;
using NotificationsExtensions.Toasts;
using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;
using Windows.ApplicationModel.Background;
using MUC;
using Windows.Security.Credentials;
using Windows.UI.Popups;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace FoodTile
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private MUConnector connector;
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void Button_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var bindingContent = new TileBindingContentAdaptive();
            var items = bindingContent.Children;

            items.Add(new TileText()
            {
                Text = "Balance: $568.12",
                Style = TileTextStyle.Subtitle
            });

            items.Add(new TileText()
            {
                Text = "Days: 47",
                Style = TileTextStyle.Caption
            });

            items.Add(new TileText()
            {
                Text = "Avg/Day: $12.42",
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

        private async void Button_Tapped_3(object sender, TappedRoutedEventArgs e)
        {
            if (connector != null)
            {
                connector.Dispose();
                connector = null;
            }
            PasswordCredential c = new PasswordCredential("myuw", UserName.Text, Password.Password);
            connector = new MUConnector(c);
            bool success = await connector.Login();

            await new MessageDialog($"Login success: {success}", "Login Status").ShowAsync();
        }

        private async void Button_Tapped_4(object sender, TappedRoutedEventArgs e)
        {
            var data = await connector.GetHfsData();
            var termInfo = await connector.GetTermInfo();

            var lastDay = DateTime.Parse(termInfo.term.last_final_exam_date).Date;
            var daysRemaining = lastDay - DateTime.Now.Date;

            double average = data.resident_dining.balance / daysRemaining.TotalDays;

            var md = new MessageDialog($"${data.resident_dining.balance:0.00}Dining Balance.  {daysRemaining.TotalDays} days remaining.  ${average:0.00} average spend");
            await md.ShowAsync();
        }
    }
}
