﻿using Discord;
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
            if (author.GuildPermissions.Administrator)
            {
                if (command[0] == "!kick")
                {
                    var reason = command[1];
                    var userToKick = message.MentionedUsers.ElementAt(0) as SocketGuildUser;
                    await Kick(message, userToKick, reason, logChannel);
                }
            } else
            {
                if (message.MentionedUsers.Count != 0)
                {
                    on = $" on {message.MentionedUsers.ElementAt(0).Username}";
                }
                await logChannel.SendMessageAsync($"**Alert**\n{author.Username} tried to execute admin command `{command[0]}`{on}\nFull requested command context: `{command[1]}`");
            }

            Program.DelMsg(message.Channel, message.Id);
        }

        private async Task Kick(SocketMessage message, SocketGuildUser user, string reason, SocketTextChannel channel)
        {
            await channel.SendMessageAsync($"**Kick**\n{user.Mention} ({user.Username}) with reason `{reason}`");
            await user.KickAsync();
        }

        private string[] GetCommand(SocketMessage msg)
        {
            var fullCommand = msg.Content;

            var cmd = fullCommand.Split(new char[] { ' ' }, 2);

            return cmd;
        }
    }
}
