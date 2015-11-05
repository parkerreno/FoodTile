using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Resources;
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
    public sealed partial class Login : Page
    {
        public Login()
        {
            this.InitializeComponent();
        }

        private void SignIn_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        private async void SingleSignIn_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ResourceLoader rl = new ResourceLoader();
            var dialog = new MessageDialog(rl.GetString("SingleUseWarning"), "Are You Sure?");
            dialog.Commands.Add(new UICommand("I'm Sure"));
            dialog.Commands.Add(new UICommand("Cancel"));
            var result = await dialog.ShowAsync();

            if (result.Label == "I'm Sure")
            {
                Frame.Navigate(typeof(MainPage));
            }
        }
    }
}
