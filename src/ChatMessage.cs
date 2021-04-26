using System;

public struct ChatMessage {
    public string username { get; }
    public string message { get; }
    public DateTime timestamp { get; }
    public string channel { get; }
    public int viewer_count { get; }
    public string game_name { get; }

    public ChatMessage(string username, string message, string currentChannel, int currentViewerCount, string currentGame) {
        this.username = username;
        this.message = message;
        this.channel = currentChannel.ToLower();
        this.timestamp = DateTime.Now;
        this.viewer_count = currentViewerCount;
        this.game_name = currentGame;
    }

    public override string ToString() => $"{timestamp.ToString()} {username}: {message}";
}
