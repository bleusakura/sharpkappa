using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace sharpkappa {
    static class FFZAPI {
        private static HttpClientHandler hcHandle = new HttpClientHandler();
        private static string baseApiUrl = "https://api.frankerfacez.com/v1";

        public static async Task<List<string>> getChannelEmotes(string channel) {
            using(var httpClient = new HttpClient(hcHandle, false)) {
                using(var response = await httpClient.GetAsync($"{baseApiUrl}/room/{channel}")) {
                    response.EnsureSuccessStatusCode();
                    string jsonString = await response.Content.ReadAsStringAsync();
                    JObject jObject = JObject.Parse(jsonString);
                    try {
                        List<string> emoteList = new List<string>();
                        string set_id = (string) jObject["room"]["set"];
                        JArray emoticons = (JArray) jObject["sets"][set_id]["emoticons"];
                        foreach(var emote_data in emoticons) {
                            emoteList.Add((string) emote_data["name"]);
                        }
                        return emoteList;
                    }
                    catch {
                        Console.WriteLine(DateTime.Now + ": Failed to get channel emotes from FFZ API");
                        return new List<string>();
                    }
                }
            }
        }
    }
}