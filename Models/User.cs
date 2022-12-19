using P2P_Chat_App.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace P2P_Chat_App.Models
{
    public class User : INotifyPropertyChanged
    {
        private String _name;
        private String _ip;
        private String _port;

        public User(string name, string ip, string port)
        {
            Name= name;
            IP= ip;
            Port= port;
        }

        public String Name
        {
            get { return _name; }
            set 
            { 
                _name = value;
                OnPropertyChanged();
            }
        }
        public String IP
        {
            get { return _ip; }
            set 
            { 
                _ip = value;
                OnPropertyChanged();
            }
        }
        public String Port
        {
            get { return _port; }
            set 
            { 
                _port = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            Trace.WriteLine(propertyName);
            if (propertyName == nameof(Port))
            {
                Application app = Application.Current;
                if (app != null)
                {
                    Window mainWindow = app.MainWindow;
                    if (mainWindow != null)
                    {
                        MainViewModel vm = (MainViewModel)App.Current.MainWindow.DataContext;
                        vm.listen();
                    }
                }
            }
        }
    }
}
