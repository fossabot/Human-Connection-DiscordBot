using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Threading.Tasks;
using static HC_DBot.MainClasses.Bot;
using MySql.Data.MySqlClient;
using System;
using System.Net;
using System.IO;

namespace HC_DBot.Commands
{
    class AdminCommands : BaseCommandModule
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

        public static ulong hcBotLogChannelId = 490977974787768353;
        public static ulong botEmote = 491234510659125271;

        [Command("shutdown"), RequirePrefixes("!"), RequireOwner(), RequireDirectMessage()]
        public async Task BotShutdown(CommandContext ctx)
        {
            await LogAction(ctx.Guild, ctx.Message, "BotShutdown", "Shut the bot down", $"Shutting down {DiscordEmoji.FromGuildEmote(ctx.Client, botEmote)}", DiscordColor.Orange);
            await ctx.Message.DeleteAsync();
            ShutdownRequest.Cancel();
        }

        [Command("ban"), RequirePrefixes("!"), RequireUserPermissions(DSharpPlus.Permissions.BanMembers), RequireGuild()]
        public async Task Ban(CommandContext ctx, DiscordMember Member, [RemainingText] string Reason = null)
        {
            await Member.BanAsync(reason: Reason);
            await ctx.Message.DeleteAsync("Admin command hide");
            await LogAction(ctx.Guild, ctx.Message, "Ban", "Bans the given user", $"Baning {Member.Username} for reason {Reason}", DiscordColor.DarkRed);
        }

        [Command("kick"), RequirePrefixes("!"), RequireUserPermissions(DSharpPlus.Permissions.KickMembers), RequireGuild()]
        public async Task Kick(CommandContext ctx, DiscordMember Member, [RemainingText] string Reason = null)
        {
            await Member.RemoveAsync(Reason);
            await ctx.Message.DeleteAsync("Admin command hide");
            await LogAction(ctx.Guild, ctx.Message, "Kick", "Kicks the given user", $"Kicking {Member.Username} for reason {Reason}", DiscordColor.Red);
        }

        [Command("welcometoggle"), Aliases("wt","welcomemessage"), RequirePrefixes("!"), RequireUserPermissions(DSharpPlus.Permissions.Administrator)]
        public async Task WelcomeMessage(CommandContext ctx)
        {
            if (!GuildsList[ctx.Guild.Id].ModuleConfig.GreetModule)
            {
                await ctx.Guild.GetChannel(hcBotLogChannelId).SendMessageAsync($"Welcome messages have been **enabled!**");
                GuildsList[ctx.Guild.Id].ModuleConfig.GreetModule = true;
            }
            else
            {
                await ctx.Guild.GetChannel(hcBotLogChannelId).SendMessageAsync($"Welcome messages have been **disabled!**");
                GuildsList[ctx.Guild.Id].ModuleConfig.GreetModule = false;
            }
            var msqlCon = new MySqlConnection(Program.config.MysqlCon);
            await msqlCon.OpenAsync();
            try
            {
                MySqlCommand cmd = new MySqlCommand
                {
                    Connection = msqlCon,
                    CommandText = $"UPDATE `modules.config` SET `greetModule` = '{Convert.ToInt16(GuildsList[ctx.Guild.Id].ModuleConfig.GreetModule)}' WHERE `modules.config`.`guildID` = {ctx.Guild.Id}"
                };
                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
                Console.WriteLine(ex.StackTrace);
            }
            await msqlCon.CloseAsync();
        }

        [Command("bdaytoggle"), Aliases("bdt"), RequirePrefixes("!"), RequireUserPermissions(DSharpPlus.Permissions.Administrator)]
        public async Task BDayMessage(CommandContext ctx)
        {
            if (!GuildsList[ctx.Guild.Id].ModuleConfig.BirthdayModule)
            {
                await ctx.Guild.GetChannel(hcBotLogChannelId).SendMessageAsync($"Birthday messages have been **enabled!**");
                GuildsList[ctx.Guild.Id].ModuleConfig.BirthdayModule = true;
            }
            else
            {
                await ctx.Guild.GetChannel(hcBotLogChannelId).SendMessageAsync($"Birthday messages have been **disabled!**");
                GuildsList[ctx.Guild.Id].ModuleConfig.BirthdayModule = false;
            }
            var msqlCon = new MySqlConnection(Program.config.MysqlCon);
            await msqlCon.OpenAsync();
            try
            {
                MySqlCommand cmd = new MySqlCommand
                {
                    Connection = msqlCon,
                    CommandText = $"UPDATE `modules.config` SET `birthdayModule` = '{Convert.ToInt16(GuildsList[ctx.Guild.Id].ModuleConfig.GreetModule)}' WHERE `modules.config`.`guildID` = {ctx.Guild.Id}"
                };
                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
                Console.WriteLine(ex.StackTrace);
            }
            await msqlCon.CloseAsync();
        }

