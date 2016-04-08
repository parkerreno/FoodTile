using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class Adjust : Page, INotifyPropertyChanged
    {
        private ObservableCollection<DateTime> _dates;
        public Adjust()
        {
            this.InitializeComponent();
            
            _dates = LoadSavedDates();
            _dates.CollectionChanged += _dates_CollectionChanged;
            DatesListView.ItemsSource = _dates;
        }

        private ObservableCollection<DateTime> LoadSavedDates()
        {
            var settings = ApplicationData.Current.RoamingSettings.Values;
            if (settings["AdjustedDates"] == null)
            {
                return new ObservableCollection<DateTime>();
            }
            else
            {
                var rawString = settings["AdjustedDates"] as string;
                var stringDates = rawString.Split('\n');
                ObservableCollection<DateTime> toReturn = new ObservableCollection<DateTime>();

                DateTime toAdd;
                foreach (var stringDate in stringDates)
                {
                    if (DateTime.TryParse(stringDate, out toAdd))
                        toReturn.Add(toAdd);
                }

                return toReturn;
            }
        }

        private void _dates_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            var settings = ApplicationData.Current.RoamingSettings.Values;
            var sb = new StringBuilder();
            foreach (var date in _dates)
            {
                sb.AppendLine(date.ToString());
            }
            settings["AdjustedDates"] = sb.ToString();
        }

        private void AddDate_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (DatePicker.Date.HasValue)
            {
                var date = DatePicker.Date.Value.Date;
                if (!_dates.Any(x => x.Equals(date)))
                {
                    _dates.Add(date);
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void DateList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listView = sender as ListView;
            if (listView.SelectedIndex == -1)
                return;

            _dates.RemoveAt(listView.SelectedIndex);

            listView.SelectedIndex = -1;
        }
    }
}
