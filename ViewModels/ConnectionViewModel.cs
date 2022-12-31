using P2P_Chat_App.Models;
using P2P_Chat_App.ViewModels.Commands;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace P2P_Chat_App.ViewModels
{
    public class ConnectionViewModel : INotifyPropertyChanged
    {
        private MainViewModel _parent;
        private User _user;
        private User _friend; 
        public User User
        {
            get { return _user; }
            set
            {
                _user = value;
                OnPropertyChanged();
            }
        }
        public User Friend
        {
            get { return _friend; }
            set 
            { 
                _friend = value;
                OnPropertyChanged();
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private ICommand _listenCommand;
        public ICommand ListenCommand
        {
            get { return _listenCommand; }
            set { _listenCommand = value; }
        }
        private ICommand _connectCommand;
        public ICommand ConnectCommand
        {
            get { return _connectCommand; }
            set { _connectCommand = value; }
        }

        public ConnectionViewModel(MainViewModel viewModel)
        {
            _parent = viewModel;
            User = new User(_parent.User.Name, _parent.User.IP, _parent.User.Port);
            Friend = new User(_parent.Friend.Name, _parent.Friend.IP, _parent.Friend.Port);
            ListenCommand = new ReListenCommand(this);
            ConnectCommand = new ConnectCommand(this);
        }

        public void Listen()
        {
            if (_parent.User.Port != User.Port)
            {
                if (ReListenBox()) // if not accepted connection
                {
                    _parent.User = User;
                    // Perform Listen action
                    _parent.listen();
                }
                // Get the current window
                Window window = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
                if (window != null)
                {
                    // Close the window
                    window.Close();
                }
            }
            else
            {
                _parent.ErrorMessage = "Lstening in the same port.";
            }
        }

        public void Connect()
        {
            _parent.Friend = Friend;
            Thread.Sleep(1000);
            // Perform Connect action
            _parent.connect();
            // Get the current window
            Window window = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
            if (window != null)
            {
                // Close the window
                window.Close();
            }
        }
        private bool ReListenBox()
        {
            if (MessageBox.Show("Do you want to re Listen?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
