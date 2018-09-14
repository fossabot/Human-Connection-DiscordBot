using Discord;
using Discord.Addons;
using Discord.Addons.PrefixService;
using Discord.Commands;
using HumanConnection.DiscordBot.Services;
using Raven.Client.Documents;
using System;
using System.Threading.Tasks;

namespace HumanConnection.DiscordBot.Modules
{
    class ServerConfigModule : ModuleBase<ICommandContext>
    {
        private readonly ServerConfigService _serverConfigService;
        private static string _prefix;
        private IDocumentStore _store;

        public ServerConfigModule(ServerConfigService service, string prefix)
        {
            _serverConfigService = service;
            _prefix = prefix;
        }
        
        [Command("server updatename", RunMode = RunMode.Async)]
        [Name("Servername Update")]
        [Summary("Update the name of the server.")]
        public async Task UpdateServerName([Remainder]string name)
        {
            await _serverConfigService.UpdateServerName(Context, name);
        }

        [Command("server updatewidget", RunMode = RunMode.Async)]
        [Name("Serverwidget Update")]
        [Summary("Update the config of the server widget")]
        public async Task Widget(string channel, string enabled)
        {
            await _serverConfigService.Widget(Context, Convert.ToBoolean(enabled));
        }
        /*
        [Command("server updatename", RunMode = RunMode.Async)]
        [Name("Servername Update")]
        [Summary("Joins the user's voice channel.")]
        public async Task UpdateServerName([Remainder]string name)
        {
            await _serverConfigService.UpdateServerName(Context, name);
        }*/
    }
}
