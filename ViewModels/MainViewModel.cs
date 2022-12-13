using System;
using System.Collections.Generic;
using System.Linq;
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
        private User _user;

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
        public User User
        {
            get { return _user; }
            set { _user = value; }
        }

        public MainViewModel(ConnectionHandler connectionHandler)
        {
            Connection = connectionHandler;
            User = Connection.User;
            PushCommand = new SendMessageCommand(this);
            MessageToSend = "Send a message";
    }
        public void sendMessage()
        {
            Connection.sendMessage(MessageToSend, _user.Name, _user.IP, _user.Port);
        }
    }
}
