using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using ApplShopAPI.Model;
using AppleShopWPF.Services;

namespace AppleShopWPF.Windows
{
    public partial class AdminEditProductWindow : Window
    {
        private readonly ApiClient _apiClient = new ApiClient();
        private readonly Product? _product;

        public Product? UpdatedProduct { get; private set; }

        public AdminEditProductWindow() : this(null) { }

        public AdminEditProductWindow(Product? product)
        {
            InitializeComponent();
            _product = product;
            Loaded += AdminEditProductWindow_Loaded;
        }

        private async void AdminEditProductWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var categories = await _apiClient.GetCategoriesAsync();
            cbCategory.ItemsSource = categories;

            if (_product != null)
            {
                tbName.Text = _product.Name;
                tbPrice.Text = _product.Price.ToString(CultureInfo.InvariantCulture);
                tbStock.Text = _product.StockQuantity.ToString(CultureInfo.InvariantCulture);
                tbImageCode.Text = _product.ImageCode ?? string.Empty;
                cbCategory.SelectedValue = _product.CategoryId;
                Title = "Редактирование товара";
            }
            else
            {
                Title = "Добавление товара";
            }
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbName.Text))
            {
                MessageBox.Show("Введите название товара", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!decimal.TryParse(tbPrice.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out var price))
            {
                MessageBox.Show("Некорректная цена", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!uint.TryParse(tbStock.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var stock))
            {
                MessageBox.Show("Некорректный остаток", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (cbCategory.SelectedValue == null)
            {
                MessageBox.Show("Выберите категорию", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var categoryId = (uint)(cbCategory.SelectedValue is uint u ? u : Convert.ToUInt32(cbCategory.SelectedValue));
            var imageCode = string.IsNullOrWhiteSpace(tbImageCode.Text) ? null : tbImageCode.Text.Trim();

            if (_product == null)
            {
                var created = await _apiClient.CreateProductAsync(categoryId, tbName.Text.Trim(), price, stock, imageCode);
                if (created != null)
                {
                    UpdatedProduct = created;
                    DialogResult = true;
                }
            }
            else
            {
                var updated = await _apiClient.UpdateProductAsync(_product.Id, categoryId, tbName.Text.Trim(), price, stock, imageCode);
                if (updated != null)
                {
                    UpdatedProduct = updated;
                    DialogResult = true;
                }
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}