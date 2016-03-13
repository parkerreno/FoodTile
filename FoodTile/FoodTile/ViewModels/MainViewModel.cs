using MUC;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace FoodTile.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged, IDisposable
    {
        public MUConnector connector;
        public void Dispose()
        {
            if (connector != null)
                connector.Dispose();
        }

        private UserHfsData _hfsData;
        public UserHfsData HfsData
        {
            get
            {
                return _hfsData;
            }
            set
            {
                NotifyPropertyChanged(nameof(HfsData)); // lazy value changes because they should work
                _hfsData = value;
            }
        }

        private TermInfo _termInfo;
        public TermInfo TermInfo
        {
            get { return _termInfo; }
            set
            {
                NotifyPropertyChanged(nameof(TermInfo));
                _termInfo = value;
            }
        }

        public double AverageSpend
        {
            get
            {
                return HfsData.resident_dining.balance / TermInfo.FullDaysRemaining;
            }
        }

        public async Task<bool> GetData()
        {
            if (connector == null)
            {
                return false;
            }

            try
            {
                HfsData = await connector.GetHfsData();
                TermInfo = await connector.GetTermInfo();
                return true;
            }
            catch
            {
                return false;
            }
            
        }

        /// <summary>
        /// Are credentials saved in the vault?
        /// </summary>
        public bool SignOnSaved { get; set; } = false;

        #region boilerplate inpc code
        /// <summary>
        /// Raised when a property notifies that it has changed in this class
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName">Property name - please use nameof() for sanity</param>
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
