using System.Windows;
using ApplShopAPI.Model;
using AppleShopWPF.Services;

namespace AppleShopWPF.Windows
{
    public partial class AdminEditCategoryWindow : Window
    {
        private readonly ApiClient _apiClient = new ApiClient();
        private readonly Category? _category;

        public Category? UpdatedCategory { get; private set; }

        public AdminEditCategoryWindow() : this(null) { }

        public AdminEditCategoryWindow(Category? category)
        {
            InitializeComponent();
            _category = category;
            Loaded += AdminEditCategoryWindow_Loaded;
        }

        private void AdminEditCategoryWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (_category != null)
            {
                tbName.Text = _category.Name;
                tbDescription.Text = _category.Description ?? string.Empty;
                Title = "Editing a category";
            }
            else
            {
                Title = "Adding a category";
            }
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            var name = tbName.Text?.Trim();
            var description = string.IsNullOrWhiteSpace(tbDescription.Text) ? null : tbDescription.Text.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Введите название категории", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_category == null)
            {
                var created = await _apiClient.CreateCategoryAsync(name, description);
                if (created != null)
                {
                    UpdatedCategory = created;
                    DialogResult = true;
                }
            }
            else
            {
                var updated = await _apiClient.UpdateCategoryAsync(_category.Id, name, description);
                if (updated != null)
                {
                    UpdatedCategory = updated;
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