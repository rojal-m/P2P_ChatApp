using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P2P_Chat_App.Models
{
    public class ChatItem
    {

        private string _type;
        private string _username;
        private DateTime _timestamp = DateTime.Now;
        private string _message;

        public ChatItem() { }
        public ChatItem(string type, string username, string message)
        {
            _type = type;
            _username = username;
            _message = message;
        }

        public string usernameColor
        {
            get
            {
                return _type;
            }
            set
            {
                //Maybe add checks for invalid input
                _type = value;
            }
        }

        public string Username
        {
            get
            {
                return _username;
            }
            set
            {
                //Maybe add checks for invalid input
                _username = value;
            }
        }

        public DateTime Time
        {
            get
            {
                return _timestamp;
            }
            set { }
        }

        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                //Maybe add checks for invalid input
                _message = value;
            }
        }
    }
}
