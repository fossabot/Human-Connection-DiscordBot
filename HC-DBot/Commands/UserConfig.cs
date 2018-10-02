using System;
using System.Threading.Tasks;
using System.Linq;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using MySql.Data.MySqlClient;
using static HC_DBot.MainClasses.Bot;
using DSharpPlus.Entities;
using System.Net;
using System.IO;

namespace HC_DBot.Commands
{
    [Group("config")]
    class UserConfig : BaseCommandModule
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

        [Command("set-birthday"), Description("Set your birthday!\nPlease use this format: MM-DD-YYYY\n__Example:__ $config birthday 03-24-1994")]
        public async Task SetBDay(CommandContext ctx,[Description("Please use this format: MM-DD-YYYY\n__Example:__ $config birthday 03-24-1994")] DateTime date)
        {
            try
            {
                await connection.OpenAsync();
                MySqlCommand cmd = new MySqlCommand
                {
                    Connection = connection,
                    CommandText = $"UPDATE `guilds.users` SET `Birthdate` = ? WHERE `guilds.users`.`userID` = {ctx.User.Id}"
                };
                cmd.Parameters.Add("Birthdate", MySqlDbType.Date).Value = date.Date;
                await cmd.ExecuteNonQueryAsync();
                await ctx.RespondAsync($"Set your birthday to: {date.Date.ToShortDateString()}");
                await connection.CloseAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }

        [Command("get-birthday"), Description("Get birthday of specific user!")]
        public async Task SetBDay(CommandContext ctx, DiscordUser user)
        {
            try
            {
                await connection.OpenAsync();
                MySqlCommand cmd = new MySqlCommand
                {
                    Connection = connection,
                    CommandText = $"SELECT * FROM `guilds.users` WHERE `guilds.users`.`userID` = {user.Id} LIMIT 1"
                };
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    await ctx.RespondAsync($"Birthday of {user.Mention}: {Convert.ToDateTime(reader["Birthdate"]).Date.ToShortDateString()}");
                }
                await connection.CloseAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }
    }
}
