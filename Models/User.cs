using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P2P_Chat_App.Models
{
    public class User
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
            set { _name = value; }
        }
        public String IP
        {
            get { return _ip; }
            set { _ip = value; }
        }
        public String Port
        {
            get { return _port; }
            set { _port = value; }
        }
    }
}
