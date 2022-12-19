using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using P2P_Chat_App.ViewModels;


namespace P2P_Chat_App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(MainViewModel mainViewModel)
        {
            InitializeComponent();
            this.DataContext = mainViewModel;
            this.Closing += mainViewModel.closingWindow;
        }
        private void TextBox_CheckNum(object sender, TextCompositionEventArgs e)
        {
            // Check if the entered text is a numerical value
            if (!Regex.IsMatch(e.Text, @"^\d*\.?\d*$"))
            {
                // If the entered text is not a numerical value, cancel the event
                e.Handled = true;
            }
        }

    }
}
