using MUC;
using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Resources;
using Windows.Security.Credentials;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using System.Linq;

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

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            PasswordVault vault = new PasswordVault();
            try
            {
                var creds = vault.FindAllByResource(App.RL.GetString("CredResName"));
                if (creds.Count > 0)
                {
                    SetLoading();
                    var connector = new MUConnector(creds.First());
                    if (await connector.Login())
                    {
                        App.MainViewModel.connector = connector;
                        Frame.Navigate(typeof(MainPage));
                    }
                    else
                    {
                        vault.Remove(creds.First());
                        await new MessageDialog(App.RL.GetString("SavedLoginFailMsg"), App.RL.GetString("SavedLoginFailTitle")).ShowAsync();
                    }
                    UnsetLoading();
                }
            }
            catch
            {
                //Used to handle the case when no credentials are found - safe to move on
            }

            base.OnNavigatedTo(e);
        }

        private async void SignIn_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(usernameBox.Text) || string.IsNullOrWhiteSpace(passwordBox.Password))
            {
                EmptyField();
                return;
            }

            SetLoading();

            var credential = new PasswordCredential(App.RL.GetString("CredResName"), usernameBox.Text, passwordBox.Password);
            var connector = new MUConnector(credential);

            if (await connector.Login())
            {
                SaveAndLogin(credential, connector);
            }
            else
            {
                await new MessageDialog(App.RL.GetString("LoginFailedMsg"), App.RL.GetString("LoginFailedTitle")).ShowAsync();
            }

            UnsetLoading();
        }

        private async void SingleSignIn_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(usernameBox.Text) || string.IsNullOrWhiteSpace(passwordBox.Password))
            {
                EmptyField();
                return;
            }

            var credential = new PasswordCredential(App.RL.GetString("CredResName"), usernameBox.Text, passwordBox.Password);
            var connector = new MUConnector(credential);

            if (await connector.Login())
            {
                App.MainViewModel.connector = connector;
                ResourceLoader rl = App.RL;
                var dialog = new MessageDialog(rl.GetString("SingleUseWarning"), "Are You Sure?");
                dialog.Commands.Add(new UICommand("I'm Sure"));
                dialog.Commands.Add(new UICommand("Save Login"));
                var result = await dialog.ShowAsync();

                if (result.Label == "I'm Sure")
                {
                    Frame.Navigate(typeof(MainPage));
                }
                else
                {
                    SaveAndLogin(credential, connector);
                }
            }
        }

        private async void EmptyField()
        {
            await new MessageDialog("Your username/password cannot be empty", "Login Failed").ShowAsync();
        }

        private void SetLoading()
        {
            LoginPanel.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            LoadingPanel.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        private void UnsetLoading()
        {
            LoadingPanel.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            LoginPanel.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        private void SaveAndLogin(PasswordCredential cred, MUConnector connector)
        {
            App.MainViewModel.connector = connector;
            PasswordVault vault = new PasswordVault();
            vault.Add(cred);
            Frame.Navigate(typeof(MainPage));
        }
    }
}
