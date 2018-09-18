using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace HumanConnection.DiscordBot.Services
{
    class ServerConfigService
    {
        #region Server Name Update
        public async Task UpdateServerName(ICommandContext Context, string newName)
        {
            await Log(new LogMessage(LogSeverity.Info, "UpdateServerName", "Updating " + Context.Guild.Name + " to " + newName));
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
            await Context.Guild.ModifyAsync(async x => 
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
            {
                x.Name = newName;
            });
            await Log(new LogMessage(LogSeverity.Info, "UpdateServerName", "Name is now " + newName));
        }
        #endregion

        #region AFK Update
        public async Task UpdateAfk (ICommandContext Context, IVoiceChannel channel, int timeout)
        {
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
            await Context.Guild.ModifyAsync(async x =>
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
            {
                x.AfkChannelId = channel.Id;
                x.AfkTimeout = timeout;
            });
        }
        #endregion

        #region Server Verify Prepare
        public async Task PrepareServerVerify(ICommandContext Context)
        {
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
            await Context.Guild.ModifyAsync(async x =>
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
            {
                x.DefaultMessageNotifications = DefaultMessageNotifications.MentionsOnly;
                x.VerificationLevel = VerificationLevel.High;
            });
        }
        #endregion

        #region Set up widget
        public async Task Widget(ICommandContext Context, bool enabled)
        {
            var mentionChannel = Context.Message.MentionedChannelIds.GetEnumerator();
            var mentionChannelObj = mentionChannel.Current;
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
            await Context.Guild.ModifyEmbedAsync(async x =>
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
            {
                x.ChannelId = mentionChannelObj;
                x.Enabled = enabled;
            });
        }
        #endregion

        private async Task Log(LogMessage msg)
        {
            Program.BOT_UI.AppendConsole(msg.Message);
            await Task.Delay(0);
        }
    }
}
