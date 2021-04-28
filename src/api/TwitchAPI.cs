using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Configuration;

namespace sharpkappa {
    class TwitchAPI {
        private static HttpClientHandler hcHandle;
        private static string access_token;
        private static string client_id;
        private static string client_secret;

        public TwitchAPI() {
            hcHandle = new HttpClientHandler();

            //oauthGenerator.requestOAuthToken();
            client_id = ConfigurationManager.AppSettings.Get("SHARPKAPPA_CLIENTID");
            client_secret = ConfigurationManager.AppSettings.Get("SHARPKAPPA_CLIENTSECRET");
            access_token = ConfigurationManager.AppSettings.Get("SHARPKAPPA_ACCESSTOKEN");
        }

        public async Task<string> requestAccessToken() {
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

        public async Task<Tuple<string, int>> getStreamsData(string channel) {
            using (var httpClient = new HttpClient(hcHandle, false)) {
                httpClient.DefaultRequestHeaders.Add("Client-ID", client_id);
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", access_token);
                httpClient.Timeout = TimeSpan.FromSeconds(5);

                using (var response = await httpClient.GetAsync($"https://api.twitch.tv/helix/streams?user_login={channel}")) {
                    response.EnsureSuccessStatusCode();
                    string jsonString = await response.Content.ReadAsStringAsync();
                    JObject jsonObject = JObject.Parse(jsonString);
                    try {
                        JToken jData = jsonObject["data"][0];
                        int viewer_count = (int) jData["viewer_count"];
                        string game_name = (string) jData["game_name"];
                        return (new Tuple<string, int>(game_name, viewer_count));
                    }
                    catch {
                        int viewer_count = 0;
                        string game_name = "";
                        return (new Tuple<string, int>(game_name, viewer_count));
                    }
                }
            }
        }
    }
}