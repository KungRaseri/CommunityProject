using System.Linq;
using Data.Helpers;
using Data.Models;
using ThirdParty;
using TwitchLib.Client;
using TwitchLib.Client.Models;

namespace KungBot.Twitch.Commands
{
    public class RankCommand : ICommand
    {
        public string GetKey()
        {
            return "Rank";
        }

        public void Perform(TwitchClient client, TwitchService service, ChatCommand chatCommand, Command command)
        {
            if (!command.IsActive)
                return;

            //var _viewerCollection = new CouchDbStore<Viewer>(ApplicationSettings.CouchDbUrl);
            //var _viewerRankCollection = new CouchDbStore<ViewerRank>(ApplicationSettings.CouchDbUrl);

            //var dbViewer = (_viewerCollection.GetAsync("viewer-username", chatCommand.ChatMessage.Username).GetAwaiter().GetResult()).FirstOrDefault()?.Value;
            //var viewRanks = _viewerRankCollection.GetAsync().GetAwaiter().GetResult();
            //if (dbViewer != null)
            //{
            //    var viewerRank = viewRanks
            //        .LastOrDefault(r => r.Value.ExperienceRequired <= dbViewer.Points)?.Value.RankName;

            //    client.SendMessage(chatCommand.ChatMessage.Channel, $"{chatCommand.ChatMessage.Username}, Your rank is {viewerRank}! You have {dbViewer.Points} experience! kungraHYPERS");
            //}
        }
    }
}
