using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AsyncAwaitBestPractices;
using System.Linq;

namespace sharpkappa {
    class EmoteManager {
        private HashSet<Emote> emoteList;
        private List<string> emoteStringList;
        private string channel;
        private string channel_id;

        public EmoteManager(string channel, string channel_id) {
            emoteList = new HashSet<Emote>();
            this.channel = channel;
            this.channel_id = channel_id;
        }

        public async Task start() {
            await refreshEmoteList(true);
            refreshEmoteList().SafeFireAndForget();
        }

        public async Task fetchAllEmoteData() {
            emoteList.UnionWith(await TwitchAPI.getChannelEmotes());
            emoteList.UnionWith(await TwitchAPI.getChannelEmotes(channel_id));
            emoteList.UnionWith(await FFZAPI.getGlobalEmotes());
            emoteList.UnionWith(await FFZAPI.getChannelEmotes(channel));
            emoteList.UnionWith(await BttvAPI.getGlobalEmotes());
            emoteList.UnionWith(await BttvAPI.getChannelEmotes(channel_id));
            emoteStringList = new List<string>();
            foreach(Emote emote in emoteList) {
                emoteStringList.Add(emote.name);
            }
        }

        public async Task refreshEmoteList(bool prefetch = false) {
            if(prefetch) {
                await fetchAllEmoteData();
            }
            while(true && !prefetch) {
                await Task.Delay(TimeSpan.FromMinutes(30));
                await fetchAllEmoteData();
            }
        }

        public HashSet<Emote> getEmoteList() {
            return emoteList;
        }

        public List<string> getEmoteStringList() {
            return emoteStringList;
        }
    }
}