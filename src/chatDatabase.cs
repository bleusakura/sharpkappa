using System.Data.SQLite;
using System.IO;
using System;

namespace sharpkappa {
    class chatDatabase {
        private SQLiteConnection connection;
        private bool generated = false;

        public chatDatabase(string channel) {
            connection = new SQLiteConnection("Data Source=database.sqlite3");
            if(!File.Exists("./database.sqlite3")){
                SQLiteConnection.CreateFile("database.sqlite3");
            }
            generateChatDatabase(channel);
        }

        public void openConnection() {
            if(connection.State != System.Data.ConnectionState.Open) {
                connection.Open();
            }
        }

        public void closeConnection() {
            if(connection.State != System.Data.ConnectionState.Closed) {
                connection.Close();
            }
        }

        public void generateChatDatabase(string channel) {
            openConnection();
            string query = $@"CREATE TABLE IF NOT EXISTS {channel}(timestamp INTEGER, username TEXT, message TEXT, channel TEXT)";
            SQLiteCommand sqlcmd = new SQLiteCommand(query, connection);
            sqlcmd.ExecuteNonQuery();
            closeConnection();
            generated = true;
        }

        public void appendMessage(chatMessage chatmessage) {
            if(generated) {
                openConnection();
                string query = $"INSERT INTO {chatmessage.channel}(timestamp, username, message, channel) VALUES (@timestamp, @username, @message, @channel)";
                SQLiteCommand sqlcmd = new SQLiteCommand(query, connection);
                sqlcmd.Parameters.AddWithValue("@timestamp", chatmessage.timestamp);
                sqlcmd.Parameters.AddWithValue("@username", chatmessage.username);
                sqlcmd.Parameters.AddWithValue("@message", chatmessage.message);
                sqlcmd.Parameters.AddWithValue("@channel", chatmessage.channel);
                sqlcmd.ExecuteNonQuery();
                closeConnection();
            }
        }
    }
}