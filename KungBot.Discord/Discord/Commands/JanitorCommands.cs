﻿using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace KungBot.Discord.Discord.Commands
{
    public class JanitorCommands
    {
        [Command("clean")]
        public async Task CleanChannelCommand(CommandContext cmdContext)
        {
            var messagesResponse = await cmdContext.Channel.GetMessagesAsync(limit: 50);
            var messages = messagesResponse.Where(dm => (dm.Author.IsBot && dm.Author.Username.Equals("KungRaseriBot")) || dm.Content.StartsWith(".")).ToList();

            await cmdContext.TriggerTypingAsync();

            await cmdContext.Channel.DeleteMessagesAsync(messages);
            var botMessage = await cmdContext.RespondAsync($"Deleted {messages.Count} of my message(s)! So fresh and so clean... clean... \n**💣 this message will self-destruct in 3... 2... 1... 💣💣💣💣💣**");

            await Task.Delay(TimeSpan.FromSeconds(3));

            await cmdContext.Channel.DeleteMessageAsync(botMessage);
        }
    }
}