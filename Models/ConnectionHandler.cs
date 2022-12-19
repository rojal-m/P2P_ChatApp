using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media.Imaging;

namespace P2P_Chat_App.Models
{
    public class ConnectionHandler
    {
        private static int DEFAULT_PORT = 11000; 
        private Socket listen;
        private Thread listenThread;
        private bool keepListening;

        private Socket connect;
        private bool connectionAccepted = false;
        private DateTime convoDT;
        private User _user;
        private User _friend;

        static ManualResetEvent listningFinish = new ManualResetEvent(false);



        public User User
        {
            get { return _user; }
            set 
            { 
                _user = value;
            } 
        }

        public User Friend
        {
            get { return _friend; }
            set { _friend = value; }
        }

        public ConnectionHandler()
        {

            

            User = new User("DefaultUser", GetLocalIP(), GetLocalPort());
            Friend = new User("DefaultFriend", GetLocalIP(), GetLocalPort());

            Listen();
        }
        public int Relisten()
        {
            if (ReListenBox()) // if not accepted connection
            {
                keepListening = false;
                listningFinish.WaitOne();
                listen.Close();
                Listen();
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public int Listen()
        {
            try
            {

                // Create a socket using the IPv4 address family, stream socket type, and TCP protocol
                listen = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                listen.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);
                IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse(User.IP), Convert.ToInt32(User.Port));
                // Bind the socket to the IP address and port number to listen on
                listen.Bind(localEndPoint);
                // Set the maximum number of pending connections
                listen.Listen(10);
                keepListening = true;
                // Create a new thread to listen for incoming connections
                listenThread = new Thread(() => {
                    while (keepListening)
                    {
                        // Begin accepting incoming connections asynchronously.
                        listen.BeginAccept(new AsyncCallback(ListenCallback), listen);

                        // Wait for the asynchronous operation to complete.
                        Thread.Sleep(1000);
                    }
                    listningFinish.Set();  // Set the event and unblock thread 1.
                });
                // Start the thread
                listenThread.Start();
                return 1;
            }
            catch (SocketException sex)
            {
                MessageBox.Show("An error occured while trying to listen, try another port number"+ sex);
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
        private void ListenCallback(IAsyncResult ar)
        {
            try
            {
                // Get the socket that handles the client request.
                Socket listener = (Socket)ar.AsyncState;
                // End the operation.
                Socket handler = listener.EndAccept(ar);

                byte[] bytes = new byte[1024 * 5000];
                int bytesRec = handler.Receive(bytes);
                string currFriend = Encoding.UTF8.GetString(bytes, 0, bytesRec);
                Friend.Name = currFriend;
                convoDT = DateTime.Now;

                // Accept or decline incoming connection request 

                if (!AcceptRequestBox(currFriend)) // if not accepted connection
                {
                    connectionAccepted = false;
                    int bytesSent = sendPacket(handler, new ChatProtocol("connectionDeclined", User.Name, ""));

                    MessageBox.Show(User.Name + ": Connection declined to " + Friend.Name);
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                    Trace.WriteLine("socket closed");
                }
                else
                {
                    connectionAccepted = true;
                    int bytesSent = sendPacket(handler, new ChatProtocol("connectionAccepted", User.Name, ""));

                    MessageBox.Show(User.Name+": Connection accepted to " + Friend.Name);
                    connect = handler;
                    //Start conversation by storing in data base and notifying the view model
                }
                
                string data = null;
                while (connectionAccepted)
                {
                    connected(connect, data);
                }
                Trace.WriteLine("Connection Ended.");
            }
            catch (SocketException se)
            {
                connectionAccepted = false;
                listen.Shutdown(SocketShutdown.Both);
                listen.Close();
            }
            catch (ObjectDisposedException ox) {
                Trace.WriteLine("Listning socket closed");
            }
        }

        private int sendPacket(Socket handler, ChatProtocol Request)
        {
            //ChatProtocol Request = new ChatProtocol(messageType, username, message);
            string jsonRequest = JsonConvert.SerializeObject(Request);
            byte[] msg = Encoding.UTF8.GetBytes(jsonRequest);
            int bytesSent = handler.Send(msg);
            return bytesSent;
        }

        private ChatProtocol receivePacket(Socket connector)
        {

            byte[] msgBytes = new byte[1024 * 5000];
            int msgRec = connector.Receive(msgBytes);
            if (msgRec != 0)
            {
                string data = Encoding.UTF8.GetString(msgBytes, 0, msgRec);
                return JsonConvert.DeserializeObject<ChatProtocol>(data);
            } 
            else
            {
                return null;
            }
        }

        public int Connect()
        {
            try
            {
                if(Friend.IP == User.IP && Friend.Port == User.Port)
                {
                    MessageBox.Show("Connecting to yourself");
                    return 0;
                }
                else if (connectionAccepted)
                {
                    MessageBox.Show("Already connected");
                    return 0;
                }
                connect = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                connect.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);
                IPEndPoint friendEndPoint = new IPEndPoint(IPAddress.Parse(Friend.IP), Convert.ToInt32(Friend.Port)); 
                connect.BeginConnect(friendEndPoint, new AsyncCallback(ConnectCallback), connect);
                return 1;
            }
            catch (SocketException sex)
            {
                MessageBox.Show("There was a problem connecting"+sex);
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
            catch (ObjectDisposedException ox)
            {
                MessageBox.Show("Cannot connect again. Restart.");
                return 0;
            }
        }
        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                connect = (Socket)ar.AsyncState;
                connect.EndConnect(ar);

                byte[] msg = Encoding.UTF8.GetBytes(User.Name);
                int bytesSent = connect.Send(msg);

                ChatProtocol response = receivePacket(connect);
                if (response != null)
                {
                    if (response.Type == "connectionDeclined")
                    {
                        MessageBox.Show(User.Name + ": Connection declined by " + response.Username);
                        connectionAccepted = false;
                        connect.Shutdown(SocketShutdown.Both);
                        connect.Close();
                    }
                    else
                    {
                        connectionAccepted = true;
                        Friend.Name = response.Username;
                        MessageBox.Show(User.Name + ": Connection accepted by " + Friend.Name);

                        //Start conversation by storing in data base and notifying the view model
                    }
                    string data = null;
                    while (connectionAccepted)
                    {
                        connected(connect, data);
                    }
                }
                
            }
            catch (SocketException se)
            {
                if (connectionAccepted)
                {
                    connectionAccepted = false;
                    connect.Shutdown(SocketShutdown.Both);
                    connect.Close();
                    MessageBox.Show("Connection broken.");
                    //p2p.MainWindow.AppWindow.ConnectionBroken();
                }
                else
                {
                    //p2p.MainWindow.AppWindow.DisconnectCallback();
                    MessageBox.Show("There is no friend listening on that port!");
                }
            }
        }
        private void connected(Socket handler, string data)
        {
            ChatProtocol responseMessage = receivePacket(handler);
            if (handler.Connected && responseMessage != null)
            {
                DateTime timestamp = DateTime.Now;
                if (responseMessage.Type == "disconnect")
                {
                    int bytesSend = sendPacket(handler, new ChatProtocol("disconnect", User.Name, " disconnected"));
                    connectionAccepted = false;
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();

                    //Start conversation by storing in data base and notifying the view model
                    MessageBox.Show(responseMessage.Username + " disconnected.");
                }
                else if (responseMessage.Type == "message")
                {
                    //storing message in database and notifying the view model
                    MessageBox.Show(timestamp.ToString() + " " + responseMessage.Username + " said : " + responseMessage.Message);
                }
            }
            else
            {
                MessageBox.Show("End connection");
                connectionAccepted = false;
            }
        }

