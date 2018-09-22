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
            MainClasses.Bot.Shutdown = true;
        }

        [Command("ban"), RequirePrefixes("!"), RequireUserPermissions(DSharpPlus.Permissions.Administrator)]
        public async Task Ban(CommandContext ctx, DiscordMember Member, [RemainingText] string Reason = null)
            => await Member.BanAsync(reason: Reason);

        [Command("kick"), RequirePrefixes("!"), RequireUserPermissions(DSharpPlus.Permissions.Administrator)]
        public async Task Kick(CommandContext ctx, DiscordMember Member, [RemainingText] string Reason = null)
            => await Member.RemoveAsync(Reason); 
    }
}
