using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Api.WebSockets.Handlers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Api.Controllers
{
    [Route("ws/api/[controller]")]
    public class BotCommandRelayController : BaseApiController
    {
        private readonly BotCommandRelayHandler _botCommandRelayHandler;

        public BotCommandRelayController(IConfiguration configuration, BotCommandRelayHandler botCommandRelayHandler) : base(configuration)
        {
            _botCommandRelayHandler = botCommandRelayHandler;
        }

        [HttpGet]
        public async Task SendMessage(string command, string message)
        {
            switch (command)
            {
                case "message":
                    await _botCommandRelayHandler.SendMessageToAllAsync(message);
                    break;
                case "timeout":
                    await _botCommandRelayHandler.SendMessageToAllAsync(message);
                    break;
            }
        }
    }
}