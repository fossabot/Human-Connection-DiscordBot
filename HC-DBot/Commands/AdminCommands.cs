using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static HC_DBot.GuildStatics;

namespace HC_DBot.Commands
{
    class AdminCommands : BaseCommandModule
    {
        [Command("shutdown"), RequirePrefixes("!"), RequireUserPermissions(DSharpPlus.Permissions.Administrator)]
        public async Task BotShutdown(CommandContext ctx)
        {
            await ctx.Guild.GetChannel(hcBotLogChannelId).SendMessageAsync($"Shuting down {DiscordEmoji.FromGuildEmote(ctx.Client, botEmote)}");
            /*Not a good way to "shutdown" the bot but i haven't figured out
            how services work yet so i cant use the CancelationToken.
            There may, be other workarounds but I want to use the intended way*/
            Environment.Exit(0);
        }

        [Command("ban"), RequirePrefixes("!"), RequireUserPermissions(DSharpPlus.Permissions.Administrator)]
        public async Task Ban(CommandContext ctx, DiscordMember Member, [RemainingText] string Reason = null)
            => await Member.BanAsync(reason: Reason);

        [Command("kick"), RequirePrefixes("!"), RequireUserPermissions(DSharpPlus.Permissions.Administrator)]
        public async Task Kick(CommandContext ctx, DiscordMember Member, [RemainingText] string Reason = null)
            => await Member.RemoveAsync(Reason); 
    }
}
