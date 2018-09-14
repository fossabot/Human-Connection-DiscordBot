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
            await Context.Guild.ModifyAsync(async x =>
            {
                x.Name = newName;
            });
            await Log(new LogMessage(LogSeverity.Info, "UpdateServerName", "Name is now " + newName));
        }
        #endregion

        #region AFK Update
        public async Task UpdateAfk (ICommandContext Context, IVoiceChannel channel, int timeout)
        {
            await Context.Guild.ModifyAsync(async x =>
            {
                x.AfkChannelId = channel.Id;
                x.AfkTimeout = timeout;
            });
        }
        #endregion

        #region Server Verify Prepare
        public async Task PrepareServerVerify(ICommandContext Context)
        {
            await Context.Guild.ModifyAsync(async x =>
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
            await Context.Guild.ModifyEmbedAsync(async x =>
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
