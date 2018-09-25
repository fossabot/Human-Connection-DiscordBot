using DSharpPlus.EventArgs;
using static HC_DBot.MainClasses.Bot;
using MySql.Data.MySqlClient;
using System;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using System.Data;

namespace HC_DBot.Module
{
    class GreetModule
    {
        public GreetModule()
        {
            // Empty
        }

        public async Task GreetUser(GuildMemberAddEventArgs eventArgs)
        {
            DiscordGuild guild = eventArgs.Guild;
            int guildId = await GetGuildIdByUid(guild.Id);
            if (await ModuleCheck(guildId))
            {
                Console.WriteLine($"Module 'Greet' is enabled on {eventArgs.Guild.Name}");

                DiscordMember dmClient = eventArgs.Member;

                ulong infoChannel = 0;
                ulong ruleChannel = 0;
                ulong cmdChannel = 0;
                string customInfo = string.Empty;
                ulong role = 0;

                try
                {
                    await connection.OpenAsync();
                    MySqlCommand selectCmdSub = new MySqlCommand();
                    selectCmdSub.Connection = connection;
                    selectCmdSub.CommandText = $"SELECT * FROM guilds.config WHERE guildId='{guildId}' LIMIT 1";
                    MySqlDataReader reader = selectCmdSub.ExecuteReader();
                    using (reader)
                    {
                        DataTable dt = new DataTable();
                        dt.Load(reader);

                        foreach (DataRow row in dt.Rows)
                        {
                            infoChannel = Convert.ToUInt64(reader["infoChannelId"]);
                            ruleChannel = Convert.ToUInt64(reader["ruleChannelId"]);
                            cmdChannel = Convert.ToUInt64(reader["cmdChannelId"]);
                            customInfo = reader["customInfo"].ToString();
                            role = Convert.ToUInt64(reader["roleId"]);
                        }
                    }
                    reader.Close();
                    await connection.CloseAsync();
                }
                catch (Exception ey)
                {
                    Console.WriteLine("Error: " + ey);
                    Console.WriteLine(ey.StackTrace);
                }

                await dmClient.SendMessageAsync($"Welcome {dmClient.Mention}\n" +
                $"You succesfully landed on {guild.Name} \n\n" +
                $"Please take a look into {guild.GetChannel(infoChannel).Mention} for informations regarding this server.\n" +
                $"To accept the rules ({guild.GetChannel(ruleChannel).Mention}), please write `$accept-rules` in {guild.GetChannel(cmdChannel).Mention}.\n" +
                $"You will automatically get assigned to the role *{guild.GetRole(role).Name}*.\n\n" + 
                $"{customInfo}");
            }
            else
            {
                Console.WriteLine($"Module 'Greet' is disabled on {eventArgs.Guild.Name}");
            }
        }

        private async Task<bool> ModuleCheck(int guildId)
        {
            bool enabled = false;

            try
            {
                await connection.OpenAsync();
                MySqlCommand selectCmdSub = new MySqlCommand();
                selectCmdSub.Connection = connection;
                selectCmdSub.CommandText = $"SELECT `greetModule`* FROM modules.config WHERE guildId='{guildId}'";
                MySqlDataReader read = selectCmdSub.ExecuteReader();
                using (read)
                {
                    DataTable dta = new DataTable();
                    dta.Load(read);

                    foreach (DataRow rows in dta.Rows)
                    {
                        enabled = Convert.ToBoolean(read[0]);
                    }
                    read.Close();
                }
                await connection.CloseAsync();
            }
            catch (Exception ey)
            {
                Console.WriteLine("Error: " + ey);
                Console.WriteLine(ey.StackTrace);
            }

            return enabled;
        }
    }
}
