using System;
using System.Threading.Tasks;
using AsyncAwaitBestPractices;

namespace sharpkappa
{
    class Program {
        static async Task Main(string[] args) {
            string oauth = Environment.GetEnvironmentVariable("SHARPKAPPA_OAUTH");
            string botUsername = "sharpkappa";
            string targetChannel = "xqcow";

            if(args.Length > 0) { targetChannel = args[0]; }
            var skbot = new sharpkappaBot(botUsername, oauth);
            skbot.start(targetChannel).SafeFireAndForget();
            await Task.Delay(-1);
        }
    }
}
