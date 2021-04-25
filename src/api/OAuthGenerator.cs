using System;
using System.Net.Http;
using System.Configuration;

namespace sharpkappa {
    class OAuthGenerator {
        private string twitchAuthURL = String.Format("https://id.twitch.tv/oauth2/authorize");
        private HttpClient httpClient;

        private string client_id;
        private string redirect_uri;
        private string response_type;
        private string scope;
        private string generatedToken;

        public OAuthGenerator() {
            httpClient = new HttpClient();
            client_id = Environment.GetEnvironmentVariable("SHARPKAPPA_CLIENTID");
            redirect_uri = "https://localhost";
            response_type = "token";
        }

        public void requestOAuthToken() {
            var responseTask = httpClient.GetAsync($@"{twitchAuthURL}?
            response_type={response_type}&
            client_id={client_id}&
            redirect_uri={redirect_uri}");
            responseTask.Wait();
            if(responseTask.IsCompleted) {
                var result = responseTask.Result;
                if(result.IsSuccessStatusCode) {
                    var messageTask = result.Content.ReadAsStreamAsync();
                    messageTask.Wait();
                    Console.WriteLine(messageTask.Result);
                }
                // need to implement httpcontext to grab current URI fragment to retrieve sent oauth token
            }
        }

        public string getOAuthToken() { return generatedToken; }
    }
}