using System.Windows;
using ApplShopAPI.Model;
using AppleShopWPF.Services;

namespace AppleShopWPF.Windows
{
    public partial class AdminEditUserWindow : Window
    {
        private readonly ApiClient _apiClient = new ApiClient();
        public User UpdatedUser { get; private set; }

        public AdminEditUserWindow(User user)
        {
            InitializeComponent();
            DataContext = new User
            {
                Id = user.Id,
                Email = user.Email,
                Phone = user.Phone,
                DeliveryAddress = user.DeliveryAddress,
                IsActive = user.IsActive,
                Role = user.Role
            };
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is User dc)
            {
                bool desiredActive = (bool)(cbBlocked.IsChecked ?? false);
                var success = await _apiClient.UpdateUserIsActiveAsync(dc.Id, desiredActive);
                if (success)
                {
                    dc.IsActive = desiredActive;
                    UpdatedUser = dc;
                    DialogResult = true;
                }
                else
                {
                    MessageBox.Show("Не удалось обновить статус пользователя", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}