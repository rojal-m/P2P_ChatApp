using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using P2P_Chat_App.Models;
using P2P_Chat_App.ViewModels.Commands;

namespace P2P_Chat_App.ViewModels
{
    public class MainViewModel
    {
        private ConnectionHandler _connection;
        private string _messageToSend;
        private ICommand _pushCommand;
        private ICommand _connectCommand;
        private ICommand _disconnectCommand;
        private ICommand _relistenCommand;

        private User _user;
        private User _friend;

        public ConnectionHandler Connection
        {
            get { return _connection; }
            set { _connection = value; }
        }
        public string MessageToSend
        {
            get { return _messageToSend; }
            set { _messageToSend = value; }
        }
        public ICommand PushCommand
        {
            get { return _pushCommand; }
            set { _pushCommand = value; }
        }
        public ICommand ConnectCommand
        {
            get { return _connectCommand; }
            set { _connectCommand = value; }
        }
        public ICommand DisconnectCommand
        {
            get { return _disconnectCommand; }
            set { _disconnectCommand = value; }
        }
        public ICommand ReListenCommand
        {
            get { return _relistenCommand; }
            set { _relistenCommand = value; }
        }
        public User User
        {
            get { return _user; }
            set 
            { 
                _user = value;
                OnPropertyChanged();
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public User Friend
        {
            get { return _friend; }
            set { _friend = value; }
        }

        public MainViewModel(ConnectionHandler connectionHandler)
        {
            Connection = connectionHandler;
            User = Connection.User;
            Friend = Connection.Friend;
            PushCommand = new SendMessageCommand(this);
            ConnectCommand = new ConnectCommand(this);
            DisconnectCommand = new DisconnectCommand(this);
            ReListenCommand = new ReListenCommand(this);
            MessageToSend = "Send a message";
    }
        public void sendMessage()
        {
            Connection.sendMessage(MessageToSend, _user.Name, _user.IP, _user.Port);
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
