using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Controls;
using AppleShopWPF.Services;
using AppleShopWPF.Components;
using ApplShopAPI.Model;

namespace AppleShopWPF.Pages
{
    public partial class CartPage : Page
    {
        private readonly ApiClient _apiClient = new ApiClient();
        private List<CartItem> _items = new();

        public CartPage()
        {
            InitializeComponent();
            Loaded += CartPage_Loaded;
        }

        private async void CartPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (AppState.CurrentUserId == 0)
            {
                MessageBox.Show("Сначала авторизуйтесь!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                NavigationService?.Navigate(new AuthorizationPage());
                return;
            }

            await ReloadCartAsync();
        }

        private void UpdateTotals()
        {
            var totalQty = _items.Sum(i => (int)i.Quantity);
            var totalPrice = _items.Sum(i => (decimal)i.Quantity * (i.Product?.Price ?? 0));
            TotalQuantityText.Text = totalQty.ToString();
            TotalPriceText.Text = $"{totalPrice} ₽";
        }

        private CartItemControl? GetControlForItem(CartItem item)
        {
            foreach (var obj in CartItemsControl.Items)
            {
                if (obj is CartItemControl ctrl && ReferenceEquals(ctrl.DataContext, item))
                    return ctrl;
            }
            return null;
        }

        private async Task ReloadCartAsync()
        {
            _items = await _apiClient.GetCartAsync((uint)AppState.CurrentUserId);
            CartItemsControl.Items.Clear();
            foreach (var cartItem in _items)
            {
                var card = new CartItemControl { DataContext = cartItem };
                card.IncreaseRequested += Increase_Click;
                card.DecreaseRequested += Decrease_Click;
                card.RemoveRequested += Remove_Click;
                CartItemsControl.Items.Add(card);
            }
            UpdateTotals();
        }

        private async void Increase_Click(object sender, RoutedEventArgs e)
        {
            CartItem? item = null;
            if (sender is Button btn && btn.DataContext is CartItem btnItem)
                item = btnItem;
            else if (sender is CartItemControl ctrl && ctrl.DataContext is CartItem ctrlItem)
                item = ctrlItem;

            if (item == null) return;

            var maxQty = (ushort)(item.Product?.StockQuantity ?? 0);
            if (item.Quantity >= maxQty)
            {
                MessageBox.Show("Максимальное количество доступных товаров достигнуто!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var newQty = (ushort)(item.Quantity + 1);
            var ok = await _apiClient.UpdateCartItemAsync((uint)AppState.CurrentUserId, item.ProductId, newQty);
            if (ok)
            {
                item.Quantity = newQty;
                var view = GetControlForItem(item);
                if (view != null)
                {
                    view.DataContext = null;
                    view.DataContext = item;
                }
                UpdateTotals();
            }
        }

        private async void Decrease_Click(object sender, RoutedEventArgs e)
        {
            CartItem? item = null;
            if (sender is Button btn && btn.DataContext is CartItem btnItem)
                item = btnItem;
            else if (sender is CartItemControl ctrl && ctrl.DataContext is CartItem ctrlItem)
                item = ctrlItem;

            if (item == null) return;

            var newQty = item.Quantity > 1 ? (ushort)(item.Quantity - 1) : (ushort)0;
            if (newQty == 0)
            {
                var okDel = await _apiClient.RemoveCartItemAsync((uint)AppState.CurrentUserId, item.ProductId);
                if (okDel)
                {
                    _items.Remove(item);
                    var view = GetControlForItem(item);
                    if (view != null)
                        CartItemsControl.Items.Remove(view);
                    UpdateTotals();
                }
            }
            else
            {
                var ok = await _apiClient.UpdateCartItemAsync((uint)AppState.CurrentUserId, item.ProductId, newQty);
                if (ok)
                {
                    item.Quantity = newQty;
                    var view = GetControlForItem(item);
                    if (view != null)
                    {
                        view.DataContext = null;
                        view.DataContext = item;
                    }
                    UpdateTotals();
                }
            }
        }

        private async void Remove_Click(object sender, RoutedEventArgs e)
        {
            CartItem? item = null;
            if (sender is Button btn && btn.DataContext is CartItem btnItem)
                item = btnItem;
            else if (sender is CartItemControl ctrl && ctrl.DataContext is CartItem ctrlItem)
                item = ctrlItem;

            if (item == null) return;

            var ok = await _apiClient.RemoveCartItemAsync((uint)AppState.CurrentUserId, item.ProductId);
            if (ok)
            {
                _items.Remove(item);
                var view = GetControlForItem(item);
                if (view != null)
                    CartItemsControl.Items.Remove(view);
                UpdateTotals();
            }
        }

        private async void PlaceOrder_Click(object sender, RoutedEventArgs e)
        {
            if (_items == null || _items.Count == 0)
            {
                MessageBox.Show("Корзина пуста", "Инфо", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var win = new AppleShopWPF.Windows.OrderWindow(_items)
            {
                Owner = Window.GetWindow(this),
                Topmost = true
            };
            bool? result = win.ShowDialog();
            if (result == true)
            {
                var cleared = await _apiClient.ClearCartAsync((uint)AppState.CurrentUserId);
                if (!cleared)
                {
                    MessageBox.Show("Заказ оформлен, но не удалось очистить корзину", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                await ReloadCartAsync();
                MessageBox.Show("Заказ оформлен!", "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new MainMenuPage());
        }

        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new ProfilePage());
        }

        private void CategoriesButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new CategoriesPage());
        }

        private void CartItemImage_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var img = sender as System.Windows.Controls.Image;
                if (img == null) return;

                if (img.DataContext is CartItem item)
                {
                    string code = item.Product?.ImageCode ?? string.Empty;
                    string imagePath = $@"C:\Users\rakhm\source\repos\AppleStore_Project\AppleStore_Project\Assets\Images\Products\{code}.png";
                    string placeholderPath = @"C:\Users\rakhm\source\repos\AppleStore_Project\AppleStore_Project\Assets\Images\Zaglushka.png";

                    string pathToUse = (!string.IsNullOrWhiteSpace(code) && File.Exists(imagePath)) ? imagePath : placeholderPath;
                    img.Source = new BitmapImage(new Uri(pathToUse));
                }
            }
            catch
            {
                var img = sender as System.Windows.Controls.Image;
                if (img != null)
                {
                    string placeholderPath = @"C:\Users\rakhm\source\repos\AppleStore_Project\AppleStore_Project\Assets\Images\Zaglushka.png";
                    img.Source = new BitmapImage(new Uri(placeholderPath));
                }
            }
        }
    }
}