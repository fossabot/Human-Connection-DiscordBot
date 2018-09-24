using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static HC_DBot.GuildStatics;
using System.IO;
using Newtonsoft.Json;

namespace HC_DBot.Commands
{
    class AdminCommands : BaseCommandModule
    {
        [Command("shutdown"), RequirePrefixes("!"), RequireUserPermissions(DSharpPlus.Permissions.Administrator)]
        public async Task BotShutdown(CommandContext ctx)
        {
            await ctx.Guild.GetChannel(hcBotLogChannelId).SendMessageAsync($"Shuting down {DiscordEmoji.FromGuildEmote(ctx.Client, botEmote)}");
            await ctx.Message.DeleteAsync();
            ShutdownRequest.Cancel();
        }

        [Command("ban"), RequirePrefixes("!"), RequireUserPermissions(DSharpPlus.Permissions.Administrator)]
        public async Task Ban(CommandContext ctx, DiscordMember Member, [RemainingText] string Reason = null)
        {
            await Member.BanAsync(reason: Reason);
            await ctx.Message.DeleteAsync("Admin command hide");
        }

        [Command("kick"), RequirePrefixes("!"), RequireUserPermissions(DSharpPlus.Permissions.Administrator)]
        public async Task Kick(CommandContext ctx, DiscordMember Member, [RemainingText] string Reason = null)
        {
            await Member.RemoveAsync(Reason);
            await ctx.Message.DeleteAsync("Admin command hide");
        }

        [Command("welcometoggle"), Aliases("wt","welcomemessage"), RequirePrefixes("!"), RequireUserPermissions(DSharpPlus.Permissions.Administrator)]
        public async Task WelcomeMessage(CommandContext ctx)
        {
            if (!Program.config.Modules.Greet)
            {
                await ctx.Guild.GetChannel(hcBotLogChannelId).SendMessageAsync($"Welcome messages have been **enabled!**");
                ctx.Client.GuildMemberAdded += MainClasses.Bot.JoinMSG;
                Program.config.Modules.Greet = true;
                using (StreamWriter file = File.CreateText(@"config.json"))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(file, Program.config);
                }
            }
            else
            {
                await ctx.Guild.GetChannel(hcBotLogChannelId).SendMessageAsync($"Welcome messages have been **disabled!**");
                ctx.Client.GuildMemberAdded -= MainClasses.Bot.JoinMSG;
                Program.config.Modules.Greet = false;
                using (StreamWriter file = File.CreateText(@"config.json"))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(file, Program.config);
                }
            }
        }
    }
}
