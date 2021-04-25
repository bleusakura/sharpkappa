using System;

public struct ChatMessage {
    public string username { get; }
    public string message { get; }
    public DateTime timestamp { get; }
    public string channel { get; }

    public ChatMessage(string username, string message, string currentChannel) {
        this.username = username;
        this.message = message;
        this.channel = currentChannel.ToLower();
        this.timestamp = DateTime.Now;
    }

    public override string ToString() => $"{timestamp.ToString()} {username}: {message}";
}