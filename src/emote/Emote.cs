using System;

namespace sharpkappa {
    public struct Emote {
        public string id { get; }
        public string name { get; }
        public string origin { get; }

        public Emote(string id, string name, string origin) {
            this.id = id;
            this.name = name;
            this.origin = origin;
        }

        public override string ToString() => $"{name}";
    }
}