using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
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

        private ICommand _pushCommand;
        public ICommand PushCommand
        {
            get { return _pushCommand; }
            set { _pushCommand = value; }
        }

        private ICommand _disconnectCommand;
        public ICommand DisconnectCommand
        {
            get { return _disconnectCommand; }
            set { _disconnectCommand = value; }
        }

        private ICommand _playSoundCommand;
        public ICommand PlaySoundCommand
        {
            get { return _playSoundCommand; }
            set { _playSoundCommand = value; }
        }
        private ICommand _filterHistoryCommand;
        public ICommand FilterHistoryCommand
        {
            get { return _filterHistoryCommand; }
            set { _filterHistoryCommand = value; }
        }
        private ICommand _showHistoryCommand;

        public ICommand ShowHistoryCommand
        {
            get { return _showHistoryCommand; }
            set { _showHistoryCommand = value; }
        }
        public ICommand OpenPopupCommand { get; set; }

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
        private string _selectedItem;
        public string SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                OnPropertyChanged();
            }
        }
        private string _searchBoxText;
        public string SearchBoxText
        {
            get { return _searchBoxText; }
            set
            {
                _searchBoxText = value;
                OnPropertyChanged();
            }
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
        public ObservableCollection<ChatItem> SelectedContactMessages { get; set; }
        public ObservableCollection<string> Contacts { get; set; }

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
            if (propertyName == nameof(SelectedItem))
            {
                //showHistory(SelectedItem);
            }
        }

        public MainViewModel(ConnectionHandler connectionHandler)
        {
            Connection = connectionHandler;
            User = Connection.User;
            Friend = Connection.Friend;
            SelectedContactMessages = Connection.SelectedContactMessages;
            Contacts = ChatDataBase.UpdateUserList();
            PushCommand = new SendMessageCommand(this);
            DisconnectCommand = new DisconnectCommand(this);
            OpenPopupCommand = new OpenPopupCommand(this);
            PlaySoundCommand = new PlaySoundCommand(this);
            FilterHistoryCommand = new FilterHistoryCommand(this);
            ShowHistoryCommand = new ShowHistoryCommand(this);
        }
        public void filterHistory()
        {
            Contacts.Clear();
            ObservableCollection<string> temp = ChatDataBase.UpdateUserList();
            if (temp.Any())
            {
                string search = SearchBoxText.ToLower();
                if (search != null)
                {
                    var filtered_list = from Element in temp //LINQ 
                                        where Element.ToLower().Contains(search)//making filter case insensitive
                                        select Element;

                    foreach (string Element in filtered_list)
                    {
                        Contacts.Add(Element);
                    }
                }
            }
        }
        public void showHistory(string friend)
        {
            ObservableCollection<ChatItem> temp = ChatDataBase.GetHistory(friend);
            SelectedContactMessages.Clear();
            foreach (ChatItem item in temp)
            {
                SelectedContactMessages.Add(item);
            }
            Friend.Name = friend;
        }
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

        internal void PlaySound()
        {
            /*if (_mediaPlayer == null)
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
            _mediaPlayer.Play();*/
            Connection.Beeping();
        }
    }
}
