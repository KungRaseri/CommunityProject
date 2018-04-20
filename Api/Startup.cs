using System;
using System.Linq;
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
using Microsoft.Net.Http.Headers;

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

            services.AddResponseCaching(options =>
            {
                options.UseCaseSensitivePaths = true;
                options.MaximumBodySize = 1024;
            });

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
                    jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey =
                            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings?.Keys.JWTSecurityKey)),

                        ValidateIssuer = true,
                        ValidIssuer = "kungraseri-api",

                        ValidateAudience = true,
                        ValidAudience = "kungraseri-audience",

                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.FromMinutes(5)
                    };
                });

            services.Configure<IISOptions>("api.kungraseri.ninja", options => { });
            services.AddMvc(options =>
            {
                options.CacheProfiles.Add("Default",
                    new CacheProfile
                    {
                        Duration = 60
                    });
                options.CacheProfiles.Add("Never",
                    new CacheProfile
                    {
                        Location = ResponseCacheLocation.None,
                        NoStore = true
                    });
            });

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
            app.UseResponseCaching();
            app.UseMvc();
            app.UseWebSockets();

            app.Use(async (context, next) =>
            {
                context.Response.GetTypedHeaders().CacheControl = new CacheControlHeaderValue
                {
                    Public = true,
                    MaxAge = TimeSpan.FromSeconds(60)
                };
                context.Response.Headers[HeaderNames.Vary] = new[] { "Accept-Encoding" };

                await next();
            });

            app.MapWebSocketManager("/botcommandrelay", serviceProvider.GetService<BotCommandRelayHandler>());
        }
    }
}