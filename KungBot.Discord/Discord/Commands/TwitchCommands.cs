using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data;
using Data.Helpers;
using Data.Models;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using TwitchLib.Api;
using TwitchLib.Api.Models.v5.Clips;

namespace KungBot.Discord.Discord.Commands
{
    public class TwitchCommands
    {
        private readonly Settings _settings;

        public TwitchCommands()
        {
            var settingsCollection = new CouchDbStore<Settings>(Settings.CouchDbUrl);
            _settings = settingsCollection.GetAsync().Result.FirstOrDefault()?.Value;
        }

        [Command("followage")]
        public async Task GetChannelFollowingCommand(CommandContext cmdContext, string channelName)
        {
            var api = new TwitchAPI();
            await api.Settings.SetClientIdAsync(_settings.Keys.Twitch.ClientId);

            try
            {
                var searchResults = await api.Search.v5.SearchChannelsAsync(channelName);

                searchResults.Channels.ToList().ForEach(c =>
                {
                    Console.WriteLine($"{c.Name}: {c.Name == channelName}");
                });

                var channel = searchResults.Channels.FirstOrDefault(c => c.Name == channelName);
                if (channel == null)
                {
                    await cmdContext.RespondAsync($"```That channel doesn't exist. :mehdiFEELS:```");
                    return;
                }
                var followersResponse = await api.Channels.v5.GetChannelFollowersAsync(channel.Id);

                await cmdContext.RespondAsync($"```{followersResponse.Total} people follow {channelName}.```");
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }
        }

        /// <summary>
        /// Get top amount of live channels.
        /// </summary>
        /// <param name="cmdContext">Discord command context</param>
        /// <param name="amount">How many channels you want returned</param>
        /// <returns></returns>
        [Command("toplive")]
        public async Task GetTopLiveChannelsCommand(CommandContext cmdContext, int amount)
        {
            var api = new TwitchAPI();
            await api.Settings.SetClientIdAsync(_settings.Keys.Twitch.ClientId);
            try
            {
                var liveStreamsResponse = await api.Streams.v5.GetLiveStreamsAsync(limit: amount);

                var liveStreams = liveStreamsResponse.Streams.OrderByDescending(s => s.Viewers).ToList();
                var streams = new List<string> { "**Live Streams**", "```" };

                streams.AddRange(liveStreams.Select(stream => $"{stream.Channel.DisplayName} is live with {stream.Viewers} viewers. Playing: {stream.Game}\n"));
                streams.Add("```");
                var message = string.Join(" ", streams.ToList());

                if (message.Length > 2000)
                {
                    await cmdContext.RespondAsync($"Message is too long, get real son.");
                    return;
                }

                await cmdContext.RespondAsync($"{message}");
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }
        }

        [Command("topclips")]
        public async Task GetTopClipsCommand(CommandContext cmdContext, [Description("The amount of clips you want returned.")]int amount, [Description("The channel that you would like the top clips from")]string channelName = null)
        {
            var api = new TwitchAPI();
            await api.Settings.SetClientIdAsync(_settings.Keys.Twitch.ClientId);

            try
            {
                var clipsResponse = await api.Clips.v5.GetTopClipsAsync(channelName, limit: amount, period: Period.All);
                clipsResponse.Clips.ForEach(async clip =>
                {
                    await cmdContext.RespondAsync(clip.Url);
                });

            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }
        }
    }
}