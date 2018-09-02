using System.Collections.Generic;
using System.Linq;
using Data;
using Data.Enumerations;
using Data.Models;
using ThirdParty;
using TwitchLib.Client;
using TwitchLib.Client.Models;

namespace KungBot.Twitch.Commands
{
    public class UptimeCommand : ICommand
    {
        public string GetKey()
        {
            return "Uptime";
        }

        public async void Perform(TwitchClient client, TwitchService service, ChatCommand chatCommand, Command command)
        {
            if (!command.IsActive)
                return;

            var channel = chatCommand.ArgumentsAsList.FirstOrDefault() ?? "KungRaseri";
            var uptime = await service.GetUpTimeByChannel(await service.GetChannelIdFromChannelName(channel));
            if (uptime.HasValue)
            {
                var uptimePhrase = $"{(uptime.Value.Hours > 0 ? uptime.Value.Hours + " hours " : " ")} {(uptime.Value.Minutes > 0 ? uptime.Value.Minutes + " minutes " : " ")}";
                var message = $"{channel} has been live for {uptimePhrase}";

                client.SendMessage(chatCommand.ChatMessage.Channel, message);
            }
            else
            {
                client.SendMessage(chatCommand.ChatMessage.Channel, $"{channel} is offline.");
            }
        }
    }
}
