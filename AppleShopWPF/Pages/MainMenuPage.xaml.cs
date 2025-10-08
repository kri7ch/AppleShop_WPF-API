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
                    ProductName = product.Name,
                    StockQuantity = (int)product.StockQuantity,
                    Price = product.Price,
                    ImageCode = product.ImageCode ?? "",
                    ProductId = (int)product.Id
                };

                productCard.InitializeProduct();

                productCard.OnBuyClicked += (productId) =>
                {
                    MessageBox.Show($"Товар '{product.Name}' добавлен в корзину!",
                        "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                };

                ProductsUniformGrid.Children.Add(productCard);
            }
        }

        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ProfilePage());
        }
    }
}