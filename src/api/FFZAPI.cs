using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace sharpkappa {
    static class FFZAPI {
        private static HttpClientHandler hcHandle = new HttpClientHandler();
        private static string baseApiUrl = "https://api.frankerfacez.com/v1";

        public static async Task<List<Emote>> getGlobalEmotes() {
            try {
                using(var httpClient = new HttpClient(hcHandle, false)) {
                    using(var response = await httpClient.GetAsync($"{baseApiUrl}/set/global")) {
                        response.EnsureSuccessStatusCode();
                        string jsonString = await response.Content.ReadAsStringAsync();
                        JObject jObject = JObject.Parse(jsonString);
                        List<Emote> emoteList = new List<Emote>();
                        JArray emoticons = (JArray) jObject["sets"]["3"]["emoticons"];
                        foreach(var emote_data in emoticons) {
                            Emote emote = new Emote((string) emote_data["id"], (string) emote_data["name"], "ffz");
                            emoteList.Add(emote);
                        }
                        return emoteList;
                    }
                }
            }
            catch {
                Console.WriteLine(DateTime.Now + $": Failed to get global emotes from FFZ API");
                return new List<Emote>();
            }
        }

        public static async Task<List<Emote>> getChannelEmotes(string channel) {
            try {
                using(var httpClient = new HttpClient(hcHandle, false)) {
                    using(var response = await httpClient.GetAsync($"{baseApiUrl}/room/{channel}")) {
                        response.EnsureSuccessStatusCode();
                        string jsonString = await response.Content.ReadAsStringAsync();
                        JObject jObject = JObject.Parse(jsonString);
                        List<Emote> emoteList = new List<Emote>();
                        string set_id = (string) jObject["room"]["set"];
                        JArray emoticons = (JArray) jObject["sets"][set_id]["emoticons"];
                        foreach(var emote_data in emoticons) {
                            Emote emote = new Emote((string) emote_data["id"], (string) emote_data["name"], "ffz");
                            emoteList.Add(emote);
                        }
                        return emoteList;
                    }
                }
            }
            catch {
                Console.WriteLine(DateTime.Now + $": Failed to get channel emotes from FFZ API for {channel}");
                return new List<Emote>();
            }
        }
    }
}