        [Command("admintoggle"), Aliases("admint"), RequirePrefixes("!"), RequireUserPermissions(DSharpPlus.Permissions.Administrator)]
        public async Task AdminModuleToggle(CommandContext ctx)
        {
            if (!GuildsList[ctx.Guild.Id].ModuleConfig.AdminModule)
            {
                await ctx.Guild.GetChannel(hcBotLogChannelId).SendMessageAsync($"Admin Module has been **enabled!**");
                GuildsList[ctx.Guild.Id].ModuleConfig.AdminModule = true;
            }
            else
            {
                await ctx.Guild.GetChannel(hcBotLogChannelId).SendMessageAsync($"Admin Module has been **disabled!**");
                GuildsList[ctx.Guild.Id].ModuleConfig.AdminModule = false;
            }
            var msqlCon = new MySqlConnection(HC_DBot.Program.config.MysqlCon);
            await msqlCon.OpenAsync();
            try
            {
                MySqlCommand cmd = new MySqlCommand
                {
                    Connection = msqlCon,
                    CommandText = $"UPDATE `modules.config` SET `adminModule` = '{Convert.ToInt16(GuildsList[ctx.Guild.Id].ModuleConfig.GreetModule)}' WHERE `modules.config`.`guildID` = {ctx.Guild.Id}"
                };
                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
                Console.WriteLine(ex.StackTrace);
            }
            await msqlCon.CloseAsync();
        }

        [Command("rulechannel"), RequirePrefixes("!"), RequireUserPermissions(DSharpPlus.Permissions.Administrator)]
        public async Task RuleChanChange(CommandContext ctx, DiscordChannel channel)
        {
            GuildsList[ctx.Guild.Id].ChannelConfig.RuleChannelID = channel.Id;
            var msqlCon = new MySqlConnection(Program.config.MysqlCon);
            await msqlCon.OpenAsync();
            try
            {
                MySqlCommand cmd = new MySqlCommand
                {
                    Connection = msqlCon,
                    CommandText = $"UPDATE `guilds.config` SET `ruleChannelID` = '{channel.Id}' WHERE `guilds.config`.`guildID` = {ctx.Guild.Id}"
                };
                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
                Console.WriteLine(ex.StackTrace);
            }
            await msqlCon.CloseAsync();
            await ctx.RespondAsync("set rulechannel to: " + channel.Name);
        }

        [Command("infochannel"), RequirePrefixes("!"), RequireUserPermissions(DSharpPlus.Permissions.Administrator)]
        public async Task InfoChanChange(CommandContext ctx, DiscordChannel channel)
        {
            GuildsList[ctx.Guild.Id].ChannelConfig.RuleChannelID = channel.Id;
            var msqlCon = new MySqlConnection(Program.config.MysqlCon);
            await msqlCon.OpenAsync();
            try
            {
                MySqlCommand cmd = new MySqlCommand
                {
                    Connection = msqlCon,
                    CommandText = $"UPDATE `guilds.config` SET `infoChannelID` = '{channel.Id}' WHERE `guilds.config`.`guildID` = {ctx.Guild.Id}"
                };
                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
                Console.WriteLine(ex.StackTrace);
            }
            await msqlCon.CloseAsync();
            await ctx.RespondAsync("set infochannel to: " + channel.Name);
        }

        [Command("cmdchannel"), RequirePrefixes("!"), RequireUserPermissions(DSharpPlus.Permissions.Administrator)]
        public async Task CmdChanChange(CommandContext ctx, DiscordChannel channel)
        {
            GuildsList[ctx.Guild.Id].ChannelConfig.RuleChannelID = channel.Id;
            var msqlCon = new MySqlConnection(Program.config.MysqlCon);
            await msqlCon.OpenAsync();
            try
            {
                MySqlCommand cmd = new MySqlCommand
                {
                    Connection = msqlCon,
                    CommandText = $"UPDATE `guilds.config` SET `cmdChannelID` = '{channel.Id}' WHERE `guilds.config`.`guildID` = {ctx.Guild.Id}"
                };
                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
                Console.WriteLine(ex.StackTrace);
            }
            await msqlCon.CloseAsync();
            await ctx.RespondAsync("set cmdchannel to: " + channel.Name);
        }

        [Command("logchannel"), Aliases("lc"), RequirePrefixes("!"), RequireUserPermissions(DSharpPlus.Permissions.Administrator)]
        public async Task LogChanChange(CommandContext ctx, DiscordChannel channel)
        {
            GuildsList[ctx.Guild.Id].ChannelConfig.RuleChannelID = channel.Id;
            var msqlCon = new MySqlConnection(Program.config.MysqlCon);
            await msqlCon.OpenAsync();
            try
            {
                MySqlCommand cmd = new MySqlCommand
                {
                    Connection = msqlCon,
                    CommandText = $"UPDATE `guilds.config` SET `logChannelID` = '{channel.Id}' WHERE `guilds.config`.`guildID` = {ctx.Guild.Id}"
                };
                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
                Console.WriteLine(ex.StackTrace);
            }
            await msqlCon.CloseAsync();
            await ctx.RespondAsync("set logchannel to: " + channel.Name);
        }

