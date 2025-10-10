using AppleShopWPF.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AppleShopWPF.Pages
{
    /// <summary>
    /// Логика взаимодействия для RegistationPage.xaml
    /// </summary>
    public partial class RegistationPage : Page
    {
        private readonly ApiClient _apiClient;
        public RegistationPage()
        {
            InitializeComponent();
            _apiClient = new ApiClient();
        }
        public void Entry_window()
        {
            var window = Window.GetWindow(this);
            window.Title = "Authorization";
            NavigationService.Navigate(new AuthorizationPage());
        }

        private void hyperlinkAuth(object sender, RoutedEventArgs e)
        {
            Entry_window();
        }

        private async void btnRegistClick(object sender, RoutedEventArgs e)
        {
            string email = btnEmailEnter.Text.Trim();
            string password = (cbShowPassword1.IsChecked == true) ? tbPasswordVisible1.Text : btnPasswordEnter.Password;
            string confirmPassword = (cbShowPassword2.IsChecked == true) ? tbPasswordVisible2.Text : btnPasswordVerify.Password;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Заполните все поля", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Пароли не совпадают", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (password.Length < 6)
            {
                MessageBox.Show("Пароль должен содержать минимум 6 символов", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Cursor = Cursors.Wait;
            var button = sender as Button;
            button.IsEnabled = false;

            try
            {
                bool success = await _apiClient.RegisterAsync(email, password);

                if (success)
                {
                    btnEmailEnter.Text = "";
                    btnPasswordEnter.Password = "";
                    tbPasswordVisible1.Text = "";
                    btnPasswordVerify.Password = "";
                    tbPasswordVisible2.Text = "";
                    Entry_window();
                }
            }
            finally
            {
                Cursor = Cursors.Arrow;
                button.IsEnabled = true;
            }
        }

        private void cbShowPassword1_Checked(object sender, RoutedEventArgs e)
        {
            tbPasswordVisible1.Text = btnPasswordEnter.Password;
            tbPasswordVisible1.Visibility = Visibility.Visible;
            btnPasswordEnter.Visibility = Visibility.Collapsed;
        }

        private void cbShowPassword1_Unchecked(object sender, RoutedEventArgs e)
        {
            btnPasswordEnter.Password = tbPasswordVisible1.Text;
            btnPasswordEnter.Visibility = Visibility.Visible;
            tbPasswordVisible1.Visibility = Visibility.Collapsed;
        }

        private void cbShowPassword2_Checked(object sender, RoutedEventArgs e)
        {
            tbPasswordVisible2.Text = btnPasswordVerify.Password;
            tbPasswordVisible2.Visibility = Visibility.Visible;
            btnPasswordVerify.Visibility = Visibility.Collapsed;
        }

        private void cbShowPassword2_Unchecked(object sender, RoutedEventArgs e)
        {
            btnPasswordVerify.Password = tbPasswordVisible2.Text;
            btnPasswordVerify.Visibility = Visibility.Visible;
            tbPasswordVisible2.Visibility = Visibility.Collapsed;
        }
    }
}
