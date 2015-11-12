using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NotificationsExtensions.Toasts;
using Windows.ApplicationModel.Background;
using Windows.UI.Notifications;
using Windows.Security.Credentials;
using Windows.Storage;
using Windows.ApplicationModel.Resources;
using MUC;

namespace BackgroundUpdater
{
    public sealed class BalanceCheck : IBackgroundTask
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            BackgroundTaskDeferral _defferal = taskInstance.GetDeferral();

            PasswordVault vault = new PasswordVault();
            var cred = vault.FindAllByResource("MYUW").First(); // Make sure this matches CredResName

            var connector = new MUConnector(cred);
            if (await connector.Login())
            {
                var appdata = ApplicationData.Current.LocalSettings.Containers.
            }

            var toastContent = new ToastContent()
            {
                Visual = new ToastVisual()
                {
                    TitleText = new ToastText()
                    {
                        Text = "Background Task!"
                    },
                    BodyTextLine1 = new ToastText()
                    {
                        Text = "Background task ran successfully"
                    },
                    BodyTextLine2 = new ToastText()
                    {
                        Text = $"Toast at {DateTime.Now.ToLocalTime().ToString()}"
                    }
                }
            };

            var note = ToastNotificationManager.CreateToastNotifier();
            var toast = new ToastNotification(toastContent.GetXml());
            note.Show(toast);

            _defferal.Complete();
        }
    }
}
