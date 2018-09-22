using DSharpPlus;
using DSharpPlus.EventArgs;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using System.Threading.Tasks;
using System;
using System.Threading;

namespace HC_DBot.MainClasses
{
    class Bot : IDisposable
    {
        private DiscordClient Client { get; }
        private CommandsNextExtension CNext;
        private InteractivityExtension INext;
        public static bool Shutdown = false;

        public Bot(string Token)
        {
            var cfg = new DiscordConfiguration
            {
                Token = Token,                
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                LogLevel = LogLevel.Debug,
                UseInternalLogHandler = true
            };
            Client = new DiscordClient(cfg);
            CNext = this.Client.UseCommandsNext(new CommandsNextConfiguration {
                StringPrefixes = new string[] { "$", "!" },
                EnableDefaultHelp = false
            });
            CNext.RegisterCommands<Commands.UserCommands>();
            CNext.RegisterCommands<Commands.AdminCommands>();
            INext = this.Client.UseInteractivity(new InteractivityConfiguration { });
        }

        public void Dispose()
        {
            Client.Dispose();
            INext = null;
            CNext = null;
            Environment.Exit(0);
        }

        public async Task RunAsync()
        {
            await Client.ConnectAsync();
            while (!Shutdown)
            {
                await Task.Delay(25);
            }
            await Client.DisconnectAsync();
            await Task.Delay(2500);
            Dispose();
        }
    }
}
