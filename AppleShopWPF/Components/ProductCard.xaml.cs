using AppleShopWPF.Services;
using AppleShopWPF.Windows;
using ApplShopAPI.Model;
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

        public Product? ProductData { get; set; }
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
            ProductPriceText.Text = $"{Price} ₽";

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

        private void RootBorder_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Не открываем детали, если клик по кнопке
            if (IsClickOnButton(e))
                return;

            var owner = Window.GetWindow(this);

            // Если полный объект продукта известен, показываем его; иначе собираем из свойств
            var product = ProductData ?? new Product
            {
                Id = (uint)ProductId,
                Name = ProductName,
                Price = Price,
                StockQuantity = (uint)Math.Max(0, StockQuantity),
                ImageCode = string.IsNullOrWhiteSpace(ImageCode) ? null : ImageCode,
                CategoryId = 0,
                StatusId = 1
            };

            var details = new ProductDetailWindow(product);
            if (owner != null)
                details.Owner = owner;
            details.ShowDialog();
        }

        private static bool IsClickOnButton(RoutedEventArgs e)
        {
            if (e.OriginalSource is DependencyObject d)
            {
                while (d != null)
                {
                    if (d is Button)
                        return true;
                    d = System.Windows.Media.VisualTreeHelper.GetParent(d);
                }
            }
            return false;
        }
    }
}