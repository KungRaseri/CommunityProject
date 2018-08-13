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
        public async Task CleanChannelCommand(CommandContext cmdContext)
        {
            var messagesResponse = await cmdContext.Channel.GetMessagesAsync(limit: 50);
            var messages = messagesResponse.Where(dm => (dm.Author.IsBot && dm.Author.Username.Equals("KungBot")) || dm.Content.StartsWith(KungBot.CommandCharacter)).ToList();

            await cmdContext.TriggerTypingAsync();

            await cmdContext.Channel.DeleteMessagesAsync(messages);
            var botMessage = await cmdContext.RespondAsync($"Deleted {messages.Count} of my message(s)! So fresh and so clean... clean... \n**💣 this message will self-destruct in 3... 2... 1... 💣💣💣💣💣**");

            await Task.Delay(TimeSpan.FromSeconds(3));

            await cmdContext.Channel.DeleteMessageAsync(botMessage);
        }

        [Command("cleanallbot")]
        public async Task CleanAllChannelCommand(CommandContext cmdContext, int limit = 100)
        {
            var messagesResponse = await cmdContext.Channel.GetMessagesAsync(limit);
            var messages = messagesResponse.Where(dm => (dm.Author.IsBot) || dm.Content.StartsWith(KungBot.CommandCharacter)).ToList();

            await cmdContext.TriggerTypingAsync();

            await cmdContext.Channel.DeleteMessagesAsync(messages);
            var botMessage = await cmdContext.RespondAsync($"Deleted {messages.Count} of my message(s)! So fresh and so clean... clean... \n**💣 this message will self-destruct in 3... 2... 1... 💣💣💣💣💣**");

            await Task.Delay(TimeSpan.FromSeconds(3));

            await cmdContext.Channel.DeleteMessageAsync(botMessage);
        }

        [Command("cleanallbotmentions")]
        public async Task CleanAllBotMentionsCommand(CommandContext cmdContext, int limit = 100)
        {
            var messagesResponse = await cmdContext.Channel.GetMessagesAsync(limit);
            var messages = messagesResponse.Where(dm => (dm.Author.IsBot) || dm.Content.StartsWith(KungBot.CommandCharacter) || dm.MentionedUsers.Any(mu => mu.Username.Contains("KungBot"))).ToList();

            await cmdContext.TriggerTypingAsync();

            await cmdContext.Channel.DeleteMessagesAsync(messages);
            var botMessage = await cmdContext.RespondAsync($"Deleted {messages.Count} of my message(s)! So fresh and so clean... clean... \n**💣 this message will self-destruct in 3... 2... 1... 💣💣💣💣💣**");

            await Task.Delay(TimeSpan.FromSeconds(3));

            await cmdContext.Channel.DeleteMessageAsync(botMessage);
        }

    }
}
