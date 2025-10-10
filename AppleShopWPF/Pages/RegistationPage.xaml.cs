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
            string password = btnPasswordEnter.Password;
            string confirmPassword = btnPasswordVerify.Password;

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
                    btnPasswordVerify.Password = "";
                    Entry_window();
                }
            }
            finally
            {
                Cursor = Cursors.Arrow;
                button.IsEnabled = true;
            }
        }
    }
}
