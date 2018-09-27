using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Threading.Tasks;
using static HC_DBot.MainClasses.Bot;
using MySql.Data.MySqlClient;
using System;

namespace HC_DBot.Commands
{
    class AdminCommands : BaseCommandModule
    {

        public static ulong hcBotLogChannelId = 490977974787768353;
        public static ulong botEmote = 491234510659125271;

        [Command("shutdown"), RequirePrefixes("!"), RequireUserPermissions(DSharpPlus.Permissions.Administrator)]
        public async Task BotShutdown(CommandContext ctx)
        {
            await ctx.Guild.GetChannel(hcBotLogChannelId).SendMessageAsync($"Shutting down {DiscordEmoji.FromGuildEmote(ctx.Client, botEmote)}");
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
            int pos = GuildsList.FindIndex(x => x.GuildID == ctx.Guild.Id);
            if (pos == -1) return;
            if (!GuildsList[pos].ModuleConfig.GreetModule)
            {
                await ctx.Guild.GetChannel(hcBotLogChannelId).SendMessageAsync($"Welcome messages have been **enabled!**");
                GuildsList[pos].ModuleConfig.GreetModule = true;

            }
            else
            {
                await ctx.Guild.GetChannel(hcBotLogChannelId).SendMessageAsync($"Welcome messages have been **disabled!**");
                GuildsList[pos].ModuleConfig.GreetModule = false;
            }
            var msqlCon = new MySqlConnection(HC_DBot.Program.config.MysqlCon);
            await msqlCon.OpenAsync();
            try
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = msqlCon;
                cmd.CommandText = $"UPDATE `modules.config` SET `greetModule` = '{Convert.ToInt16(GuildsList[pos].ModuleConfig.GreetModule)}' WHERE `modules.config`.`guildID` = {ctx.Guild.Id}";
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
            int pos = GuildsList.FindIndex(x => x.GuildID == ctx.Guild.Id);
            if (pos == -1) return;
            if (!GuildsList[pos].ModuleConfig.BirthdayModule)
            {
                await ctx.Guild.GetChannel(hcBotLogChannelId).SendMessageAsync($"Birthday messages have been **enabled!**");
                GuildsList[pos].ModuleConfig.BirthdayModule = true;

            }
            else
            {
                await ctx.Guild.GetChannel(hcBotLogChannelId).SendMessageAsync($"Birthday messages have been **disabled!**");
                GuildsList[pos].ModuleConfig.BirthdayModule = false;
            }
            var msqlCon = new MySqlConnection(HC_DBot.Program.config.MysqlCon);
            await msqlCon.OpenAsync();
            try
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = msqlCon;
                cmd.CommandText = $"UPDATE `modules.config` SET `birthdayModule` = '{Convert.ToInt16(GuildsList[pos].ModuleConfig.GreetModule)}' WHERE `modules.config`.`guildID` = {ctx.Guild.Id}";
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
            int pos = GuildsList.FindIndex(x => x.GuildID == ctx.Guild.Id);
            if (pos == -1) return;
            if (!GuildsList[pos].ModuleConfig.AdminModule)
            {
                await ctx.Guild.GetChannel(hcBotLogChannelId).SendMessageAsync($"Admin Module has been **enabled!**");
                GuildsList[pos].ModuleConfig.AdminModule = true;

            }
            else
            {
                await ctx.Guild.GetChannel(hcBotLogChannelId).SendMessageAsync($"Admin Module has been **disabled!**");
                GuildsList[pos].ModuleConfig.AdminModule = false;
            }
            var msqlCon = new MySqlConnection(HC_DBot.Program.config.MysqlCon);
            await msqlCon.OpenAsync();
            try
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = msqlCon;
                cmd.CommandText = $"UPDATE `modules.config` SET `adminModule` = '{Convert.ToInt16(GuildsList[pos].ModuleConfig.GreetModule)}' WHERE `modules.config`.`guildID` = {ctx.Guild.Id}";
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
            int pos = GuildsList.FindIndex(x => x.GuildID == ctx.Guild.Id);
            if (pos == -1) return;
            GuildsList[pos].ChannelConfig.RuleChannelID = channel.Id;
            var msqlCon = new MySqlConnection(Program.config.MysqlCon);
            await msqlCon.OpenAsync();
            try
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = msqlCon;
                cmd.CommandText = $"UPDATE `guilds.config` SET `ruleChannelID` = '{channel.Id}' WHERE `guilds.config`.`guildID` = {ctx.Guild.Id}";
                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
                Console.WriteLine(ex.StackTrace);
            }
            await msqlCon.CloseAsync();
            await ctx.RespondAsync("set channel to: " + channel.Name);
        }

