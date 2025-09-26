using AppleShopWPF.Services;
using Microsoft.Win32;
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
    /// Логика взаимодействия для AuthorizationPage.xaml
    /// </summary>
    public partial class AuthorizationPage : Page
    {
        private readonly ApiClient _apiClient;
        public AuthorizationPage()
        {
            InitializeComponent();
            _apiClient = new ApiClient();
        }

        private async void btnEnterClick(object sender, RoutedEventArgs e)
        {
            string email = btnEmailEnter.Text.Trim();
            string password = btnPasswordEnter.Password;

            // Валидация
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Заполните все поля", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Показываем курсор ожидания
            Cursor = Cursors.Wait;
            var button = sender as Button;
            button.IsEnabled = false;

            try
            {
                bool success = await _apiClient.LoginAsync(email, password);

                if (success)
                {
                    // Очищаем поля после успешного входа
                    btnEmailEnter.Text = "";
                    btnPasswordEnter.Password = "";

                    // Здесь можно добавить переход на главную страницу
                    // NavigationService.Navigate(new MainShopWindow());
                }
            }
            finally
            {
                Cursor = Cursors.Arrow;
                button.IsEnabled = true;
            }
        }

        public void Entry_window()
        {
            var window = Window.GetWindow(this);
            window.Title = "Регистрация";
            NavigationService.Navigate(new RegistationPage());
        }

        private void hyperlinkRegistration(object sender, RoutedEventArgs e)
        {
            Entry_window();
        }
    }
}
