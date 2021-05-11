using System.Data.SQLite;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace sharpkappa {
    class EmoteDatabase {
        private SQLiteConnection connection;

        public EmoteDatabase(string channel) {
            connection = new SQLiteConnection($"Data Source=data/{channel}_emotedb.sqlite3");
            if(!File.Exists($"./data/{channel}_emotedb.sqlite3")){
                SQLiteConnection.CreateFile($"./data/{channel}_emotedb.sqlite3");
            }
            connection.Open();
            generateEmoteDatabase(channel);
        }

        public void generateEmoteDatabase(string channel) {
            SQLiteCommand sqlcmd;
            string query;
            query = $@"CREATE TABLE IF NOT EXISTS '{channel}'( id TEXT, name TEXT, count INTEGER, origin TEXT )";
            sqlcmd = new SQLiteCommand(query, connection);
            int made_table = sqlcmd.ExecuteNonQuery();
            if(made_table > 0) {
                query = $"CREATE UNIQUE index '{channel}_id_name' ON '{channel}' ( id, name )";
                sqlcmd = new SQLiteCommand(query, connection);
                sqlcmd.ExecuteNonQuery();
            }
        }

        public void incrementEmotes(string channel, ChatMessage message, EmoteManager emoteManager) {
            List<string> split_msg = new List<string>(message.message.Split(' '));
            HashSet<Emote> emoteList = new HashSet<Emote>(emoteManager.getEmoteList());
            SQLiteCommand sqlcmd;
            string query;

            foreach(Emote emote in emoteList) {
                int occurances = split_msg.Where(word => word == emote.name).Count();
                if(occurances > 0) {
                    query = $"UPDATE '{channel}' SET count = count + {occurances} WHERE id = '{emote.id}'";
                    sqlcmd = new SQLiteCommand(query, connection);
                    int record_exists = sqlcmd.ExecuteNonQuery();
                    if(record_exists < 1) {
                        query = $"INSERT OR IGNORE INTO '{channel}'(id, name, count, origin) VALUES (@id, @name, @count, @origin)";
                        sqlcmd = new SQLiteCommand(query, connection);
                        sqlcmd.Parameters.AddWithValue("@id", emote.id);
                        sqlcmd.Parameters.AddWithValue("@name", emote.name);
                        sqlcmd.Parameters.AddWithValue("@count", 0);
                        sqlcmd.Parameters.AddWithValue("@origin", emote.origin);
                        sqlcmd.ExecuteNonQuery();
                        query = $"UPDATE '{channel}' SET count = count + {occurances} WHERE id = '{emote.id}'";
                        sqlcmd = new SQLiteCommand(query, connection);
                        sqlcmd.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}