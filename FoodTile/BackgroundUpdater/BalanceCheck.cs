using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NotificationsExtensions.Toasts;
using Windows.ApplicationModel.Background;
using Windows.UI.Notifications;

namespace BackgroundUpdater
{
    public sealed class BalanceCheck : IBackgroundTask
    {
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            BackgroundTaskDeferral _defferal = taskInstance.GetDeferral();

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
