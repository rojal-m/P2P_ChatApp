using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using P2P_Chat_App.Models;
using P2P_Chat_App.ViewModels.Commands;
using P2P_Chat_App.Views;
using Prism.Commands;

namespace P2P_Chat_App.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private ConnectionHandler _connection;
        public ConnectionHandler Connection
        {
            get { return _connection; }
            set { _connection = value; }
        }

        private string _messageToSend;
        public string MessageToSend
        {
            get { return _messageToSend; }
            set 
            { 
                _messageToSend = value;
                OnPropertyChanged();
            }
        }

        private ICommand _pushCommand;
        private ICommand _disconnectCommand;
        public ICommand PushCommand
        {
            get { return _pushCommand; }
            set { _pushCommand = value; }
        }
        
        public ICommand DisconnectCommand
        {
            get { return _disconnectCommand; }
            set { _disconnectCommand = value; }
        }

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

        public ObservableCollection<ChatItem> SelectedContactMessages { get; set; }


        public ICommand OpenPopupCommand { get; set; }

        


        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            if (propertyName == nameof(User))
            {
                Connection.User = User;
            }
            if (propertyName == nameof(Friend))
            {
                Connection.Friend = Friend;
            }
        }

        public MainViewModel(ConnectionHandler connectionHandler)
        {
            Connection = connectionHandler;
            User = Connection.User;
            Friend = Connection.Friend;
            SelectedContactMessages = Connection.SelectedContactMessages;
           // SelectedContactMessages.CollectionChanged += ItemsList_CollectionChanged;
            PushCommand = new SendMessageCommand(this);
            DisconnectCommand = new DisconnectCommand(this);
            OpenPopupCommand = new OpenPopupCommand(this);
        }
        /*private void ItemsList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Collection has been changed, do something here
            Application app = Application.Current;
            if (app != null)
            {
                MainWindow mainWindow = (MainWindow)app.MainWindow;
                if (mainWindow != null)
                {
                    object value = mainWindow.CollectionChanged();
                }
            }

        }*/


        public void OpenPopup()
        {
            ConnectionWindow connectWindow = new ConnectionWindow(new ConnectionViewModel(this));
            connectWindow.Show();
        }
        public void sendMessage()
        {
            Connection.sendMessage(MessageToSend);
            MessageToSend = null;
        }
        public void connect()
        {
            Connection.Connect();
        }
        public void disconnect()
        {
            Connection.Disconnect();
        }
        public void listen()
        {
            Connection.Relisten();
        }
        public void closingWindow(object sender, CancelEventArgs e)
        {
            Connection.End();
        }
    }
}
