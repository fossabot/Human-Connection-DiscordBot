using DSharpPlus;
using DSharpPlus.EventArgs;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using System.Threading.Tasks;
using System;
using System.Data;
using static HC_DBot.GuildStatics;
using HC_DBot.Module;
using System.Threading;
using DSharpPlus.Entities;
using MySql.Data.MySqlClient;

namespace HC_DBot.MainClasses
{
    class Bot : IDisposable
    {
        private static DiscordClient Client { get; set; }
        private CommandsNextExtension CNext;
        private InteractivityExtension INext;
        private static RoleModule roles = new RoleModule();
        private static GreetModule greetModule = new GreetModule();
        public static MySqlConnection connection;

        public Bot(string Token, string mysqlCon)
        {
            connection = new MySqlConnection(mysqlCon);
            ShutdownRequest = new CancellationTokenSource();
            StartUp();
            var cfg = new DiscordConfiguration
            {
                Token = Token,                
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                LogLevel = LogLevel.Debug,
                UseInternalLogHandler = true
            };
            Client = new DiscordClient(cfg);
            Client.GuildMemberAdded += GuildMemberAddActions;
            Client.MessageReactionAdded += ReactorModulAdd;
            Client.MessageReactionRemoved += ReactorModulRemove;
            Client.GuildDownloadCompleted += AddUsers;
            CNext = Client.UseCommandsNext(new CommandsNextConfiguration {
                StringPrefixes = new string[] { "$", "!" },
                EnableDefaultHelp = false
            });
            CNext.RegisterCommands<Commands.UserCommands>();
            CNext.RegisterCommands<Commands.AdminCommands>();
            CNext.RegisterCommands<Commands.UserConfig>();
            INext = Client.UseInteractivity(new InteractivityConfiguration { });
        }

        public static void StartUp()
        {
            try
            {
                connection.Open();
                MySqlCommand selectCmd = new MySqlCommand();
                selectCmd.Connection = connection;
                selectCmd.CommandText = $"SELECT * FROM guilds";
                MySqlDataReader reader = selectCmd.ExecuteReader();
                using(reader)
                {
                    DataTable dt = new DataTable();
                    dt.Load(reader);

                    foreach(DataRow row in dt.Rows)
                    {
                        var serverName = row["server"].ToString();
                        var serverConfig = GetServerModuleConfigByUid(int.Parse(row["id"].ToString()));
                        Console.WriteLine($"Guild: {serverName} | Config: {serverConfig}");
                    }
                }
                connection.Close();
            }
            catch (Exception ey)
            {
                Console.WriteLine("Error: " + ey);
                Console.WriteLine(ey.StackTrace);
            }
        }

        private static string GetServerModuleConfigByUid(int id)
        {
            var serverConfigString = string.Empty;
            try
            {
                MySqlCommand selectCmdSub = new MySqlCommand();
                selectCmdSub.Connection = connection;
                selectCmdSub.CommandText = $"SELECT * FROM `modules.config` WHERE guildId='{id}'";
                MySqlDataReader read = selectCmdSub.ExecuteReader();
                if (read.Read())
                {
                    serverConfigString = $"Admin: {Convert.ToString(read["adminModule"])} | Greet: {Convert.ToString(read["greetModule"])} | Birthday: {Convert.ToString(read["birthdayModule"])}";
                }
                read.Close();
            }
            catch (Exception ey)
            {
                Console.WriteLine("Error: " + ey);
                Console.WriteLine(ey.StackTrace);
            }

            return serverConfigString;
        }

        public async Task GuildMemberAddActions(GuildMemberAddEventArgs e)
        {
            await AddJoinUser(e);
            await JoinMSG(e);
        }

        public async Task ReactorModulAdd(MessageReactionAddEventArgs e)
        {
            await roles.ReactOnAdd(e, Client);
        }

        public static async Task ReactorModulRemove(MessageReactionRemoveEventArgs e)
        {
            await roles.ReactOnRemove(e, Client);
        }

        public static async Task JoinMSG(GuildMemberAddEventArgs e)
        {
            await greetModule.GreetUser(e);
        }

        public void Dispose()
        {
            connection.Close();
            connection.Dispose();
            connection = null;
            Client.Dispose();
            INext = null;
            CNext = null;
            Environment.Exit(0);
        }

