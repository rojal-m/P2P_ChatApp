using P2P_Chat_App.ViewModels;
using System;
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

namespace P2P_Chat_App
{
    /// <summary>
    /// Interaction logic for connection.xaml
    /// </summary>
    public partial class ConnectionWindow : Window
    {
        public ConnectionWindow(ConnectionViewModel popupVM)
        {
            InitializeComponent();
            this.DataContext= popupVM;
        }
    }
}
