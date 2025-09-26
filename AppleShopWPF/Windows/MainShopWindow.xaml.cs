using System;
using AppleShopWPF.Pages;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AppleShopWPF.Windows
{
    /// <summary>
    /// Логика взаимодействия для MainShopWindow.xaml
    /// </summary>
    public partial class MainShopWindow : Window
    {
        public MainShopWindow()
        {
            InitializeComponent();
            MainWindFrame.Content = new AuthorizationPage();
        }
    }
}
