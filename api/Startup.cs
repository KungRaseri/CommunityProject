using System;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Api.WebSockets;
using Api.WebSockets.Handlers;
using Data.Helpers;
using Data.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.Configure<Settings>(Configuration);

            var configSettings = Configuration.GetSection("Settings");
            var settingsCollection = new CouchDbStore<Settings>(configSettings.GetSection("CouchDbUri").Value);

            var settings = settingsCollection.GetAsync().GetAwaiter().GetResult().FirstOrDefault()?.Value;

            services.AddApiVersioning(api =>
            {
                api.DefaultApiVersion = new ApiVersion(1, 0);
                api.AssumeDefaultVersionWhenUnspecified = true;
                api.ReportApiVersions = true;
            });

            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = "JwtBearer";
                    options.DefaultChallengeScheme = "JwtBearer";
                })
                .AddJwtBearer("JwtBearer", jwtBearerOptions =>
                {
                    jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings?.Keys.JWTSecurityKey)),

                        ValidateIssuer = true,
                        ValidIssuer = "kungraseri-api",

                        ValidateAudience = true,
                        ValidAudience = "kungraseri-audience",

                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.FromMinutes(5)
                    };
                });

            services.Configure<IISOptions>("api.kungraseri.ninja", options => { });
            services.AddMvc();

            services.AddWebSocketManager();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseCors(cors =>
                {
                    cors
                    .WithOrigins("http://localhost:8080")
                    .AllowAnyHeader();
                });
            }

            app.UseAuthentication();

            app.UseMvc();
            app.UseWebSockets();

            app.MapWebSocketManager("/botcommandrelay", serviceProvider.GetService<BotCommandRelayHandler>());

            //app.Use(async (context, next) =>
            //{
            //    if (context.Request.Path == "/ws")
            //    {
            //        if (context.WebSockets.IsWebSocketRequest)
            //        {
            //            var webSocket = await context.WebSockets.AcceptWebSocketAsync();
            //            await Echo(context, webSocket);
            //        }
            //        else
            //        {
            //            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            //        }
            //    }
            //    else
            //    {
            //        await next();
            //    }
            //});
        }

        private async Task Echo(HttpContext context, WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            while (!result.CloseStatus.HasValue)
            {
                await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);

                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }
            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }
    }
}
