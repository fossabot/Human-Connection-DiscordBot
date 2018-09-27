using System;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using MySql.Data.MySqlClient;
using static HC_DBot.MainClasses.Bot;

namespace HC_DBot.Commands
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
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = connection;
                cmd.CommandText = $"UPDATE `guilds.users` SET `Birthdate` = ? WHERE `guilds.users`.`userID` = {ctx.User.Id}";
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
    }
}
