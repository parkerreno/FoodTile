using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace FoodTile
{
    public class Helpers
    {
        public static ObservableCollection<DateTime> LoadSavedDates()
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


        public static List<DateTime> LoadSavedDatesList()
        {
            return LoadSavedDates().ToList();
        }
    }
}
