using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Configuration;

namespace sharpkappa {
    static class TwitchAPI {
        private static HttpClientHandler hcHandle = new HttpClientHandler();
        private static string access_token = ConfigurationManager.AppSettings.Get("SHARPKAPPA_ACCESSTOKEN");
        private static string client_id = ConfigurationManager.AppSettings.Get("SHARPKAPPA_CLIENTID");
        private static string client_secret = ConfigurationManager.AppSettings.Get("SHARPKAPPA_CLIENTSECRET");

        public static async Task<string> requestAccessToken() {
            // have to add clause to regenerate expired key
            using (var httpClient = new HttpClient(hcHandle, false)) {
                List<KeyValuePair<string, string>> parameters = new List<KeyValuePair<string, string>>();
                parameters.Add(new KeyValuePair<string, string>("client_id", client_id));
                parameters.Add(new KeyValuePair<string, string>("client_secret", client_secret));
                parameters.Add(new KeyValuePair<string, string>("grant_type", "client_credentials"));
                var content = new FormUrlEncodedContent(parameters);

                using (var response = await httpClient.PostAsync("https://id.twitch.tv/oauth2/token", content)) {
                    try{
                        response.EnsureSuccessStatusCode();
                        string jsonString = await response.Content.ReadAsStringAsync();
                        JObject jsonObject = JObject.Parse(jsonString);

                        string oauthtoken = (string) jsonObject["access_token"];
                        return oauthtoken;
                    }
                    catch {
                        throw new Exception("Unable to generate Access Token for API use");
                        
                    }
                }
            }
        }

        public static async Task<string> getUserData(string channel) {
            using (var httpClient = new HttpClient(hcHandle, false)) {
                httpClient.DefaultRequestHeaders.Add("Client-ID", client_id);
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", access_token);
                httpClient.Timeout = TimeSpan.FromSeconds(5);

                using (var response = await httpClient.GetAsync($"https://api.twitch.tv/helix/users?login={channel}")) {
                    try {
                        response.EnsureSuccessStatusCode();
                        string jsonString = await response.Content.ReadAsStringAsync();
                        JObject jsonObject = JObject.Parse(jsonString);
                        JToken jData = jsonObject["data"][0];
                        string id = (string) jData["id"];
                        return id;
                    }
                    catch {
                        string id = "";
                        Console.WriteLine(DateTime.Now + $": Failed to get channel id from Twitch API for {channel}");
                        return id;
                    }
                }
            }
        }

        public static async Task<Tuple<string, int>> getStreamsData(string channel) {
            using (var httpClient = new HttpClient(hcHandle, false)) {
                httpClient.DefaultRequestHeaders.Add("Client-ID", client_id);
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", access_token);
                httpClient.Timeout = TimeSpan.FromSeconds(10);

                using (var response = await httpClient.GetAsync($"https://api.twitch.tv/helix/streams?user_login={channel}")) {
                    try {
                        response.EnsureSuccessStatusCode();
                        string jsonString = await response.Content.ReadAsStringAsync();
                        JObject jsonObject = JObject.Parse(jsonString);
                        JToken jData = jsonObject["data"][0];
                        int viewer_count = (int) jData["viewer_count"];
                        string game_name = (string) jData["game_name"];
                        return (new Tuple<string, int>(game_name, viewer_count));
                    }
                    catch {
                        int viewer_count = 0;
                        string game_name = "";
                        Console.WriteLine(DateTime.Now + $": Failed to get streams data from Twitch API for {channel}");
                        return (new Tuple<string, int>(game_name, viewer_count));
                    }
                }
            }
        }

        // unofficial api through twitchemotes.com, no new emote api from twitch since legacy atm
        public static async Task<List<Emote>> getChannelEmotes(string channel_id = "0") {
            using(var httpClient = new HttpClient()) {
                using(var response = await httpClient.GetAsync($"https://api.twitchemotes.com/api/v4/channels/{channel_id}")) {
                    try {
                        response.EnsureSuccessStatusCode();
                        string jsonString = await response.Content.ReadAsStringAsync();
                        JObject jObject = JObject.Parse(jsonString);
                        List<Emote> emoteList = new List<Emote>();
                        JArray emotes = (JArray) jObject["emotes"];
                        foreach(var emote_data in emotes) {
                            Emote emote = new Emote((string) emote_data["id"], (string) emote_data["code"], "twitch");
                            emoteList.Add(emote);
                        }
                        return emoteList;
                    }
                    catch {
                        Console.WriteLine(DateTime.Now + $": Failed to get emote data from twitchemotes API for {channel_id}");
                        return new List<Emote>();
                    }
                }
            }
        }
    }
}