        public void sendMessage(string message, string Name, string IP, string port)
        {
            // Here is the code which sends the data over the network.
            try
            {
                if (connectionAccepted)
                {
                    int bytesSend = sendPacket(connect, new ChatProtocol("message", User.Name, message));

                }
                else
                {
                    MessageBox.Show("Not connected to anyone.");
                }
            }
            catch (SocketException sex)
            {
                connectionAccepted = false;
                connect.Shutdown(SocketShutdown.Both);
                connect.Close();
                MessageBox.Show("Message not sent.");
            }
        }

        public void Disconnect()
        {
            try
            {
                if (connectionAccepted)
                {
                    int bytesSent = sendPacket(connect, new ChatProtocol("disconnect", User.Name, "disconnected"));
                }
                else
                {
                    MessageBox.Show("Not connected to anyone.");
                }
            }
            catch (SocketException sex)
            {
                connectionAccepted = false;
                connect.Shutdown(SocketShutdown.Both);
                connect.Close();
                MessageBox.Show("Couldn't disconnect.");
            }
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

        public string GetLocalPort()
        {
            int temp_port = DEFAULT_PORT;
            for (int i = DEFAULT_PORT; i == temp_port; i++  )
            {
                TcpListener listener = new(IPAddress.Parse(GetLocalIP()), i);
                try
                {
                    listener.Start();
                }
                catch (SocketException)
                {
                    temp_port++;
                }
                finally
                {
                    listener.Stop();
                }
            }
            return temp_port.ToString();
        }
      
        private bool AcceptRequestBox(string connectingFriendname)
        {
            if (MessageBox.Show("Connection request from: " + connectingFriendname + " \nAccept the request?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        private bool ReListenBox()
        {
            if (MessageBox.Show("Do you want to re Listen?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public void End()
        {
            keepListening = false;
            listningFinish.WaitOne();  // Block the thread until the event is set.
            if(listen.Connected)
            {
                listen.Shutdown(SocketShutdown.Both);
            }
            listen.Close();
            Trace.WriteLine("Listen ended");
        }
    }
}
