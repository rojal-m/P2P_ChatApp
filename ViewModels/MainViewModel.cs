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
        private String _messageToSend;
        private ICommand _pushCommand;

        public ConnectionHandler Connection
        {
            get { return _connection; }
            set { _connection = value; }
        }
        public String MessageToSend
        {
            get { return _messageToSend; }
            set { _messageToSend = value; }
        }
        public ICommand PushCommand
        {
            get { return _pushCommand; }
            set { _pushCommand = value; }
        }

        public MainViewModel(ConnectionHandler connectionHandler)
        {
            this.Connection = connectionHandler;
            this.PushCommand = new SendMessageCommand(this);
        }
        public void sendMessage()
        {
            Connection.sendMessage(MessageToSend);
        }
    }
}
