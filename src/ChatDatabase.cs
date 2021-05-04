using System.Data.SQLite;
using System.IO;
using System;

namespace sharpkappa {
    class ChatDatabase {
        private SQLiteConnection connection;
        private bool generated = false;

        public ChatDatabase(string channel) {
            connection = new SQLiteConnection($"Data Source=data/{channel}_db.sqlite3");
            if(!File.Exists($"./data/{channel}_db.sqlite3")){
                SQLiteConnection.CreateFile($"./data/{channel}_db.sqlite3");
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
            string query = $@"CREATE TABLE IF NOT EXISTS '{channel}'(timestamp INTEGER, username TEXT, message TEXT, channel TEXT, game_name STRING, viewer_count INTEGER)";
            SQLiteCommand sqlcmd = new SQLiteCommand(query, connection);
            sqlcmd.ExecuteNonQuery();
            closeConnection();
            generated = true;
        }

        public void appendMessage(ChatMessage chatmessage) {
            if(generated) {
                openConnection();
                string query = $"INSERT INTO '{chatmessage.channel}'(timestamp, username, message, channel, game_name, viewer_count) VALUES (@timestamp, @username, @message, @channel, @game_name, @viewer_count)";
                SQLiteCommand sqlcmd = new SQLiteCommand(query, connection);
                sqlcmd.Parameters.AddWithValue("@timestamp", chatmessage.timestamp);
                sqlcmd.Parameters.AddWithValue("@username", chatmessage.username);
                sqlcmd.Parameters.AddWithValue("@message", chatmessage.message);
                sqlcmd.Parameters.AddWithValue("@channel", chatmessage.channel);
                sqlcmd.Parameters.AddWithValue("@game_name", chatmessage.game_name);
                sqlcmd.Parameters.AddWithValue("@viewer_count", chatmessage.viewer_count);
                sqlcmd.ExecuteNonQuery();
                closeConnection();
            }
        }
    }
}
