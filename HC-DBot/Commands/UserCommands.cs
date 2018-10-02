using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using MySql.Data.MySqlClient;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using static HC_DBot.MainClasses.Bot;

namespace HC_DBot.Commands
{
    class UserCommands : BaseCommandModule
    {
        public async Task LogAction(DiscordGuild guild, DiscordMessage msg, string functionName, string description, string message, DiscordColor color)
        {

            DiscordChannel channel = guild.GetChannel(GuildsList[guild.Id].ChannelConfig.LogChannelID);

            WebRequest request = WebRequest.Create($"https://png2.kisspng.com/sh/ae7a514d72b233a0ccf5aff823ba701f/L0KzQYm3VMAzN5J0fZH0aYP2gLBuTfcue6ZujNc2Z3Byd73sTgN6e6VqhZ9qZH3sfrr6lQJifJD3ReV4ZoT6ccPsTfRmeJ10RdNtbXnxecT7kvF1d6MyTdNsMnHlRIjoWcZmPmozTKo7N0K7QYK4VcIzP2E8Sqk6Nkm3PsH1h5==/kisspng-g-suite-google-system-administrator-software-deplo-administrator-5ac2ab47a96e69.482728111522707271694.png");
            WebResponse response = await request.GetResponseAsync();
            Stream dataStream = response.GetResponseStream();

            // Init builder
            DiscordEmbedBuilder builder = new DiscordEmbedBuilder();
            builder.WithColor(color);
            // Build author
            builder.WithAuthor($"{msg.Author.Username}", null, $"{msg.Author.AvatarUrl}");
            // Build Header
            builder.WithTitle("Changelog");
            builder.WithDescription("Logged user/bot action");
            builder.WithThumbnailUrl("attachment://logthumbnail.png");
            // Build content
            builder.AddField(name: "Function", value: $"{functionName}");
            builder.AddField(name: "Description", value: $"{description}");
            builder.AddField(name: "Message", value: $"{message}");
            // Build footer
            builder.WithFooter("Copyright 2018 Lala Sabathil");
            builder.WithTimestamp(msg.CreationTimestamp);

            await channel.SendFileAsync(fileName: "logthumbnail.png", fileData: dataStream, content: null, tts: false, embed: builder.Build());
        }

        [Command("ping"), RequirePrefixes("$")]
        public async Task Ping(CommandContext ctx)
        {
            await ctx.RespondAsync($"{ctx.Member.Mention}, Pong! :3 miau!");
        }

