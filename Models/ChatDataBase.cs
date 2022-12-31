using System.Data.SQLite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Collections.ObjectModel;
using System.Reflection.PortableExecutable;
using System.Diagnostics;

namespace P2P_Chat_App.Models
{
    internal class ChatDataBase
    {
        public static SQLiteConnection Connection()
        {
            string connectionString = "Data Source = chatdatabase.db; Version = 3;";
            SQLiteConnection conn;
            conn = new SQLiteConnection(connectionString);
            conn.Open();
            return conn;
        }
        public static void InitConversation(string friendName)
        {
            string sql = "CREATE TABLE IF NOT EXISTS '" + friendName + "' ( id INTEGER PRIMARY KEY AUTOINCREMENT UNIQUE, datetime TEXT, message TEXT, sender TEXT, color TEXT)";
            SQLiteConnection conn = Connection();
            try
            {
                SQLiteCommand command = new SQLiteCommand(sql, conn);
                command.ExecuteNonQuery();                                              //Table withh friend's name created
                AddToUserList(friendName);                                              //Friend's name is then added to table for getting the list of users 
            }
            catch (SQLiteException se)
            {
                throw se;
            }
            conn.Close();
        }
        public static void AddMessage(DateTime dt, string message, string username, string color, string friend)
        {
            SQLiteConnection conn = Connection();
            string insertSql = "INSERT INTO " + friend + " (datetime, message, sender, color) VALUES ('"+ dt.ToString() + "','" + message + "','"+ username +"','"+ color + "')";
            try
            {
                SQLiteCommand command2 = new SQLiteCommand(insertSql, conn);
                command2.ExecuteNonQuery();                                             //Inserted a message inside the DBtable of the friend(receiver)
            }
            catch (SQLiteException se)
            {
                throw se;
            }
            conn.Close();
        }
        

        public static void AddToUserList(string username)
        {
            string sqlcreate = "CREATE TABLE IF NOT EXISTS Users ( id INTEGER PRIMARY KEY AUTOINCREMENT UNIQUE, username TEXT UNIQUE)"; 
            string insertSql = "INSERT OR REPLACE INTO Users (username) values('" + username + "')";
            SQLiteConnection conn = Connection();
            try
            {
                SQLiteCommand command = new SQLiteCommand(sqlcreate, conn);
                command.ExecuteNonQuery();                                              //Table 'Users' created to store user
                SQLiteCommand command2 = new SQLiteCommand(insertSql, conn);
                command2.ExecuteNonQuery();                                             //username is then inserted into it
            }
            catch (SQLiteException se)
            {
                throw se;
            }
            conn.Close();
        }

        public static ObservableCollection<string> UpdateUserList()
        {
            ObservableCollection<string> userList = new ObservableCollection<string>();
            try
            {
                SQLiteConnection conn = Connection();
                string sql = "SELECT * FROM Users";
                SQLiteCommand command = new SQLiteCommand(sql, conn);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    if (reader["username"] != null)
                    {
                        userList.Add(reader.GetString("username"));
                    }
                }
                conn.Close();
                return userList;
            }
            catch (SQLiteException ex)
            {
                ObservableCollection<string> userList1 = new ObservableCollection<string>();
                return userList1;
            }
        }

        public static ObservableCollection<ChatItem> GetHistory(string selectedUser)
        {
            ObservableCollection<ChatItem> messageHistory = new ObservableCollection<ChatItem>();
            string sql = "SELECT * FROM " + selectedUser;
            try
            {
                SQLiteConnection conn = Connection();
                SQLiteCommand command = new SQLiteCommand(sql, conn);
                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {/*
                    if (!reader.IsDBNull(nrOfRows++))                                    //checking if column is null as + counting rows
                    {*/
                    //datetime TEXT, message TEXT, sender TEXT, color TEXT
                    DateTime timestamp = DateTime.Parse(reader.GetString("datetime"));
                    messageHistory.Add(new ChatItem() { usernameColor = reader.GetString("color"), Username = reader.GetString("sender"), Time = timestamp, Message = reader.GetString("message") });
                }
                if (messageHistory.Count < 1)
                {
                    throw new Exception("There is no chat history to show..");
                }

                return messageHistory;
            }
            catch (SQLiteException se)
            {
                ObservableCollection<ChatItem> messageHistoryFail = new ObservableCollection<ChatItem>();
                messageHistoryFail.Add(new ChatItem() { usernameColor = "Black", Username = "System", Time = DateTime.Now, Message = "Start chatting with friends!" });
                return messageHistoryFail;
            }
            catch (Exception ce)
            {
                ObservableCollection<ChatItem> messageHistoryFail = new ObservableCollection<ChatItem>();
                messageHistoryFail.Add(new ChatItem() { usernameColor = "Black", Username = "System", Time = DateTime.Now, Message = ce.Message });
                return messageHistoryFail;
            }
        }
    }
}
