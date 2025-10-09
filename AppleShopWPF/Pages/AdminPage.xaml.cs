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
        private readonly Dictionary<uint, (string? email, string? phone, string? address)> _originalUserValues = new();

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

            ShowOnly("UsersCards");
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
                var items = this.FindName("UsersItems") as ItemsControl;
                if (items != null)
                    items.ItemsSource = users;
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
            var names = new[] { "UsersCards", "ProductsCards", "OrdersCards", "CategoriesCards" };
            foreach (var name in names)
            {
                var el = this.FindName(name) as UIElement;
                if (el != null)
                    el.Visibility = (name == elementName) ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void SetTableTitle(string sectionName)
        {
            var titleBlock = this.FindName("TableTitle") as TextBlock;
            if (titleBlock != null)
                titleBlock.Text = sectionName;
        }

        private async void UsersButton_Click(object sender, RoutedEventArgs e)
        {
            ShowOnly("UsersCards");
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
            ShowOnly("ProductsCards");
            SetTableTitle("Продукты");
            try
            {
                var products = await _apiClient.GetProductsAsync();
                var items = this.FindName("ProductsItems") as ItemsControl;
                if (items != null)
                    items.ItemsSource = products;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки продуктов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var msgExit = MessageBox.Show("Вы точно хотите выйти?", "Проверка", MessageBoxButton.YesNo);
            if (msgExit == MessageBoxResult.Yes)
            {
                AppleShopWPF.Services.AppState.CurrentUserId = 0;
                AppleShopWPF.Services.AppState.CurrentUserRoleId = 0;

                var mainWindow = Window.GetWindow(this) as Windows.MainShopWindow;
                if (mainWindow != null)
                {
                    mainWindow.Title = "Авторизация";
                    var nav = mainWindow.MainWindFrame.NavigationService;
                    if (nav != null)
                    {
                        while (nav.RemoveBackEntry() != null) { }
                    }
                    mainWindow.MainWindFrame.Content = new AuthorizationPage();
                }
                else
                {
                    var frame = this.Parent as Frame;
                    if (frame != null)
                    {
                        var owner = Window.GetWindow(this);
                        if (owner != null) owner.Title = "Авторизация";
                        var nav2 = frame.NavigationService;
                        if (nav2 != null)
                        {
                            while (nav2.RemoveBackEntry() != null) { }
                        }
                        frame.Content = new AuthorizationPage();
                    }
                    else
                    {
                        var newMain = new Windows.MainShopWindow();
                        newMain.Title = "Авторизация";
                        newMain.Show();
                        Window.GetWindow(this)?.Close();
                    }
                }
            }
            else
            {
                return;
            }
            
        }

        private async void OrdersButton_Click(object sender, RoutedEventArgs e)
        {
            ShowOnly("OrdersCards");
            SetTableTitle("Заказы");
            try
            {
                var orders = await _apiClient.GetOrdersAsync();
                var items = this.FindName("OrdersItems") as ItemsControl;
                if (items != null)
                    items.ItemsSource = orders;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки заказов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void CategoriesButton_Click(object sender, RoutedEventArgs e)
        {
            ShowOnly("CategoriesCards");
            SetTableTitle("Категории");
            try
            {
                var categories = await _apiClient.GetCategoriesAsync();
                var items = this.FindName("CategoriesItems") as ItemsControl;
                if (items != null)
                    items.ItemsSource = categories;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки категорий: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditUser_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is ApplShopAPI.Model.User user)
            {
                var window = new AppleShopWPF.Windows.AdminEditUserWindow(user);

                window.Owner = Window.GetWindow(this);
                var result = window.ShowDialog();
                if (result == true && window.UpdatedUser != null)
                {
                    // Обновляем только статус IsActive в карточке
                    user.IsActive = window.UpdatedUser.IsActive;
                    // Обновим визуализацию текста статуса
                    var cards = this.FindName("UsersCards") as ScrollViewer;
                    if (cards != null && cards.Content is ItemsControl items)
                    {
                        items.Items.Refresh();
                    }
                }
            }
        }

        private static T? FindChild<T>(DependencyObject parent, string childName) where T : FrameworkElement
        {
            if (parent == null) return null;
            int count = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T fe && fe.Name == childName)
                    return fe;
                var result = FindChild<T>(child, childName);
                if (result != null)
                    return result;
            }
            return null;
        }
    }
}
