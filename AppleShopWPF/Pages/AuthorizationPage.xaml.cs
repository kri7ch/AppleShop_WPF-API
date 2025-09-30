using AppleShopWPF.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AppleShopWPF.Pages
{
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

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Заполните все поля", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }


            bool success = await _apiClient.LoginAsync(email, password);

            if (success)
            {
                // Получаем ID пользователя через API (нужно добавить метод в ApiClient)
                int userId = await _apiClient.GetUserIdByEmailAsync(email);

                // Сохраняем ID в статической переменной
                AppState.CurrentUserId = userId;

                // Переход на главную страницу
                NavigationService.Navigate(new MainMenuPage());
            }
            
        }

        private void Entry_window()
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