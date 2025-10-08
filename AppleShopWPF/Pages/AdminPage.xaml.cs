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
    /// Логика взаимодействия для AdminPage.xaml
    /// </summary>
    public partial class AdminPage : Page
    {
        private readonly AppleShopWPF.Services.ApiClient _apiClient = new AppleShopWPF.Services.ApiClient();

        public AdminPage()
        {
            InitializeComponent();
            Loaded += AdminPage_Loaded;
        }

        private async void AdminPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!AppleShopWPF.Services.AppState.IsLoggedIn)
            {
                MessageBox.Show("Необходимо войти в систему", "Авторизация", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationService?.Navigate(new AuthorizationPage());
                return;
            }

            if (!AppleShopWPF.Services.AppState.IsAdmin)
            {
                MessageBox.Show("Доступ в админ-панель запрещён", "Доступ", MessageBoxButton.OK, MessageBoxImage.Warning);
                NavigationService?.Navigate(new MainMenuPage());
                return;
            }

            try
            {
                var products = await _apiClient.GetProductsAsync();
                ProductsList.ItemsSource = products;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки продуктов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            ShowOnly("UsersGrid");
            try
            {
                await LoadUsersAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки пользователей: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task LoadUsersAsync()
        {
            try
            {
                var users = await _apiClient.GetUsersAsync();
                var grid = this.FindName("UsersGrid") as DataGrid;
                if (grid != null)
                    grid.ItemsSource = users;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки пользователей: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task UpdateIsActiveFromCheckBox(CheckBox? checkBox)
        {
            if (checkBox == null) return;

            var user = checkBox.DataContext as ApplShopAPI.Model.User;
            if (user == null) return;

            var newValue = user.IsActive == true;

            try
            {
                var ok = await _apiClient.UpdateUserIsActiveAsync(user.Id, newValue);
                if (!ok)
                {
                    user.IsActive = !newValue;
                    var grid = this.FindName("UsersGrid") as DataGrid;
                    grid?.Items.Refresh();
                    MessageBox.Show("Не удалось обновить статус активности.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                user.IsActive = !newValue;
                var grid = this.FindName("UsersGrid") as DataGrid;
                grid?.Items.Refresh();
                MessageBox.Show($"Ошибка обновления активности: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ShowOnly(string elementName)
        {
            var names = new[] { "UsersGrid", "ProductsList", "OrdersGrid", "CategoriesGrid" };
            foreach (var name in names)
            {
                var el = this.FindName(name) as UIElement;
                if (el != null)
                    el.Visibility = (name == elementName) ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void SetTableTitle(string tableName)
        {
            var titleBlock = this.FindName("TableTitle") as TextBlock;
            if (titleBlock != null)
                titleBlock.Text = $"Таблица: {tableName}";
        }

        private async void UsersButton_Click(object sender, RoutedEventArgs e)
        {
            ShowOnly("UsersGrid");
            SetTableTitle("Пользователи");
            try
            {
                await LoadUsersAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки пользователей: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void ProductsButton_Click(object sender, RoutedEventArgs e)
        {
            ShowOnly("ProductsList");
            SetTableTitle("Продукты");
            try
            {
                var products = await _apiClient.GetProductsAsync();
                ProductsList.ItemsSource = products;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки продуктов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void OrdersButton_Click(object sender, RoutedEventArgs e)
        {
            ShowOnly("OrdersGrid");
            SetTableTitle("Заказы");
            try
            {
                var orders = await _apiClient.GetOrdersAsync();
                var grid = this.FindName("OrdersGrid") as DataGrid;
                if (grid != null)
                    grid.ItemsSource = orders;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки заказов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void CategoriesButton_Click(object sender, RoutedEventArgs e)
        {
            ShowOnly("CategoriesGrid");
            SetTableTitle("Категории");
            try
            {
                var categories = await _apiClient.GetCategoriesAsync();
                var grid = this.FindName("CategoriesGrid") as DataGrid;
                if (grid != null)
                    grid.ItemsSource = categories;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки категорий: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void IsActiveCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            await UpdateIsActiveFromCheckBox(sender as CheckBox);
        }

        private async void IsActiveCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            await UpdateIsActiveFromCheckBox(sender as CheckBox);
        }
    }
}
