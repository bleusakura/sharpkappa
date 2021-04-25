using System;
using System.Threading.Tasks;
using AsyncAwaitBestPractices;
using System.Collections.Generic;

namespace sharpkappa
{
    class Program {
        static async Task Main(string[] args) {
            string oauth = Environment.GetEnvironmentVariable("SHARPKAPPA_OAUTH");
            string botUsername = "sharpkappa";
            List<string> targetChannels = new List<string>();
            List<sharpkappaBot> bots = new List<sharpkappaBot>();

            if(args.Length > 0) {
                for(int i = 0; i < args.Length; i++) {
                    targetChannels.Add(args[i]); 
                }
            }
            for(int i = 0; i < targetChannels.Count; i++) {
                bots.Add(new sharpkappaBot(botUsername, oauth));
                bots[i].start(targetChannels[i]).SafeFireAndForget();
            }
            await Task.Delay(-1);
        }
    }
}
