using Discord;
using Discord.Net;
using Discord.Webhook;
using Discord.WebSocket;
using Discord.Rest;
using Discord.Rpc;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HumanConnection.DiscordBot.Modules
{
    public class AdminCommandModule
    {
        private static DiscordSocketClient client;
        private static SocketTextChannel logChannel;

        public AdminCommandModule(DiscordSocketClient socketClient, SocketTextChannel log)
        {
            client = socketClient;
            logChannel = log;
            Program.Log(new LogMessage(LogSeverity.Debug, "AdminCommandModule", "Admin module loaded"));
        }

        public async Task HandleMessage(SocketMessage message)
        {
            SocketGuildUser author = (SocketGuildUser)message.Author;
            var on = String.Empty;
            var command = GetCommand(message);
            /*if (author.GuildPermissions.Administrator)
            {*/
                await ExecuteCommand(command, message, author);
            /*}
            else
            {
                if (message.MentionedUsers.Count != 0)
                {
                    on = $" on {message.MentionedUsers.ElementAt(0).Username}";
                }
                await logChannel.SendMessageAsync($"**Alert**\n{author.Username} tried to execute admin command `{command[0]}`{on}\nFull requested command context: `{command[1]}`");
            }*/

            Program.DelMsg(message.Channel, message.Id);
        }

        private async Task ExecuteCommand(string[] command, SocketMessage message, SocketGuildUser author)
        {
            Program.Log(new LogMessage(LogSeverity.Info, "Admin Module", $"{command[0]} | {command[1]}"));
            if (command[0] == "!kick")
            {
                Program.Log(new LogMessage(LogSeverity.Info, "Admin Module", $"{command[0]} | {command[1]}"));
                var reason = command[1];
                var guildUser = message.MentionedUsers.ElementAt(0) as SocketGuildUser;
                await Kick(guildUser, reason, logChannel);
            }
            else if (command[0] == "!ban")
            {
                Program.Log(new LogMessage(LogSeverity.Info, "Admin Module", $"{command[0]} | {command[1]}"));
                var vari = TrimIt(command[1], ' ', 2);
                var reason = vari[1];
                var timespan = Convert.ToInt16(vari[0]);
                var guildUser = message.MentionedUsers.ElementAt(0) as SocketGuildUser;
                await Ban(guildUser, timespan, reason, logChannel);
            }
            else
            {
                Program.Log(new LogMessage(LogSeverity.Info, "Admin Module", $"{command[0]} | {command[1]}"));
                Console.WriteLine($"Unkown command '{command[0]}'");
            }
        }

        private async Task Kick(SocketGuildUser user, string reason, SocketTextChannel channel)
        {
            await channel.SendMessageAsync($"**Kick**\n{user.Mention} ({user.Username}) with reason `{reason}`");
            await user.KickAsync(reason);
        }

        private async Task Ban(SocketGuildUser user, int length, string reason, SocketTextChannel channel)
        {
            await channel.SendMessageAsync($"**Ban**\n{user.Mention} ({user.Username}) with reason `{reason}` and deletes messages from the last {length} days");
            await user.Guild.AddBanAsync(user as IUser, length, reason);
        }

        private string[] TrimIt(string text, char delemite, int length)
        {
            string[] stringArr = text.Split(new char[] { delemite }, length);

            return stringArr;
        }

        private string[] GetCommand(SocketMessage msg)
        {
            var fullCommand = msg.Content;

            string[] cmd = fullCommand.Split(new char[] { ' ' }, 2);

            return cmd;
        }
    }
}
