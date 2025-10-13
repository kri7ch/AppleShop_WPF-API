using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ApplShopAPI.Model;
using AppleShopWPF.Services;

namespace AppleShopWPF.Windows
{
    public partial class OrderWindow : Window
    {
        public class ItemSummary
        {
            public string Name { get; set; } = string.Empty;
            public int Quantity { get; set; }
            public decimal TotalPrice { get; set; }
        }

        private readonly IEnumerable<CartItem> _cartItems;
        public IReadOnlyList<ItemSummary> Summaries { get; private set; } = new List<ItemSummary>();
        public int TotalQuantity { get; private set; }
        public decimal TotalAmount { get; private set; }

        public string SelectedPaymentMethod { get; private set; } = "";

        public OrderWindow(IEnumerable<CartItem> cartItems)
        {
            InitializeComponent();
            _cartItems = cartItems;
            Aggregate();
            BindUi();
        }

        private void Aggregate()
        {
            var grouped = _cartItems.Where(ci => ci.Product != null).GroupBy(ci => ci.Product!.Id)
                .Select(g => new ItemSummary
                {
                    Name = g.First().Product!.Name,
                    Quantity = g.Sum(x => (int)x.Quantity),
                    TotalPrice = g.Sum(x => (decimal)x.Quantity * x.Product!.Price)
                }).ToList();

            Summaries = grouped;
            TotalQuantity = grouped.Sum(s => s.Quantity);
            TotalAmount = grouped.Sum(s => s.TotalPrice);
        }

        private void BindUi()
        {
            ItemsList.ItemsSource = Summaries;
            TotalItemsText.Text = TotalQuantity.ToString();
            TotalAmountText.Text = string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0:N2} ₽", TotalAmount);
        }

        private void PaymentMethod_Checked(object sender, RoutedEventArgs e)
        {
            if (RbCard.IsChecked == true)
            {
                SelectedPaymentMethod = "Card";
                CardForm.Visibility = Visibility.Visible;
            }
            else if (RbCash.IsChecked == true)
            {
                SelectedPaymentMethod = "Cash";
                CardForm.Visibility = Visibility.Collapsed;
            }
        }

        private bool ValidateCard()
        {
            if (SelectedPaymentMethod != "Card") return true;

            if (string.IsNullOrWhiteSpace(TbCardholder.Text)) 
            { 
                MessageBox.Show("Введите имя владельца карты", "Оплата", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(TbCardNumber.Text) || TbCardNumber.Text.Replace(" ", "").Length < 12) 
            {   
                MessageBox.Show("Введите корректный номер карты", "Оплата", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(TbMonth.Text) || string.IsNullOrWhiteSpace(TbYear.Text)) 
            { 
                MessageBox.Show("Введите срок действия карты", "Оплата", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (PbCvv.Password.Length < 3) 
            {
                MessageBox.Show("Введите CVV", "Оплата", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private async void Place_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(SelectedPaymentMethod))
            {
                MessageBox.Show("Выберите способ оплаты (карта или наличные)", "Оформление заказа", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!ValidateCard()) return;

            var userId = (uint)AppState.CurrentUserId;
            if (userId == 0)
            {
                MessageBox.Show("Необходимо авторизоваться", "Оформление заказа", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var deliveryAddress = string.Empty;
            try
            {
                var api = new ApiClient();
                var profile = await api.GetProfileAsync(userId);
                deliveryAddress = profile?.DeliveryAddress?.Trim() ?? string.Empty;
            }
            catch { }

            if (string.IsNullOrWhiteSpace(deliveryAddress))
            {
                MessageBox.Show("Укажите адрес доставки в профиле и повторите оформление", "Адрес доставки", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var items = _cartItems.Where(ci => ci.Product != null).Select(ci => new OrderItem
            {
                ProductId = ci.ProductId,
                Quantity = ci.Quantity,
                Price = ci.Product!.Price
            }).ToList();

            var client = new ApiClient();
            var created = await client.CreateOrderAsync(userId, deliveryAddress, items, SelectedPaymentMethod);
            if (created != null)
            {
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Не удалось оформить заказ", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}