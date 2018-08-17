using System.Collections.Generic;
using System.Linq;
using Data;
using Data.Enumerations;
using Data.Helpers;
using Data.Models;
using MoreLinq;
using ThirdParty;
using TwitchLib.Client;
using TwitchLib.Client.Models;

namespace KungBot.Twitch.Commands
{
    public class RankCommand : ICommand
    {
        public string Name { get; set; }
        public string Identifier { get; set; }
        public AuthLevel AuthorizeLevel { get; set; }
        public bool IsActive { get; set; }

        public void Perform(TwitchClient client, TwitchService service, ChatCommand chatCommand, Command command)
        {
            if (!IsActive)
                return;

            var _viewerCollection = new CouchDbStore<Viewer>(ApplicationSettings.CouchDbUrl);
            var _viewerRankCollection = new CouchDbStore<ViewerRank>(ApplicationSettings.CouchDbUrl);

            var dbViewer = (_viewerCollection.GetAsync("viewer-username", chatCommand.ChatMessage.Username).GetAwaiter().GetResult()).FirstOrDefault()?.Value;
            var viewRanks = _viewerRankCollection.GetAsync().GetAwaiter().GetResult();
            if (dbViewer != null)
            {
                var viewerRank = viewRanks
                    .LastOrDefault(r => r.Value.ExperienceRequired <= dbViewer.Experience)?.Value.RankName;

                client.SendMessage(chatCommand.ChatMessage.Channel, $"{chatCommand.ChatMessage.Username}, Your rank is {viewerRank}! You have {dbViewer.Experience} experience! kungraHYPERS");
            }
        }
    }
}
