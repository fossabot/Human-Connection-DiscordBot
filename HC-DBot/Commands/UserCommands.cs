using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HC_DBot.GuildStatics;

namespace HC_DBot.Commands
{
    class UserCommands : BaseCommandModule
    {
        [Command("ping"), RequirePrefixes("$")]
        public async Task Ping(CommandContext ctx)
        {
            await ctx.RespondAsync($"{ctx.Member.Mention}, Pong! :3");
        }

        [Command("accept-rules"), RequirePrefixes("$")]
        public async Task RuleAccept(CommandContext ctx)
        {
            await ctx.Message.CreateReactionAsync(DiscordEmoji.FromGuildEmote(ctx.Client, hcEmote));
            await ctx.Member.GrantRoleAsync(ctx.Guild.GetRole(hcMemberGroupId));
            await ctx.Message.DeleteAsync();
        }

        [Command("help"), RequirePrefixes("$")]
        public async Task Help(CommandContext ctx)
        {
            var builder = new DiscordEmbedBuilder();
            builder.WithTitle("Commands of HC Control");
            builder.WithThumbnailUrl("https://cdn.pbrd.co/images/HEjzSg5.png");
            builder.WithImageUrl("https://cdn.pbrd.co/images/HEjzvIZ.png");
            builder.WithDescription($"This are the commands for the **HC Control**. The prefix is **$**");
            builder.AddField("$accept-rules", "Accept the rules of this server");
            builder.AddField("$author", "Information about the author");
            builder.AddField("$gpdr", "GPDR of this Server - Not implemented");
            builder.AddField("$help", "This help");
            builder.AddField("$info", "Info about the server - Not implemented");
            builder.AddField("$ping", "Returns a friendly \"Pong\"");
            builder.WithFooter($"©2018 Lala Sabathil");
            builder.WithColor(new DiscordColor(r: 0, g: 0, b:255));
            await ctx.RespondAsync(embed: builder.Build());

            if (ctx.Member.Roles.Any(x => x.Permissions == DSharpPlus.Permissions.Administrator))
            {
                var adminBuilder = new DiscordEmbedBuilder();
                adminBuilder.WithAuthor($"©2018 Lala Sabathil | {this.GetType().ToString()}");
                adminBuilder.WithTitle("Admin commands of HC Control - Not implemented right now");
                adminBuilder.WithThumbnailUrl("https://cdn.pbrd.co/images/HEjzSg5.png");
                adminBuilder.WithImageUrl("https://cdn.pbrd.co/images/HEjzvIZ.png");
                adminBuilder.WithDescription($"This are the admin commands for the **HC Control**.\nSince you have Administrator Privilege, you get this message.\nAdmin prefix is **!**");
                adminBuilder.AddField("!ban <usermention> <reason>", "Ban the mentioned *user* with *reason*");
                adminBuilder.AddField("!info", "Info about the server for admins");
                adminBuilder.AddField("!kick <usermention> <reason>", "Kick the mentioned *user* with *reason*");
                adminBuilder.AddField("!role-add <usermention> <rolename>", "Add *user* to *role*");
                adminBuilder.AddField("!role-remove <usermention> <rolename>", "Remove *user* from *role*");
                adminBuilder.AddField("!shutdown", "Stop's the bot and exits the application on bot side");
                adminBuilder.AddField("!warn <usermention> <message>", "Warn mentioned *user* with *message*");
                adminBuilder.WithFooter("HC Bot", "https://github.com/Lulalaby/Human-Connection-DiscordBot/");
                adminBuilder.WithColor(new DiscordColor(r: 255, g: 20, b: 80));
                await ctx.Member.SendMessageAsync(embed: adminBuilder.Build());
            }
        }

        [Command("author"), RequirePrefixes("$")]
        public async Task Author(CommandContext ctx)
        {
            var lala = await ctx.Guild.GetMemberAsync(199858858166976513);
            var getInvites = await ctx.Guild.GetInvitesAsync();
            var Invite = getInvites.First(x => x.Code == "THZue3w");
            var builder = new DiscordEmbedBuilder();
            builder.WithTitle("Author contact data of HC Control");
            builder.WithThumbnailUrl("https://cdn.pbrd.co/images/HEjzSg5.png");
            builder.WithImageUrl("https://cdn.pbrd.co/images/HEjzvIZ.png");
            builder.WithDescription("The author of the HC Control is Lala Sabathil");
            builder.AddField("Discord server", $"https://discord.gg/{Invite.Code}");
            builder.AddField("Discord user", lala.Mention);
            builder.AddField("Facebook", "https://www.facebook.com/LalaDeviChan");
            builder.AddField("Twitter", "https://twitter.com/Lala_devi_chan");
            builder.AddField("Mail", "admin@latias.eu");
            builder.AddField("Telegram", "https://telegram.me/Lulalaby");
            builder.WithUrl("https://www.latias.eu");
            builder.WithColor(new DiscordColor(r: 75, g: 80, b: 255));
            builder.WithFooter($"©2018 Lala Sabathil");
        }
    }
}