        [Command("accept-rules"), RequirePrefixes("$")]
        public async Task RuleAccept(CommandContext ctx)
        {
            await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":ok_hand::skin-tone-2:"));
            await ctx.Member.GrantRoleAsync(ctx.Guild.GetRole(GuildsList[ctx.Guild.Id].ChannelConfig.RoleID));
            await Task.Delay(2000);
            await ctx.Message.DeleteAsync();
        }

        [Command("help"), RequirePrefixes("$")]
        public async Task Help(CommandContext ctx)
        {
            var invy = ctx.Client.GetInteractivity();
            var builder = new DiscordEmbedBuilder();
            builder.WithTitle("Commands of HC Control - Not all are implemented right now");
            builder.WithThumbnailUrl("https://cdn.pbrd.co/images/HEjzSg5.png");
            builder.WithDescription($"This are the commands for the **HC Control**. The prefix is **$**");
            builder.AddField("$accept-rules", "Accept the rules of this server");
            builder.AddField("$author", "Information about the author");
            builder.AddField("$gpdr", "GPDR of this Server - Not implemented");
            builder.AddField("$help", "This help");
            builder.AddField("$info", "Info about the server - Not implemented");
            builder.AddField("$ping", "Returns a friendly \"Pong\"");
            builder.WithFooter($"©2018 Lala Sabathil");
            builder.WithColor(new DiscordColor(r: 0, g: 0, b: 255));

            if (ctx.Member.Roles.Any(x => x.CheckPermission(Permissions.Administrator) == PermissionLevel.Allowed))
            {
                var adminBuilder = new DiscordEmbedBuilder();
                adminBuilder.WithTitle("Admin commands of HC Control - Not all are implemented right now");
                adminBuilder.WithThumbnailUrl("https://cdn.pbrd.co/images/HEjzSg5.png");
                adminBuilder.WithDescription($"This are the admin commands for the **HC Control**.\nSince you have Administrator Privilege, you get this message.\nAdmin prefix is **!**");
                adminBuilder.AddField("!ban <usermention> <reason>", "Ban the mentioned *user* with *reason*");
                adminBuilder.AddField("!info", "Info about the server for admins");
                adminBuilder.AddField("!kick <usermention> <reason>", "Kick the mentioned *user* with *reason*");
                adminBuilder.AddField("!role-add <usermention> <rolename>", "Add *user* to *role*");
                adminBuilder.AddField("!role-remove <usermention> <rolename>", "Remove *user* from *role*");
                adminBuilder.AddField("!shutdown", "Stop's the bot and exits the application on bot side");
                adminBuilder.AddField("!warn <usermention> <message>", "Warn mentioned *user* with *message*");
                builder.WithFooter($"©2018 Lala Sabathil");
                adminBuilder.WithColor(new DiscordColor(r: 255, g: 20, b: 80));
                await ctx.Message.DeleteAsync("command hide");
                var msg = await ctx.RespondAsync(embed: builder.Build());
                await msg.CreateReactionAsync(DiscordEmoji.FromUnicode("◀"));
                await msg.CreateReactionAsync(DiscordEmoji.FromUnicode("▶"));
                await msg.CreateReactionAsync(DiscordEmoji.FromUnicode("❌"));
                while (true)
                {
                    var Inmsg = await invy.WaitForMessageReactionAsync(msg, ctx.User, TimeSpan.FromMinutes(1));
                    if (Inmsg.Emoji == DiscordEmoji.FromUnicode("◀") && msg.Embeds.First().Title.ToLower().StartsWith("admin"))
                    {
                        await msg.DeleteReactionAsync(DiscordEmoji.FromUnicode("◀"), ctx.User);
                    }
                    else if (Inmsg.Emoji == DiscordEmoji.FromUnicode("◀") && !msg.Embeds.First().Title.ToLower().StartsWith("admin"))
                    {
                        await msg.ModifyAsync(embed: builder.Build());
                        await msg.DeleteReactionAsync(DiscordEmoji.FromUnicode("◀"), ctx.User);
                    }
                    else if (Inmsg.Emoji == DiscordEmoji.FromUnicode("▶") && msg.Embeds.First().Title.ToLower().StartsWith("commands"))
                    {
                        await msg.ModifyAsync(embed: adminBuilder.Build());
                        await msg.DeleteReactionAsync(DiscordEmoji.FromUnicode("▶"), ctx.User);
                    }
                    else if (Inmsg.Emoji == DiscordEmoji.FromUnicode("▶") && msg.Embeds.First().Title.ToLower().StartsWith("commands"))
                    {
                        await msg.DeleteReactionAsync(DiscordEmoji.FromUnicode("▶"), ctx.User);
                    }
                    else if (Inmsg.Emoji == DiscordEmoji.FromUnicode("❌"))
                    {
                        await msg.DeleteAllReactionsAsync();
                        await msg.ModifyAsync(embed: builder.Build());
                        return;
                    }
                }
            }
            await ctx.RespondAsync(embed: builder.Build());
        }

        [Command("author"), RequirePrefixes("$")]
        public async Task Author(CommandContext ctx)
        {
            WebRequest request = WebRequest.Create($"https://cdn.pbrd.co/images/HEjzvIZ.png");
            WebResponse response = await request.GetResponseAsync();
            Stream dataStream = response.GetResponseStream();

            var lala = await ctx.Client.GetUserAsync(199858858166976513);
            var builder = new DiscordEmbedBuilder();
            builder.WithTitle("Author contact data of HC Control");
            builder.WithThumbnailUrl("https://cdn.pbrd.co/images/HEjzSg5.png");
            builder.WithImageUrl($"attachment://authorimage.png");
            builder.WithDescription("The author of the HC Control is Lala Sabathil");
            builder.AddField("Discord server", $"[Invite Link](https://discord.gg/heqF6P4)");
            builder.AddField("Discord user", lala.Mention);
            builder.AddField("Facebook", "[Profile Link](https://www.facebook.com/LalaDeviChan)");
            builder.AddField("Twitter", "[Profile Link](https://twitter.com/Lala_devi_chan)");
            builder.AddField("Mail", "admin@latias.eu");
            builder.AddField("Telegram", "[Link](https://telegram.me/Lulalaby)");
            builder.WithUrl("https://www.latias.eu");
            builder.WithColor(new DiscordColor(r: 75, g: 80, b: 255));
            builder.WithFooter($"©2018 Lala Sabathil");
            await ctx.Message.DeleteAsync("command hide");
            await ctx.Message.RespondWithFileAsync(fileName: $"authorimage.png", fileData: dataStream, embed: builder.Build());
        }
    }
}
