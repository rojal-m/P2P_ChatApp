using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace P2P_Chat_App.Models
{
    public class ConnectionHandler
    {
        private static int DEFAULT_PORT = 11000;
        private static string GRAY = "#3e4042";
        private static string BLUE = "#0084ff";

        private Socket listen;
        private Thread listenThread;
        private bool keepListening;

        private Socket connect;
        private bool connectionAccepted = false;
        private User _user;
        private User _friend;

        static ManualResetEvent listningFinish = new ManualResetEvent(false);
        static ManualResetEvent listenConnected = new ManualResetEvent(false);

        private MediaPlayer _mediaPlayer;


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
            set 
            { 
                _friend = value;
            }
        }
        public ObservableCollection<ChatItem> SelectedContactMessages { get; set; }


        public ConnectionHandler()
        {
            User = new User("DefaultUser", GetLocalIP(), GetLocalPort());
            Friend = new User("", GetLocalIP(), GetLocalPort());
            SelectedContactMessages = new ObservableCollection<ChatItem>();
            Listen();
        }
        public int Relisten()
        {
            if (ReListenBox()) // if not accepted connection
            {
                keepListening = false;
                listenConnected.Set();
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
                        listenConnected.WaitOne();
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
                Thread.Sleep(1000);
                
                // Get the socket that handles the client request.
                Socket listener = (Socket)ar.AsyncState;
                // End the operation.
                Socket handler = listener.EndAccept(ar);
                connect = handler;
                listenConnected.Set();

                byte[] bytes = new byte[1024 * 5000];
                int bytesRec = connect.Receive(bytes);
                string currFriend = Encoding.UTF8.GetString(bytes, 0, bytesRec);
                

                // Accept or decline incoming connection request 

                if (!AcceptRequestBox(currFriend)) // if not accepted connection
                {
                    connectionAccepted = false;
                    int bytesSent = sendPacket(connect, new ChatProtocol("connectionDeclined", User.Name, ""));
                }
                else
                {
                    DeleteChat();
                    Friend.Name = currFriend;
                    ChatDataBase.InitConversation(Friend.Name);
                    connectionAccepted = true;
                    int bytesSent = sendPacket(connect, new ChatProtocol("connectionAccepted", User.Name, "Connected!!"));

                    //Start conversation by storing in data base and notifying the view model
                    DateTime timestamp = DateTime.Now;
                    AddChat(new ChatItem() { usernameColor = "Black", Username = "System", Time = timestamp, Message = Friend.Name+" Connected!!" });
                    ChatDataBase.AddMessage(timestamp, Friend.Name+" Connected!!", "System", "Black", Friend.Name);
                }

                string data = null;
                while (connectionAccepted)
                {
                    connected(connect, data);
                }
                connect.Shutdown(SocketShutdown.Both);
                connect.Close();
                connect = null;
            }
            catch (SocketException se)
            {
                connectionAccepted = false;
                MessageBox.Show(User.Name + "> Connection broken.L");

            }
            catch (ObjectDisposedException ox) {
                Trace.WriteLine(User.Name + "> Listning socket closed");
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
                    }
                    else
                    {
                        DeleteChat();
                        connectionAccepted = true;
                        Friend.Name = response.Username;
                        ChatDataBase.InitConversation(Friend.Name);
                        //Start conversation by storing in data base and notifying the view model
                        DateTime timestamp = DateTime.Now;
                        AddChat(new ChatItem() { usernameColor = "Black", Username = "System", Time = timestamp, Message = Friend.Name+" Connected!!" });
                        ChatDataBase.AddMessage(timestamp, Friend.Name + " Connected!!", "System", "Black", Friend.Name);

                    }
                    string data = null;
                    while (connectionAccepted)
                    {
                        connected(connect, data);
                    }
                }
                connect.Shutdown(SocketShutdown.Both);
                connect.Close();
                connect = null;
            }
            catch (SocketException se)
            {
                if (connectionAccepted)
                {
                    connectionAccepted = false;
                    MessageBox.Show(User.Name + "> Connection broken.C");
                    //p2p.MainWindow.AppWindow.ConnectionBroken();
                }
                else
                {
                    //p2p.MainWindow.AppWindow.DisconnectCallback();
                    MessageBox.Show(User.Name + "> Device refuse to connect.");
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
                    connectionAccepted = false;
                    //Start conversation by storing in data base and notifying the view model
                    AddChat(new ChatItem() { usernameColor = "Black", Username = "System", Time = timestamp, Message = responseMessage.Message });
                    ChatDataBase.AddMessage(timestamp, responseMessage.Message, "System", "Black", Friend.Name);
                }
                else if (responseMessage.Type == "message")
                {
                    //storing message in database and notifying the view model
                    AddChat(new ChatItem() { usernameColor = GRAY, Username = responseMessage.Username, Time = timestamp, Message = responseMessage.Message });
                    ChatDataBase.AddMessage(timestamp, responseMessage.Message, responseMessage.Username, GRAY, Friend.Name);
                }
                else if (responseMessage.Type == "beep")
                {
                    //storing message in database and notifying the view model
                    BEEP();
                    AddChat(new ChatItem() { usernameColor = GRAY, Username = responseMessage.Username, Time = timestamp, Message = responseMessage.Message });
                    ChatDataBase.AddMessage(timestamp, responseMessage.Message, responseMessage.Username, GRAY, Friend.Name);
                }
            }
            else
            {
                connectionAccepted = false;
            }
        }

        private void AddChat(ChatItem chatItem)
        {
            Application.Current.Dispatcher.BeginInvoke((Action)delegate
            {
                SelectedContactMessages.Add(chatItem);
            });
        }
        private void BEEP()
        {
            Application.Current.Dispatcher.BeginInvoke((Action)delegate
            {
                if (_mediaPlayer == null)
                {
                    _mediaPlayer = new MediaPlayer();
                    _mediaPlayer.MediaFailed += (o, args) =>
                    {
                        //here you can get hint of what causes the failure 
                        //from method parameter args 
                        Trace.WriteLine(args);
                    };
                }
                _mediaPlayer.Open(new Uri(@"../../../Assets/sound.wav", UriKind.Relative));
                _mediaPlayer.Play();
            });
        }
        private void DeleteChat()
        {
            Application.Current.Dispatcher.BeginInvoke((Action)delegate
            {
                SelectedContactMessages.Clear();
            });
        }

        public void sendMessage(string message)
        {
            // Here is the code which sends the data over the network.
            try
            {
                if (connectionAccepted)
                {   
                    if (message != null && message != "")
                    {
                        DateTime timestamp = DateTime.Now;
                        int bytesSend = sendPacket(connect, new ChatProtocol("message", User.Name, message));
                        AddChat(new ChatItem() { usernameColor = BLUE, Username = User.Name, Time = timestamp, Message = message });
                        ChatDataBase.AddMessage(timestamp, message, User.Name, BLUE, Friend.Name);
                    }
                }
                else
                {
                    MessageBox.Show("Not connected to anyone.");
                }
            }
            catch (SocketException sex)
            {
                connectionAccepted = false;
                MessageBox.Show("Message not sent.");
            }
        }
        public void Beeping()
        {
            // Here is the code which sends the data over the network.
            try
            {
                if (connectionAccepted)
                {
                    string message = "Beeped";
                    DateTime timestamp = DateTime.Now;
                    int bytesSend = sendPacket(connect, new ChatProtocol("beep", User.Name, message));
                    AddChat(new ChatItem() { usernameColor = BLUE, Username = User.Name, Time = timestamp, Message = message });
                    ChatDataBase.AddMessage(timestamp, message, User.Name, BLUE, Friend.Name);
                }
                else
                {
                    MessageBox.Show("Not connected to anyone.");
                }
            }
            catch (SocketException sex)
            {
                connectionAccepted = false;
                MessageBox.Show("Message not sent.");
            }
        }

        public void Disconnect()
        {
            try
            {
                if (connectionAccepted)
                {
                    int bytesSent = sendPacket(connect, new ChatProtocol("disconnect", User.Name, "Disconnected..."));
                    connectionAccepted = false;
                    DateTime timestamp = DateTime.Now;
                    AddChat(new ChatItem() { usernameColor = "Black", Username = "System", Time = timestamp, Message = "Disconnected..." });
                    ChatDataBase.AddMessage(timestamp, "Disconnected...", "System", "Black", Friend.Name);
                }
                else
                {
                    MessageBox.Show("Not connected to anyone.");
                }
            }
            catch (SocketException sex)
            {
                connectionAccepted = false;
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
            if (connectionAccepted)
            {
                int bytesSent = sendPacket(connect, new ChatProtocol("disconnect", User.Name, "Disconnected..."));
                connectionAccepted = false;
                DateTime timestamp = DateTime.Now;
                AddChat(new ChatItem() { usernameColor = "Black", Username = "System", Time = timestamp, Message = "Disconnected..." });
                ChatDataBase.AddMessage(timestamp, "Disconnected...", "System", "Black", Friend.Name);
            }
            keepListening = false;
            listenConnected.Set();
            listningFinish.WaitOne();  // Block the thread until the event is set.
            listen.Close();
            Trace.WriteLine("Listen ended");
        }
    }
}
