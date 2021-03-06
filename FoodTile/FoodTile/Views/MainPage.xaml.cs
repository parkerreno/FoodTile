﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.System;
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

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            Frame.BackStack.Clear(); //clear the backstack to remove the back button
            if (ApplicationData.Current.LocalSettings.Values[App.RL.GetString("TileSettingString")] == null)
            {
                var md = new MessageDialog(
                    "You can enable the live tile and notifications now, or you can change these in settings at any time.",
                    "Enable Live Tile?");
                md.Commands.Clear();
                md.Commands.Add(new UICommand("Yes"));
                md.Commands.Add(new UICommand("Later"));

                var result = await md.ShowAsync();
                if (result.Label == "Yes")
                {
                    Frame.Navigate(typeof (Views.Settings));
                }
            }
            UpdateData();
            base.OnNavigatedTo(e);
        }

        private void Adjust_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof (Adjust));
        }

        private async void UpdateData(bool force = false)
        {
            // (( We have existing data     AND   Not forcing) OR Try get that data )
            if ((App.MainViewModel.HfsData!=null && !force)||await App.MainViewModel.GetData())
            {
                var hfsData = App.MainViewModel.HfsData;
                TotalBlock.Text = $"{hfsData.resident_dining.balance:C}";
                AvgSpendBlock.Text = $"{hfsData.resident_dining.balance/ App.MainViewModel.TermInfo.AdjustedDaysRemaining(Helpers.LoadSavedDatesList()):C} per day";
                DaysBlock.Text = $"{App.MainViewModel.TermInfo.AdjustedDaysRemaining(Helpers.LoadSavedDatesList())} days";

                HuskyAcct.Text = $"Husky Card: {hfsData?.student_husky_card?.balance:C}";
                EmpAcct.Text = $"Employee Acct: {hfsData?.employee_husky_card?.balance:C}";
            }
            else
            {
                await new MessageDialog(
                    "We couldn't get update information...  Please make sure you have internet and have not changed your password lately.",
                    "Error Getting Data").ShowAsync();
            }
        }

        private void Refresh_Tapped(object sender, TappedRoutedEventArgs e)
        {
            UpdateData(true);
        }

        private void Settings_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(Views.Settings));
        }

        private void About_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof (Views.Settings), 1);
        }

        private async void Review_Tapped(object sender, TappedRoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri(string.Format("ms-windows-store:REVIEW?PFN={0}", Windows.ApplicationModel.Package.Current.Id.FamilyName)));
            //http://stackoverflow.com/questions/33341354/windows-10-how-to-launch-rate-and-review-popup-view
        }
    }
}