        [Command("infochannel"), RequirePrefixes("!"), RequireUserPermissions(DSharpPlus.Permissions.Administrator)]
        public async Task InfoChanChange(CommandContext ctx, DiscordChannel channel)
        {
            int pos = GuildsList.FindIndex(x => x.GuildID == ctx.Guild.Id);
            if (pos == -1) return;
            GuildsList[pos].ChannelConfig.RuleChannelID = channel.Id;
            var msqlCon = new MySqlConnection(Program.config.MysqlCon);
            await msqlCon.OpenAsync();
            try
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = msqlCon;
                cmd.CommandText = $"UPDATE `guilds.config` SET `infoChannelID` = '{channel.Id}' WHERE `guilds.config`.`guildID` = {ctx.Guild.Id}";
                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
                Console.WriteLine(ex.StackTrace);
            }
            await msqlCon.CloseAsync();
            await ctx.RespondAsync("set channel to: " + channel.Name);
        }

        [Command("cmdchannel"), RequirePrefixes("!"), RequireUserPermissions(DSharpPlus.Permissions.Administrator)]
        public async Task CmdChanChange(CommandContext ctx, DiscordChannel channel)
        {
            int pos = GuildsList.FindIndex(x => x.GuildID == ctx.Guild.Id);
            if (pos == -1) return;
            GuildsList[pos].ChannelConfig.RuleChannelID = channel.Id;
            var msqlCon = new MySqlConnection(Program.config.MysqlCon);
            await msqlCon.OpenAsync();
            try
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = msqlCon;
                cmd.CommandText = $"UPDATE `guilds.config` SET `cmdChannelID` = '{channel.Id}' WHERE `guilds.config`.`guildID` = {ctx.Guild.Id}";
                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
                Console.WriteLine(ex.StackTrace);
            }
            await msqlCon.CloseAsync();
            await ctx.RespondAsync("set channel to: " + channel.Name);
        }

        [Command("rolechange"), RequirePrefixes("!"), RequireUserPermissions(DSharpPlus.Permissions.Administrator), Priority(2)]
        public async Task RoleChange(CommandContext ctx, DiscordRole role)
        {
            int pos = GuildsList.FindIndex(x => x.GuildID == ctx.Guild.Id);
            if (pos == -1) return;
            GuildsList[pos].ChannelConfig.RuleChannelID = role.Id;
            var msqlCon = new MySqlConnection(Program.config.MysqlCon);
            await msqlCon.OpenAsync();
            try
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = msqlCon;
                cmd.CommandText = $"UPDATE `guilds.config` SET `roleID` = '{role.Id}' WHERE `guilds.config`.`guildID` = {ctx.Guild.Id}";
                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
                Console.WriteLine(ex.StackTrace);
            }
            await msqlCon.CloseAsync();
            await ctx.RespondAsync("set channel to: " + role.Name);
        }

        [Command("rolechange"), RequirePrefixes("!"), RequireUserPermissions(DSharpPlus.Permissions.Administrator), Priority(1)]
        public async Task RoleChangeID(CommandContext ctx, ulong ID)
        {
            int pos = GuildsList.FindIndex(x => x.GuildID == ctx.Guild.Id);
            if (pos == -1) return;
            GuildsList[pos].ChannelConfig.RuleChannelID = ID;
            var role = ctx.Guild.GetRole(ID);
            var msqlCon = new MySqlConnection(Program.config.MysqlCon);
            await msqlCon.OpenAsync();
            try
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = msqlCon;
                cmd.CommandText = $"UPDATE `guilds.config` SET `roleID` = '{ID}' WHERE `guilds.config`.`guildID` = {ctx.Guild.Id}";
                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
                Console.WriteLine(ex.StackTrace);
            }
            await msqlCon.CloseAsync();
            await ctx.RespondAsync("set channel to: " + role.Name);
        }
    }
}
