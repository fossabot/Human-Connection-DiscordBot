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

namespace HC_DBot.Config
{
    [Group("config")]
    class UserConfig : BaseCommandModule
    {

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
