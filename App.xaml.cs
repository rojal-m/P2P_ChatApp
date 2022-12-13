using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using P2P_Chat_App.Models;
using P2P_Chat_App.ViewModels;

namespace P2P_Chat_App
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Main(Object Sender, StartupEventArgs e)
        {
            MainWindow mainWindow = new MainWindow(new MainViewModel(new ConnectionHandler()));
            mainWindow.Title = "P2P Chat App";
            mainWindow.Show();
        }
    }
}