        public async Task RunAsync()
        {
            await Client.ConnectAsync();
            while (!ShutdownRequest.IsCancellationRequested)
            {
                await Task.Delay(25);
            }
            await Client.DisconnectAsync();
            await Task.Delay(2500);
            Dispose();
        }

        public async Task AddJoinUser(GuildMemberAddEventArgs e)
        {
            try
            {
                var guildId = GetGuildIdByUid(e.Guild.Id);
                await connection.OpenAsync();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = connection;
                cmd.CommandText = $"INSERT INTO `guilds.users` (discordId, discordName, guildId, notes) "
                                                 + $" values (?, ?, ?, 'auto add') "
                                                 + $" ON DUPLICATE KEY UPDATE discordId=discordId";
                cmd.Parameters.Add("discordId", MySqlDbType.VarChar).Value = Convert.ToString(e.Member.Id);
                cmd.Parameters.Add("discordName", MySqlDbType.VarChar).Value = Convert.ToString(e.Member.Username);
                cmd.Parameters.Add("guildId", MySqlDbType.Int16).Value = guildId;
                await cmd.ExecuteNonQueryAsync();
                await connection.CloseAsync();
            }
            catch (Exception ey)
            {
                Console.WriteLine("Error: " + ey);
                Console.WriteLine(ey.StackTrace);
            }
            Console.WriteLine("UserAdd done!");
        }

        public static int GetGuildIdByUid(ulong id)
        {
            int guildInt = 0;

            try
            {
                connection.Open();
                MySqlCommand selectCmd = new MySqlCommand();
                selectCmd.Connection = connection;
                selectCmd.CommandText = $"SELECT id FROM `guilds` WHERE guildId='{Convert.ToString(id)}' LIMIT 1";
                MySqlDataReader reader = selectCmd.ExecuteReader();
                if (reader.Read())
                {
                    guildInt = Convert.ToInt16(reader["id"]);
                }
                connection.Close();
                reader.Close();
            }
            catch (Exception ey)
            {
                Console.WriteLine("Error: " + ey);
                Console.WriteLine(ey.StackTrace);
            }

            return guildInt;
        }

        public async Task<string> GetGuildNameByUid(ulong id)
        {
            string guildName = String.Empty;

            try
            {
                await connection.OpenAsync();
                MySqlCommand selectCmd = new MySqlCommand();
                selectCmd.Connection = connection;
                selectCmd.CommandText = $"SELECT * FROM `guilds` WHERE guildId='{Convert.ToString(id)}' LIMIT 1";
                MySqlDataReader reader = selectCmd.ExecuteReader();
                if (reader.Read())
                {
                    guildName = Convert.ToString(reader["server"]);
                }
                reader.Close();
                await connection.CloseAsync();
            }
            catch (Exception ey)
            {
                Console.WriteLine("Error: " + ey);
                Console.WriteLine(ey.StackTrace);
            }

            return guildName;
        }

        public async Task AddUsers(GuildDownloadCompletedEventArgs e)
        {
            foreach (var guild in e.Guilds)
            {
                var guildId = GetGuildIdByUid(guild.Value.Id);
                foreach (var user in guild.Value.Members)
                {
                    try
                    {
                        await connection.OpenAsync();
                        MySqlCommand cmd = new MySqlCommand();
                        cmd.Connection = connection;
                        cmd.CommandText = $"INSERT INTO `guilds.users` (discordId, discordName, guildId, notes) "
                                                         + $" values (?, ?, ?, 'auto add') "
                                                         + $" ON DUPLICATE KEY UPDATE discordId=discordId";
                        cmd.Parameters.Add("discordId", MySqlDbType.VarChar).Value = Convert.ToString(user.Id);
                        cmd.Parameters.Add("discordName", MySqlDbType.VarChar).Value = Convert.ToString(user.Username);
                        cmd.Parameters.Add("guildId", MySqlDbType.Int16).Value = guildId;
                        await cmd.ExecuteNonQueryAsync();
                        await connection.CloseAsync();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + ex);
                        Console.WriteLine(ex.StackTrace);
                    }
                }
            }
            Console.WriteLine("UserAdd done!");
        }
    }
}
