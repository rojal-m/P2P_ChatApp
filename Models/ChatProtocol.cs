using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace P2P_Chat_App.Models
{
    public class ChatProtocol
    {
        private string _type;
        private string _username;
        private DateTime _timestamp = DateTime.Now;
        private string _message;

        public ChatProtocol(string type, string username, string message)
        {
            _type = type;
            _username = username;
            _message = message;
        }

        public string Type
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

        public DateTime Timestamp
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

        public string CreateJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
