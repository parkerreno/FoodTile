using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
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
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            UpdateData();
            base.OnNavigatedTo(e);
        }

        private void Adjust_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof (Adjust));
        }

        private async void UpdateData()
        {
            if (App.MainViewModel.HfsData!=null||await App.MainViewModel.GetData())
            {
                TotalBlock.Text = $"{App.MainViewModel.HfsData.resident_dining.balance:C}";
                AvgSpendBlock.Text = $"{App.MainViewModel.AverageSpend:C}";
                DaysBlock.Text = $"{App.MainViewModel.TermInfo.FullDaysRemaining} days";
            }
            else
            {
                await new MessageDialog(
                    "We couldn't get update information...  Please make sure you have internet and have not changed your password lately.",
                    "Error Getting Data").ShowAsync();
            }
        }
    }
}
