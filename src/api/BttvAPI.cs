using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace sharpkappa {
    static class BttvAPI {
        private static HttpClientHandler hcHandle = new HttpClientHandler();
        private static string baseApiUrl = "https://api.betterttv.net/3/cached";

        public static async Task<List<Emote>> getGlobalEmotes() {
            using(var httpClient = new HttpClient(hcHandle, false)) {
                using(var response = await httpClient.GetAsync($"{baseApiUrl}/emotes/global")) {
                    response.EnsureSuccessStatusCode();
                    string jsonString = await response.Content.ReadAsStringAsync();
                    JArray emoticons = JArray.Parse(jsonString);
                    try {
                        List<Emote> emoteList = new List<Emote>();
                        foreach(var emote_data in emoticons) {
                            Emote emote = new Emote((string) emote_data["id"], (string) emote_data["code"], "bttv");
                            emoteList.Add(emote);
                        }
                        return emoteList;
                    }
                    catch {
                        Console.WriteLine(DateTime.Now + ": Failed to get global emotes from Bttv API");
                        return new List<Emote>();
                    }
                }
            }
        }

        public static async Task<List<Emote>> getChannelEmotes(string channel_id) {
            using(var httpClient = new HttpClient(hcHandle, false)) {
                using(var response = await httpClient.GetAsync($"{baseApiUrl}/users/twitch/{channel_id}")) {
                    response.EnsureSuccessStatusCode();
                    string jsonString = await response.Content.ReadAsStringAsync();
                    JObject jObject = JObject.Parse(jsonString);
                    try {
                        List<Emote> emoteList = new List<Emote>();
                        JArray channelEmotes = (JArray) jObject["channelEmotes"];
                        JArray sharedEmotes = (JArray) jObject["sharedEmotes"];
                        channelEmotes.Merge(sharedEmotes);
                        foreach(var emote_data in channelEmotes) {
                            Emote emote = new Emote((string) emote_data["id"], (string) emote_data["code"], "bttv");
                            emoteList.Add(emote);
                        }
                        return emoteList;
                    }
                    catch {
                        Console.WriteLine(DateTime.Now + ": Failed to get channel emotes from Bttv API");
                        return new List<Emote>();
                    }
                }
            }
        }
    }
}