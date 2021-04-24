using System;
using System.Threading.Tasks;
using AsyncAwaitBestPractices;

namespace sharpkappa
{
    class Program {
        static async Task Main(string[] args) {
            string oauth = Environment.GetEnvironmentVariable("SHARPKAPPA_OAUTH");
            string botUsername = "sharpkappa";

            var skbot = new sharpkappaBot(botUsername, oauth);
            skbot.start().SafeFireAndForget();
            await skbot.joinChannel("forsen");
            await Task.Delay(-1);
        }
    }
}
