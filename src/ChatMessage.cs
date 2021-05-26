using System;

public struct ChatMessage {
    public string username { get; }
    public string message { get; }
    public string channel { get; }
    public int subscriber { get; }
    public int viewer_count { get; }
    public string game_name { get; }
    public DateTime timestamp { get; }

    public ChatMessage(string username, string message, int subscriber, string currentChannel, int currentViewerCount, string currentGame) {
        this.username = username;
        this.message = message;
        this.channel = currentChannel.ToLower();
        this.subscriber = subscriber;
        this.viewer_count = currentViewerCount;
        this.game_name = currentGame;
        this.timestamp = DateTime.Now;
    }

    public override string ToString() => $"{timestamp.ToString()} {username}: {message}";
}
