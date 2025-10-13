using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using AppleShopWPF.Services;
using AppleShopWPF.Components;
using ApplShopAPI.Model;

namespace AppleShopWPF.Pages
{
    public partial class CategoriesPage : Page
    {
        private readonly ApiClient _apiClient = new ApiClient();

        public CategoriesPage()
        {
            InitializeComponent();
            Loaded += CategoriesPage_Loaded;
        }

        private async void CategoriesPage_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var categories = await _apiClient.GetCategoriesAsync();
                CategoriesItems.ItemsSource = categories;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки категорий: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task LoadProductsByCategoryAsync(uint categoryId)
        {
            HintSelectCategory.Visibility = Visibility.Collapsed;
            ProductsUniformGrid.Children.Clear();
            ProductsTitle.Text = "Products";

            try
            {
                var products = await _apiClient.GetProductsAsync();
                var filtered = products.Where(p => p.CategoryId == categoryId).ToList();
                foreach (var product in filtered)
                {
                    var card = new ProductCard
                    {
                        ProductName = product.Name,
                        StockQuantity = (int)product.StockQuantity,
                        Price = product.Price,
                        ImageCode = product.ImageCode ?? string.Empty,
                        ProductId = (int)product.Id
                    };
                    card.Margin = new Thickness(12);
                    card.InitializeProduct();
                    card.OnBuyClicked += async (productId) =>
                    {
                        if (AppState.CurrentUserId == 0)
                        {
                            MessageBox.Show("Сначала авторизуйтесь!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                        var added = await _apiClient.AddToCartAsync((uint)AppState.CurrentUserId, (uint)productId, 1);
                        if (added != null)
                        {
                            MessageBox.Show($"Товар {product.Name} добавлен в корзину!", "Добавление в корзину", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show("Не удалось добавить товар в корзину", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    };
                    ProductsUniformGrid.Children.Add(card);
                }

                if (filtered.Count == 0)
                {
                    ProductsTitle.Text = "No products in this category";
                    HintSelectCategory.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки товаров: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void Category_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is uint id)
            {
                await LoadProductsByCategoryAsync(id);
            }
            else if (sender is Button btn2 && btn2.Tag is object tag)
            {
                if (uint.TryParse(tag.ToString(), out var parsed))
                {
                    await LoadProductsByCategoryAsync(parsed);
                }
            }
        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new MainMenuPage());
        }

        private void BasketButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new CartPage());
        }

        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new ProfilePage());
        }
    }
}