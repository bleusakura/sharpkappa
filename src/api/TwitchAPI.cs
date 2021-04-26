using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace sharpkappa {
    class TwitchAPI {
        private static HttpClientHandler hcHandle;
        private OAuthGenerator oauthGenerator;
        private static string OAuthToken;
        private static string clientID;

        public TwitchAPI() {
            hcHandle = new HttpClientHandler();
            oauthGenerator = new OAuthGenerator();

            //oauthGenerator.requestOAuthToken();
            OAuthToken = oauthGenerator.getOAuthToken();
            clientID = oauthGenerator.getClientID();
        }

        public async Task<Tuple<string, int>> getStreamsData(string channel) {
            using (var httpClient = new HttpClient(hcHandle, false)) {
                httpClient.DefaultRequestHeaders.Add("Client-ID", clientID);
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", OAuthToken);
                httpClient.Timeout = TimeSpan.FromSeconds(5);

                using (var response = await httpClient.GetAsync($"https://api.twitch.tv/helix/streams?user_login={channel}")) {
                    response.EnsureSuccessStatusCode();
                    string jsonString = await response.Content.ReadAsStringAsync();
                    JObject jsonObject = JObject.Parse(jsonString);
                    JToken jData = jsonObject["data"][0];
                    int viewer_count = (int) jData["viewer_count"];
                    string game_name = (string) jData["game_name"];
                    return (new Tuple<string, int>(game_name, viewer_count));
                }
            }
        }
    }
}