using System.Collections.Generic;
using System.Linq;
using Data;
using Data.Helpers;
using Data.Models;
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
        private CouchDbStore<Viewer> _viewerCollection;

        public void Perform(TwitchClient client, TwitchService service, ChatCommand chatCommand, Command command)
        {
            if (!IsActive)
                return;

            _viewerCollection = new CouchDbStore<Viewer>(Settings.CouchDbUrl);

            var dbViewer = (_viewerCollection.GetAsync("viewer-username", chatCommand.ChatMessage.Username).GetAwaiter().GetResult()).FirstOrDefault().Value;

            client.SendMessage(chatCommand.ChatMessage.Channel, $"{chatCommand.ChatMessage.Username}, You have {dbViewer.Experience} experience! kungraHYPERS");
        }
    }
}
