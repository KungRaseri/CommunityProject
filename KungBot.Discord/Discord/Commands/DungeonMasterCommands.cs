using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace KungBot.Discord.Discord.Commands
{
    public class DungeonMasterCommands
    {
        [Command("roll")]
        public async Task GetRollCommand(CommandContext cmdContext, string rollInput)
        {
            var rand = new Random();
            if (!rollInput.Contains('d'))
            {
                await cmdContext.RespondAsync("Missing 'd' from the input. Required Format: ##d##[+##]");
                return;
            }

            var rollSplit = rollInput.Split('d');

            var amountSplit = rollInput.Contains('+') ? rollSplit[1].Split('+') : new[] { rollSplit[1], "0" };

            try
            {
                var quantity = int.Parse(rollSplit[0]);
                var diceAmount = int.Parse(amountSplit[0]);
                var rollAddition = int.Parse(amountSplit[1]);
                var messageStrings = new List<int>();

                for (int i = 0; i < quantity; i++)
                {
                    messageStrings.Add(rand.Next(1, diceAmount));
                }
                var message = $"{cmdContext.User.Mention}: Total: {messageStrings.Sum() + rollAddition} Roll: {string.Join("+", messageStrings)}{(rollAddition > 0 ? '+' + rollAddition.ToString() : "")}";
                if (message.Length > 2000)
                {
                    await cmdContext.RespondAsync("Try a smaller roll.  You're not a God.");
                    return;
                }
                await cmdContext.RespondAsync(message);

            }
            catch (Exception e)
            {
                await cmdContext.RespondAsync($"Something went wrong: {e.Message}\n{e.StackTrace}");
                throw;
            }
        }
    }
}
