using System;
using System.Collections.Generic;
using System.Text;
using TwitchLib;
using TwitchLib.Client;
using TwitchLib.Client.Models;

namespace ThirdParty
{
    public class TwitchBotService
    {
        public readonly TwitchClient Client;

        public TwitchBotService(ConnectionCredentials credentials)
        {
            Client = new TwitchClient { };
            Client.SetConnectionCredentials(credentials);
        }

        public void Connect()
        {
            Client.Connect();
        }

        public void Reconnect()
        {
            Client.Reconnect();
        }

        public void Disconnect()
        {
            Client.Disconnect();
        }

        public void JoinChannel(string channelName)
        {
            Client.JoinChannel(channelName);
        }

        public void LeaveChannel(string channelName)
        {
            Client.LeaveChannel(channelName);
        }

        public void GetJoinedChannel(string channelName)
        {
            Client.GetJoinedChannel(channelName);
        }
    }
}
