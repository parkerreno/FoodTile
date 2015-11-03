using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace BackgroundUpdater
{
    public sealed class BalanceCheck : IBackgroundTask
    {
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            BackgroundTaskDeferral _defferal = taskInstance.GetDeferral();

            _defferal.Complete();
        }
    }
}
