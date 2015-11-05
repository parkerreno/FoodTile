using MUC;
using System;
using System.ComponentModel;

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
