using DSharpPlus;
using DSharpPlus.EventArgs;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using System.Threading.Tasks;
using System;
using static HC_DBot.GuildStatics;
using System.Threading;
using DSharpPlus.Entities;

namespace HC_DBot.MainClasses
{
    class Bot : IDisposable
    {
        private DiscordClient Client { get; }
        private CommandsNextExtension CNext;
        private InteractivityExtension INext;

        public Bot(string Token)
        {
            ShutdownRequest = new CancellationTokenSource();
            var cfg = new DiscordConfiguration
            {
                Token = Token,                
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                LogLevel = LogLevel.Debug,
                UseInternalLogHandler = true
            };
            Client = new DiscordClient(cfg);
            //Client.GuildMemberAdded += JoinMSG;
            CNext = this.Client.UseCommandsNext(new CommandsNextConfiguration {
                StringPrefixes = new string[] { "$", "!" },
                EnableDefaultHelp = false
            });
            CNext.RegisterCommands<Commands.UserCommands>();
            CNext.RegisterCommands<Commands.AdminCommands>();
            INext = this.Client.UseInteractivity(new InteractivityConfiguration { });
        }

        public static bool WelcomeMsg = false; //I dont like this...
        public static async Task JoinMSG(GuildMemberAddEventArgs e)
        {
            await e.Guild.GetChannel(hcDeChannelId).SendMessageAsync($"Herzlich willkommen {e.Member.Mention}\n" +
                $"Du bist auf dem Entwickler Discord von {e.Member.Username} gelandet :smile: \n\n" +
                $"Schau bitte in {e.Guild.GetChannel(hcBotLogChannelId).Mention} für weiter Informationen, wie du mithelfen kannst.\n" +
                $"Um die Regeln ({e.Guild.GetChannel(hcBotRegelChannelId).Mention}) zu akzeptieren, schreibe bitte `$accept-rules` in einen Channel deiner Wahl um die Rolle _{e.Guild.GetRole(hcMemberGroupId).Name}_ zu bekommen.");
            await e.Guild.GetChannel(hcEnChannelId).SendMessageAsync($"Welcome {e.Member.Mention} on the developer discord by {e.Guild.Name} {DiscordEmoji.FromGuildEmote(e.Client, hcEmote)}");
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
            while (!ShutdownRequest.IsCancellationRequested)
            {
                await Task.Delay(25);
            }
            await Client.DisconnectAsync();
            await Task.Delay(2500);
            Dispose();
        }
    }
}
