using System;
using System.Threading.Tasks;
using AsyncAwaitBestPractices;
using System.Collections.Generic;
using System.Configuration;

namespace sharpkappa
{
    class Program {
        static async Task Main(string[] args) {
            if(args.Length > 0) {
                if(args[0] == "--generateAccessToken" || args[0] == "-g") {
                    string generateAccessToken = await TwitchAPI.requestAccessToken();
                    Console.WriteLine(generateAccessToken);
                    Console.WriteLine("Place this in App.config for the value for key SHARPKAPPA_ACCESSTOKEN");
                    System.Environment.Exit(0);
                }
            }
            string oauth = ConfigurationManager.AppSettings.Get("SHARPKAPPA_OAUTH");
            string botUsername = "sharpkappa";
            List<string> targetChannels = new List<string>();
            List<SharpkappaBot> bots = new List<SharpkappaBot>();

            if(args.Length > 0) {
                for(int i = 0; i < args.Length; i++) {
                    targetChannels.Add(args[i]); 
                }
            }
            for(int i = 0; i < targetChannels.Count; i++) {
                bots.Add(new SharpkappaBot(botUsername, oauth));
                bots[i].start(targetChannels[i]).SafeFireAndForget();
            }
            await Task.Delay(-1);
        }
    }
}
