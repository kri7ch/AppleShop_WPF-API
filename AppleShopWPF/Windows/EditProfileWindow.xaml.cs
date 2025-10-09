using System.Windows;
using ApplShopAPI.Model;
using AppleShopWPF.Services;

namespace AppleShopWPF.Windows
{
    public partial class EditProfileWindow : Window
    {
        private readonly ApiClient _apiClient = new ApiClient();
        private readonly User _currentUser;

        public EditProfileWindow(User user)
        {
            InitializeComponent();
            _currentUser = user;
            // preload values
            EmailBox.Text = user.Email;
            PhoneBox.Text = user.Phone ?? string.Empty;
            AddressBox.Text = user.DeliveryAddress ?? string.Empty;
        }

        public User? UpdatedUser { get; private set; }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            var email = EmailBox.Text?.Trim();
            var phone = string.IsNullOrWhiteSpace(PhoneBox.Text) ? null : PhoneBox.Text.Trim();
            var address = string.IsNullOrWhiteSpace(AddressBox.Text) ? null : AddressBox.Text.Trim();

            var refreshed = await _apiClient.UpdateProfileAsync(_currentUser.Id, email, phone, address);
            if (refreshed != null)
            {
                UpdatedUser = refreshed;
                DialogResult = true;
                Close();
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}