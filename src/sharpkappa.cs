using System;
using System.Threading.Tasks;
using System.IO;
using System.Net.Sockets;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using AsyncAwaitBestPractices;

namespace sharpkappa
{
    public class SharpkappaBot {
        const string ip = "irc.chat.twitch.tv";
        const int port = 6697;
        private TaskCompletionSource<int> connected = new TaskCompletionSource<int>();

        private string nick;
        private string oauth;
        private StreamReader streamReader;
        private StreamWriter streamWriter;
        private string currentChannel = "";
        private string channelId = "";
        private int currentViewerCount = 0;
        private string currentGame = "";
        private ChatDatabase chatDatabase;
        private EmoteDatabase emoteDatabase;
        private EmoteManager emoteManager;

        public SharpkappaBot(string nick, string oauth) {
            this.nick = nick;
            this.oauth = oauth;
        }

        private bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) {
            return sslPolicyErrors == SslPolicyErrors.None;
        }

        public async Task start(string channel) {
            var tcpClient = new TcpClient();
            await tcpClient.ConnectAsync(ip, port);
            SslStream sslStream = new SslStream(tcpClient.GetStream(), false, ValidateServerCertificate, null);
            await sslStream.AuthenticateAsClientAsync(ip);
            
            streamReader = new StreamReader(sslStream);
            streamWriter = new StreamWriter(sslStream) { NewLine = "\r\n", AutoFlush = true };

            await streamWriter.WriteLineAsync("CAP REQ :twitch.tv/tags");
            await streamWriter.WriteLineAsync($"PASS {oauth}");
            await streamWriter.WriteLineAsync($"NICK {nick}");
            connected.SetResult(0);

            await joinChannel(channel);
            await refreshStreamsData(true);
            refreshStreamsData().SafeFireAndForget();

            chatDatabase = new ChatDatabase(currentChannel);
            emoteDatabase = new EmoteDatabase(currentChannel);
            emoteManager = new EmoteManager(currentChannel, channelId);
            await emoteManager.start();

            string line = "";
            while(true) {
                line = await streamReader.ReadLineAsync();

                string[] split = line.Split(" ");
                if(line.StartsWith("PING")) {
                    await streamWriter.WriteLineAsync($"PONG {split[1]}");
                }

                if(split.Length > 3 && split[2] == "PRIVMSG") {
                    ChatMessage twitchMessage = processIRCMessage(line, split);
                    chatDatabase.appendMessage(twitchMessage);
                    emoteDatabase.incrementEmotes(currentChannel, twitchMessage, emoteManager);
                }
            }
        }

        public ChatMessage processIRCMessage(string line, string[] split) {
            string tags = split[0];
            string privmsg = line.Substring(line.IndexOf("PRIVMSG"));

            string username = split[1].Substring(1, split[1].IndexOf("!")-1);
            string message = privmsg.Substring(privmsg.IndexOf(':', 1)+1);
            int subscriber = Int32.Parse(tags.Substring(tags.IndexOf("subscriber=")+11, 1));
            return new ChatMessage(username, message, subscriber, currentChannel, currentViewerCount, currentGame);
        }

        public async Task sendMessage(string channel, string message) {
            await connected.Task;
            await streamWriter.WriteLineAsync($"PRIVMSG #{channel} :{message}");
        }

        public async Task joinChannel(string channel) {
            await connected.Task;
            await streamWriter.WriteLineAsync($"JOIN #{channel}");
            currentChannel = channel.ToLower();
            channelId = await TwitchAPI.getUserData(channel);
        }

        public async Task refreshStreamsData(bool prefetch=false) {
            if(prefetch) {
                Tuple<string, int> streamData = await TwitchAPI.getStreamsData(currentChannel);
                currentGame = streamData.Item1;
                currentViewerCount = streamData.Item2;
            }
            while(true && !prefetch) {
                await Task.Delay(TimeSpan.FromMinutes(2));
                Tuple<string, int> streamData = await TwitchAPI.getStreamsData(currentChannel);
                currentGame = streamData.Item1;
                currentViewerCount = streamData.Item2;
            }
        }
    }
}
