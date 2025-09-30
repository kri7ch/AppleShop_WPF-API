using AppleShopWPF.Services;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace AppleShopWPF.Components
{
    public partial class ProductCard : UserControl
    {
        public ProductCard()
        {
            InitializeComponent();
        }

        public string ProductName { get; set; } = "Product Name";
        public int StockQuantity { get; set; } = 0;
        public decimal Price { get; set; } = 0;
        public string ImageCode { get; set; } = "";
        public int ProductId { get; set; } = 0;

        public event Action<int> OnBuyClicked;

        public void InitializeProduct()
        {
            ProductNameText.Text = ProductName;
            ProductCountText.Text = $"In stock: {StockQuantity} pcs";
            ProductPriceText.Text = $"{Price}₽";

            LoadProductImage();

            BtnBuyProduct.IsEnabled = StockQuantity > 0;
            BtnBuyProduct.Content = StockQuantity > 0 ? "Buy" : "Out of stock";
        }

        private void LoadProductImage()
        {
            try
            {
                string imagePath = $@"C:\Users\rakhm\source\repos\AppleStore_Project\AppleStore_Project\Assets\Images\Products\{ImageCode}.png";

                if (File.Exists(imagePath))
                {
                    ProductImage.Source = new BitmapImage(new Uri(imagePath));
                }
                else
                {
                    string placeholderPath = @"C:\Users\rakhm\source\repos\AppleStore_Project\AppleStore_Project\Assets\Images\Zaglushka.png";
                    ProductImage.Source = new BitmapImage(new Uri(placeholderPath));
                }
            }
            catch
            {
                string placeholderPath = @"C:\Users\rakhm\source\repos\AppleStore_Project\AppleStore_Project\Assets\Images\Zaglushka.png";
                ProductImage.Source = new BitmapImage(new Uri(placeholderPath));
            }
        }
        private void BtnBuyProduct_Click(object sender, RoutedEventArgs e)
        {
            if (AppState.CurrentUserId == 0)
            {
                MessageBox.Show("Сначала авторизуйтесь!", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            OnBuyClicked?.Invoke(ProductId);
        }
    }
}