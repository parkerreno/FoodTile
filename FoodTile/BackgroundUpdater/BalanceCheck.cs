using System.Linq;
using NotificationsExtensions.Toasts;
using Windows.ApplicationModel.Background;
using Windows.UI.Notifications;
using Windows.Security.Credentials;
using Windows.Storage;
using MUC;
using NotificationsExtensions.Tiles;

namespace BackgroundUpdater
{
    public sealed class BalanceCheck : IBackgroundTask
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            BackgroundTaskDeferral _defferal = taskInstance.GetDeferral();
            var appdata = ApplicationData.Current.LocalSettings;
            if (!(bool) (appdata.Values["TileUpdates"] ?? false) && !(bool) (appdata.Values["UseToasts"] ?? false)) // if task isn't needed, kill it
            {
                _defferal.Complete();
                return;
            }

            PasswordVault vault = new PasswordVault();
            PasswordCredential cred;
            try
            {
                cred = vault.FindAllByResource("MYUW").First(); // Make sure this matches CredResName
            }
            catch
            {
                _defferal.Complete();
                return;
            }

            var connector = new MUConnector(cred);
            if (await connector.Login())
            {
                
                //double lastValue = (float)(appdata.Values["LastValue"] ?? -1); //shit's stored as a float...  avoids invalid cast

                float lastValue = -1;

                if (appdata.Values.ContainsKey("LastValue"))
                {
                    float? lv = appdata.Values["LastValue"] as float?; // stupid workarounds for float precision
                    lastValue = lv.Value;
                }
                var hfs = await connector.GetHfsData();
                var term = await connector.GetTermInfo();

                double average = hfs.resident_dining.balance / term.FullDaysRemaining;

                if ((bool)(appdata.Values["TileUpdates"] ?? false))
                {
                    var bindingContent = new TileBindingContentAdaptive();
                    var medBindingContent = new TileBindingContentAdaptive();
                    var items = bindingContent.Children;
                    var medItems = medBindingContent.Children;

                    medItems.Add(new TileText()
                    {
                        Text = $"${hfs.resident_dining.balance:0.00} remaining",
                        Style = TileTextStyle.Body
                    });

                    items.Add(new TileText()
                    {
                        Text = $"${hfs.resident_dining.balance:0.00} remaining",
                        Style = TileTextStyle.Subtitle
                    });

                    var line2 = new TileText()
                    {
                        Text = $"{term.FullDaysRemaining} days",
                        Style = TileTextStyle.Caption
                    };

                    var line3 = new TileText()
                    {            
                        Text = $"${average:0.00} per day",
                        Style = TileTextStyle.Caption
                    };

                    items.Add(line2);
                    items.Add(line3);
                    medItems.Add(line2);
                    medItems.Add(line3);

                    var tileBinding = new TileBinding()
                    {
                        Content = bindingContent,
                        DisplayName = "FoodTile",
                        Branding = TileBranding.NameAndLogo
                    };

                    var medTileBinding = new TileBinding()
                    {
                        Content = medBindingContent,
                        DisplayName = "FoodTile",
                        Branding = TileBranding.NameAndLogo
                    };

                    var content = new TileContent()
                    {
                        Visual = new TileVisual
                        {
                            TileMedium = medTileBinding,
                            TileWide = tileBinding,
                            TileLarge = tileBinding
                        }
                    };

                    var note = new TileNotification(content.GetXml());
                    var updater = TileUpdateManager.CreateTileUpdaterForApplication();

                    updater.Update(note);
                }

                if (hfs.resident_dining.balance != lastValue)
                {
                    if ((bool)(appdata.Values["UseToasts"] ?? false))
                    {
                        var toastContent = new ToastContent()
                        {
                            Visual = new ToastVisual()
                            {
                                TitleText = new ToastText()
                                {
                                    Text = $"Dining Balance ${hfs.resident_dining.balance:0.00}"
                                },
                                BodyTextLine1 = new ToastText()
                                {
                                    Text = $"New daily average ${average:0.00}"
                                },
                                BodyTextLine2 = new ToastText()
                                {
                                    Text = $"{term.FullDaysRemaining} days remaining in quarter"
                                }
                            }
                        };

                        var note = ToastNotificationManager.CreateToastNotifier();
                        var toast = new ToastNotification(toastContent.GetXml());
                        note.Show(toast);
                    }
                    appdata.Values["LastValue"] = hfs.resident_dining.balance;
                }
            }
            connector.Dispose();
            _defferal.Complete();
        }
    }
}
