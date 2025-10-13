using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using ApplShopAPI.Model;
using AppleShopWPF.Services;

namespace AppleShopWPF.Windows
{
    public partial class OrdersWindow : Window
    {
        private readonly ApiClient _apiClient = new ApiClient();

        public class OrdersByDate
        {
            public string DateHeader { get; set; } = string.Empty;
            public List<Order> Orders { get; set; } = new();
        }

        public OrdersWindow()
        {
            InitializeComponent();
            Loaded += OrdersWindow_Loaded;
        }

        private async void OrdersWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadAndBindAsync();
        }

        private async Task LoadAndBindAsync()
        {
            try
            {
                var userId = (uint)AppState.CurrentUserId;
                if (userId == 0)
                {
                    MessageBox.Show("Необходимо авторизоваться", "Мои заказы", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var orders = await _apiClient.GetUserOrdersAsync(userId);

                var groups = orders
                    .OrderByDescending(o => o.OrderDate)
                    .GroupBy(o => o.OrderDate.Date)
                    .Select(g => new OrdersByDate
                    {
                        DateHeader = g.Key.ToString("yyyy-MM-dd"),
                        Orders = g.ToList()
                    })
                    .ToList();

                OrdersByDateList.ItemsSource = groups;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки заказов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Close_Click(object? sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}