        [Command("rolechange"), RequirePrefixes("!"), RequireUserPermissions(DSharpPlus.Permissions.Administrator), Priority(2)]
        public async Task RoleChange(CommandContext ctx, DiscordRole role)
        {
            GuildsList[ctx.Guild.Id].ChannelConfig.RuleChannelID = role.Id;
            var msqlCon = new MySqlConnection(Program.config.MysqlCon);
            await msqlCon.OpenAsync();
            try
            {
                MySqlCommand cmd = new MySqlCommand
                {
                    Connection = msqlCon,
                    CommandText = $"UPDATE `guilds.config` SET `roleID` = '{role.Id}' WHERE `guilds.config`.`guildID` = {ctx.Guild.Id}"
                };
                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
                Console.WriteLine(ex.StackTrace);
            }
            await msqlCon.CloseAsync();
            await ctx.RespondAsync("set role to: " + role.Name);
        }

        [Command("rolechange"), RequirePrefixes("!"), RequireUserPermissions(DSharpPlus.Permissions.Administrator), Priority(1)]
        public async Task RoleChangeID(CommandContext ctx, ulong ID)
        {
            GuildsList[ctx.Guild.Id].ChannelConfig.RuleChannelID = ID;
            var role = ctx.Guild.GetRole(ID);
            var msqlCon = new MySqlConnection(Program.config.MysqlCon);
            await msqlCon.OpenAsync();
            try
            {
                MySqlCommand cmd = new MySqlCommand
                {
                    Connection = msqlCon,
                    CommandText = $"UPDATE `guilds.config` SET `roleID` = '{ID}' WHERE `guilds.config`.`guildID` = {ctx.Guild.Id}"
                };
                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
                Console.WriteLine(ex.StackTrace);
            }
            await msqlCon.CloseAsync();
            await ctx.RespondAsync("set role to: " + role.Name);
        }

        [Command("greet"), RequirePrefixes("!"), RequireUserPermissions(DSharpPlus.Permissions.Administrator), RequireGuild()]
        public async Task GreetManual(CommandContext ctx, DiscordMember user)
        {
            await user.SendMessageAsync($"Welcome {user.Mention}\n" +
            $"You succesfully landed on {ctx.Guild.Name} \n\n" +
            $"Please take a look into {ctx.Guild.GetChannel(GuildsList[ctx.Guild.Id].ChannelConfig.InfoChannelID).Mention} for informations regarding this server.\n" +
            $"To accept the rules ({ctx.Guild.GetChannel(GuildsList[ctx.Guild.Id].ChannelConfig.RuleChannelID).Mention}), please write `$accept-rules` in {ctx.Guild.GetChannel(GuildsList[ctx.Guild.Id].ChannelConfig.CmdChannelID).Mention}.\n" +
            $"You will automatically get assigned to the role *{ctx.Guild.GetRole(GuildsList[ctx.Guild.Id].ChannelConfig.RoleID).Name}*.\n\n" +
            $"{GuildsList[ctx.Guild.Id].ChannelConfig.CustomInfo}", false, null);
            await LogAction(ctx.Guild, ctx.Message, "GreetManuel", "Greet's the given user manually", $"User {user.Mention} was greeted manually by {ctx.Message.Author.Mention}", DiscordColor.SpringGreen);
        }
        
        [Command("role-add"), Aliases("radd"), RequirePrefixes("!"), RequireUserPermissions(DSharpPlus.Permissions.ManageRoles), RequireGuild()]
        public async Task RoleAdd(CommandContext ctx, DiscordMember user, DiscordRole role, [RemainingText] string Reason = null)
        {
            await ctx.Message.DeleteAsync();
            await user.GrantRoleAsync(role, Reason);
            await LogAction(ctx.Guild, ctx.Message, "RoleAdd", "Adds given role to given user", $"User {user.Mention} was granted role '{role.Name}'", role.Color);
        }

        [Command("role-remove"), Aliases("rrm"), RequirePrefixes("!"), RequireUserPermissions(DSharpPlus.Permissions.ManageRoles), RequireGuild()]
        public async Task RoleRemove(CommandContext ctx, DiscordMember user, DiscordRole role, [RemainingText] string Reason = null)
        {
            await ctx.Message.DeleteAsync();
            await user.RevokeRoleAsync(role, Reason);
            await LogAction(ctx.Guild, ctx.Message, "RoleRemove", "Removes given role to given user", $"User {user.Mention} was revoked role '{role.Name}'", role.Color);
        }
    }
}
