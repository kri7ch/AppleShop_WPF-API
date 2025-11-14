using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using ApplShopAPI.Model;

namespace AppleShopWPF.Windows
{
    public partial class ProductDetailWindow : Window
    {
        private readonly Product _product;

        public ProductDetailWindow(Product product)
        {
            InitializeComponent();
            _product = product;
            Populate();
        }

        private void Populate()
        {
            ProductNameText.Text = _product.Name;
            ProductPriceText.Text = $"{_product.Price} ₽";
            ProductCountText.Text = _product.StockQuantity > 0 ? $"In stock: {_product.StockQuantity} pcs" : "Out of stock";

            AdditionalInfoText.Text = $"Product ID: {_product.Id}\n" + $"Category ID: {_product.CategoryId}\n" + $"Status: {_product.StatusId}\n" + $"Image: {_product.ImageCode ?? "—"}";

            LoadProductImage(_product.ImageCode);
        }

        private void LoadProductImage(string? imageCode)
        {
            try
            {
                var code = imageCode ?? string.Empty;
                string imagePath = $@"C:\\Users\\rakhm\\source\\repos\\AppleStore_Project\\AppleStore_Project\\Assets\\Images\\Products\\{code}.png";

                if (File.Exists(imagePath))
                {
                    ProductImage.Source = new BitmapImage(new Uri(imagePath));
                }
                else
                {
                    string placeholderPath = @"C:\\Users\\rakhm\\source\\repos\\AppleStore_Project\\AppleStore_Project\\Assets\\Images\\Zaglushka.png";
                    ProductImage.Source = new BitmapImage(new Uri(placeholderPath));
                }
            }
            catch
            {
                string placeholderPath = @"C:\\Users\\rakhm\\source\\repos\\AppleStore_Project\\AppleStore_Project\\Assets\\Images\\Zaglushka.png";
                ProductImage.Source = new BitmapImage(new Uri(placeholderPath));
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}