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
using AppleShopWPF.Services;
using ApplShopAPI.Model;

namespace AppleShopWPF.Pages
{
    /// <summary>
    /// Логика взаимодействия для ProfilePage.xaml
    /// </summary>
    public partial class ProfilePage : Page
    {
        private readonly ApiClient _apiClient = new ApiClient();

        public ProfilePage()
        {
            InitializeComponent();
            Loaded += ProfilePage_Loaded;
        }

        private async void ProfilePage_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!Services.AppState.IsLoggedIn || Services.AppState.CurrentUserId == 0)
                {
                    MessageBox.Show("Необходимо войти, чтобы просмотреть профиль", "Авторизация", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                } 

                var profile = await _apiClient.GetProfileAsync((uint)Services.AppState.CurrentUserId);
                if (profile != null)
                {
                    this.DataContext = profile;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки профиля: {ex.Message}");
            }
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Выйти из аккаунта?", "Утверждение выхода", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                NavigationService.Navigate(new AuthorizationPage());
            }
            else
            {
                return;
            }
        }

        private void btnMenuClick(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MainMenuPage());
        }

    }
}
