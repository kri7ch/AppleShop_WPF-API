using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using ApplShopAPI.Model;

namespace AppleShopWPF.Components
{
    public partial class CartItemControl : UserControl
    {
        public CartItemControl()
        {
            InitializeComponent();
        }

        private void ProductImage_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var img = sender as Image;
                if (img == null) return;
                if (DataContext is CartItem item)
                {
                    string code = item.Product?.ImageCode ?? string.Empty;
                    string imagePath = $@"C:\\Users\\rakhm\\source\\repos\\AppleStore_Project\\AppleStore_Project\\Assets\\Images\\Products\\{code}.png";
                    string placeholderPath = @"C:\\Users\\rakhm\\source\\repos\\AppleStore_Project\\AppleStore_Project\\Assets\\Images\\Zaglushka.png";

                    string pathToUse = (!string.IsNullOrWhiteSpace(code) && File.Exists(imagePath)) ? imagePath : placeholderPath;
                    img.Source = new BitmapImage(new Uri(pathToUse));
                }
            }
            catch
            {
                var img = sender as Image;
                if (img != null)
                {
                    string placeholderPath = @"C:\\Users\\rakhm\\source\\repos\\AppleStore_Project\\AppleStore_Project\\Assets\\Images\\Zaglushka.png";
                    img.Source = new BitmapImage(new Uri(placeholderPath));
                }
            }
        }

        private void Increase_Click(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(IncreaseRequestedEvent, this));
        }

        private void Decrease_Click(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(DecreaseRequestedEvent, this));
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(RemoveRequestedEvent, this));
        }

        public static readonly RoutedEvent IncreaseRequestedEvent = EventManager.RegisterRoutedEvent(
            name: nameof(IncreaseRequested), routingStrategy: RoutingStrategy.Bubble, ownerType: typeof(CartItemControl), handlerType: typeof(RoutedEventHandler));
        public event RoutedEventHandler IncreaseRequested
        {
            add { AddHandler(IncreaseRequestedEvent, value); }
            remove { RemoveHandler(IncreaseRequestedEvent, value); }
        }

        public static readonly RoutedEvent DecreaseRequestedEvent = EventManager.RegisterRoutedEvent(
            name: nameof(DecreaseRequested), routingStrategy: RoutingStrategy.Bubble, ownerType: typeof(CartItemControl), handlerType: typeof(RoutedEventHandler));
        public event RoutedEventHandler DecreaseRequested
        {
            add { AddHandler(DecreaseRequestedEvent, value); }
            remove { RemoveHandler(DecreaseRequestedEvent, value); }
        }

        public static readonly RoutedEvent RemoveRequestedEvent = EventManager.RegisterRoutedEvent(
            name: nameof(RemoveRequested), routingStrategy: RoutingStrategy.Bubble, ownerType: typeof(CartItemControl), handlerType: typeof(RoutedEventHandler));
        public event RoutedEventHandler RemoveRequested
        {
            add { AddHandler(RemoveRequestedEvent, value); }
            remove { RemoveHandler(RemoveRequestedEvent, value); }
        }
    }
}