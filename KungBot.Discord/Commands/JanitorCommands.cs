using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace KungBot.Discord.Commands
{
    public class JanitorCommands
    {
        [Command("clean")]
        public async Task CleanChannelCommand(CommandContext cmdContext, string author = "KungBot")
        {
            var messagesResponse = await cmdContext.Channel.GetMessagesAsync(limit: 50);
            var messages = messagesResponse.Where(dm => dm.Author.IsBot && dm.Author.Username.Equals(author)).ToList();

            await cmdContext.TriggerTypingAsync();

            await cmdContext.Channel.DeleteMessagesAsync(messages);
            var botMessage = await cmdContext.RespondAsync($"Deleted {messages.Count} of my message(s)! So fresh and so clean... clean... \n**💣 this message will self-destruct in 3... 2... 1... 💣💣💣💣💣**");

            await Task.Delay(TimeSpan.FromSeconds(3));

            await cmdContext.Channel.DeleteMessageAsync(botMessage);
            await cmdContext.Channel.DeleteMessageAsync(cmdContext.Message);
        }

        [Command("cleanallbot")]
        public async Task CleanAllChannelCommand(CommandContext cmdContext, int limit = 100)
        {
            var messagesResponse = await cmdContext.Channel.GetMessagesAsync(limit);
            var messages = messagesResponse.Where(dm => dm.Author.IsBot).ToList();

            await cmdContext.TriggerTypingAsync();

            await cmdContext.Channel.DeleteMessagesAsync(messages);
            var botMessage = await cmdContext.RespondAsync($"Deleted {messages.Count} of my message(s)! So fresh and so clean... clean... \n**💣 this message will self-destruct in 3... 2... 1... 💣💣💣💣💣**");

            await Task.Delay(TimeSpan.FromSeconds(3));

            await cmdContext.Channel.DeleteMessageAsync(botMessage);
            await cmdContext.Channel.DeleteMessageAsync(cmdContext.Message);
        }

        [Command("cleanallbotmentions")]
        public async Task CleanAllBotMentionsCommand(CommandContext cmdContext, string username = "KungBot", int limit = 100)
        {
            var messagesResponse = await cmdContext.Channel.GetMessagesAsync(limit);
            var messages = messagesResponse.Where(dm => (dm.Author.IsBot) || dm.MentionedUsers.Any(mu => mu.Username.Contains(username))).ToList();

            await cmdContext.TriggerTypingAsync();

            await cmdContext.Channel.DeleteMessagesAsync(messages);
            var botMessage = await cmdContext.RespondAsync($"Deleted {messages.Count} of my message(s)! So fresh and so clean... clean... \n**💣 this message will self-destruct in 3... 2... 1... 💣💣💣💣💣**");

            await Task.Delay(TimeSpan.FromSeconds(3));

            await cmdContext.Channel.DeleteMessageAsync(botMessage);
            await cmdContext.Channel.DeleteMessageAsync(cmdContext.Message);
        }

    }
}
