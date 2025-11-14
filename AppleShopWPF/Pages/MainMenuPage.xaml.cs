using AppleShopWPF.Components;
using ApplShopAPI.Model;
using AppleShopWPF.Services;
using AppleShopWPF.Pages;
using System.Windows;
using System.Windows.Controls;

namespace AppleShopWPF.Pages
{
    public partial class MainMenuPage : Page
    {
        private readonly ApiClient _apiClient;

        public MainMenuPage()
        {
            InitializeComponent();
            _apiClient = new ApiClient();
            LoadProducts();
        }

        private async void LoadProducts()
        {
            var products = await _apiClient.GetProductsAsync();
            DisplayProducts(products);
        }

        private void DisplayProducts(List<Product> products)
        {
            ProductsUniformGrid.Children.Clear();

            foreach (var product in products)
            {
                var productCard = new ProductCard
                {
                    ProductData = product,
                    ProductName = product.Name,
                    StockQuantity = (int)product.StockQuantity,
                    Price = product.Price,
                    ImageCode = product.ImageCode ?? "",
                    ProductId = (int)product.Id
                };

                productCard.InitializeProduct();

                productCard.OnBuyClicked += async (productId) =>
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

                ProductsUniformGrid.Children.Add(productCard);
            }
        }

        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ProfilePage());
        }

        private void BasketButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new CartPage());
        }

        private void CategoriesButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new CategoriesPage());
        }
    }
}