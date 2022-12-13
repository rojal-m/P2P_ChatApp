using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace P2P_Chat_App.Models
{
    public class ConnectionHandler
    {
        private Socket s;
        private Thread thread;
        private bool connectionAccepted = false;
        private DateTime convoDT;
        private string connectedUsername;
        private User _user;

        public User User
        {
            get { return _user; }
            set { _user = value; } 
        }

        public ConnectionHandler()
        {
            // Create a socket using the IPv4 address family, stream socket type, and TCP protocol
            s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            s.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);

            User = new User("DefaultUser", GetLocalIP(), "11000");

            // Create a new thread to listen for incoming connections
            thread = new Thread(() => {
                try
                {
                    int success = Listen();
                    if (success == 1)
                    {
                        MessageBox.Show("Listening On: " + User.Port);
                    }
                }
                catch (SocketException se)
                {
                    MessageBox.Show("Connection broken.");
                    Thread_Abort();
                }
            });

            Thread_Start();
        }
        ~ConnectionHandler()
        {
            // Abort the thread when the application is closing
            Thread_Abort();
        }
        public int Listen()
        {
            try
            {
                IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse(User.IP), Convert.ToInt32(User.Port));
                // Bind the socket to the IP address and port number to listen on
                s.Bind(localEndPoint);
                // Set the maximum number of pending connections
                s.Listen(10);
                // Loop indefinitely to accept incoming connections
                /*while (true)
                {
                    // Accept an incoming connection
                    var client = s.Accept();

                    // Send a message to the client
                    var message = Encoding.ASCII.GetBytes("Hello, world!");
                    client.Send(message);

                    // Receive a response from the client
                    var response = new byte[1024];
                    client.Receive(response);
                }*/
                //s.BeginAccept(new AsyncCallback(ListenCallback), s);
                return 1;
            }
            catch (SocketException sex)
            {
                MessageBox.Show("An error occured while trying to listen, try another port number");
                return 0;
            }

            catch (FormatException fex)
            {
                MessageBox.Show("Invalid IP or PORT number");
                return 0;
            }
            catch (ArgumentOutOfRangeException range)
            {
                MessageBox.Show("Invalid IP or PORT number");
                return 0;
            }

            catch (OverflowException oex)
            {
                MessageBox.Show("Invalid IP or PORT number");
                return 0;
            }
        }

        public void sendMessage(String message, String Name, String IP, String port)
        {
            // Here is the code which sends the data over the network.
            // No user interaction shall exist in the model.
            MessageBox.Show(message+"|"+ Name+ "|"+IP+ "|"+port);
        }

        public string GetLocalIP()
        {
            IPHostEntry host;
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return "127.0.0.1";
        }
        public void Thread_Start()
        {
            // Start the thread
            thread.Start();
        }
        public void Thread_Abort()
        {
            // Abort the thread
            thread.Abort();
            MessageBox.Show("Listen ended");
        }

    }
}
