using MUC;
using System;
using Windows.ApplicationModel.Resources;
using Windows.Security.Credentials;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

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

        private async void SignIn_Tapped(object sender, TappedRoutedEventArgs e)
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
                PasswordVault vault = new PasswordVault();
                vault.Add(credential);
                await new MessageDialog("Login succeeded and added to vault").ShowAsync();
                vault.Remove(credential);
            }
            else
            {
                await new MessageDialog(App.RL.GetString("LoginFailedMsg"), App.RL.GetString("LoginFailedTitle")).ShowAsync();
            }
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
                    //TODO: Implement saving from this dialog
                }
            }            
        }

        private async void EmptyField()
        {
            await new MessageDialog("Your username/password cannot be empty", "Login Failed").ShowAsync();
        }
    }
}
