using DSharpPlus;
using DSharpPlus.EventArgs;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using System.Threading.Tasks;
using System;
using System.Threading;
using MySql.Data.MySqlClient;
using System.Linq;
using System.Collections.Generic;
using DSharpPlus.Entities;

namespace HC_DBot.MainClasses
{
    class Bot : IDisposable
    {
        private static DiscordClient Client { get; set; }
        private CommandsNextExtension CNext;
        private InteractivityExtension INext;
        public static MySqlConnection connection;
        public static Dictionary<ulong, Guilds> GuildsList = new Dictionary<ulong, Guilds>();
        public static CancellationTokenSource ShutdownRequest;

        public Bot(string Token, string mysqlCon)
        {
            connection = new MySqlConnection(mysqlCon);
            ShutdownRequest = new CancellationTokenSource();
            var cfg = new DiscordConfiguration
            {
                Token = Token,                
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                LogLevel = LogLevel.Debug,
                UseInternalLogHandler = true
            };
            Client = new DiscordClient(cfg);
            Client.GuildDownloadCompleted += BotGuildsDownloaded;
            Client.GuildMemberAdded += MemberAdd;
            Client.GuildCreated += BotGuildAdded;
            //Client.MessageReactionAdded += ReactAdd;
            //Client.MessageReactionRemoved += ReactRemove;
            CNext = Client.UseCommandsNext(new CommandsNextConfiguration {
                StringPrefixes = new string[] { "$", "!" },
                EnableDefaultHelp = false
            });
            CNext.RegisterCommands<Commands.UserCommands>();
            CNext.RegisterCommands<Commands.AdminCommands>();
            CNext.RegisterCommands<Commands.UserConfig>();
            INext = Client.UseInteractivity(new InteractivityConfiguration { });
        }

        public void Dispose()
        {
            connection.Close();
            connection.Dispose();
            connection = null;
            Client.Dispose();
            INext = null;
            CNext = null;
            Client = null;
            Environment.Exit(0);
        }

        public async Task RunAsync()
        {
            await Client.ConnectAsync();
            var bday = CheckBday();
            bday.Wait(1000);
            while (!ShutdownRequest.IsCancellationRequested)
            {
                await Task.Delay(25);
            }
            await Client.DisconnectAsync();
            await Task.Delay(2500);
            Dispose();
        }

        public async Task CheckBday()
        {
            var BdayCon = new MySqlConnection(Program.config.MysqlCon);
            await Task.Delay(TimeSpan.FromMinutes(1));
            while (true)
            {
                try
                {
                    await BdayCon.OpenAsync();
                    List<ulong> BDayPPL = new List<ulong>();
                    Console.WriteLine(BDayPPL.Count);
                    MySqlCommand bdaycmd = new MySqlCommand();
                    bdaycmd.Connection = BdayCon;
                    var timenow = DateTime.Now;
                    string day = timenow.Day.ToString();
                    if (day.Length == 1) day = "0" + day;
                    string month = timenow.Month.ToString();
                    if (month.Length == 1) month = "0" + month;
                    bdaycmd.CommandText = $"SELECT * FROM `guilds.users` WHERE MONTH(`Birthdate`) = {month} AND DAY(`Birthdate`) = {day}";
                    var reader = await bdaycmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        Console.WriteLine(reader["userID"]);
                        BDayPPL.Add(Convert.ToUInt64(reader["userID"]));
                    }
                    await BdayCon.CloseAsync();
                    Console.WriteLine(BDayPPL.Count);
                    if (BDayPPL.Count != 0)
                    {
                        foreach (var user in BDayPPL)
                        {
                            var entry = GuildsList.First(x => x.Value.GuildMembers.Any(y => y.Key == user));
                            if (entry.Value.ModuleConfig.BirthdayModule)
                            {
                                var Guild = await Client.GetGuildAsync(entry.Key);
                                var DM = await Guild.GetMemberAsync(user);
                                entry.Value.GuildMembers.First(x => x.Key == user).Value.BdaySent = true;
                                var resetTrigger = ResetBday(entry.Value.GuildMembers.First(x => x.Key == user).Value);
                                resetTrigger.Wait(1000);
                                await DM.SendMessageAsync("Congrats!");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                }
                await Task.Delay(TimeSpan.FromHours(1));
            }
        }

        public async Task ResetBday(Members mb)
        {
            await Task.Delay(TimeSpan.FromDays(1));
            mb.BdaySent = false;
        }

        public async Task BotGuildsDownloaded(GuildDownloadCompletedEventArgs e)
        {
            foreach (var guild in e.Guilds)
            {
                Console.WriteLine(guild.Value.Name);
                await connection.OpenAsync();
                try
                {
                    MySqlCommand guildcmd = new MySqlCommand();
                    guildcmd.Connection = connection;
                    guildcmd.CommandText = $"INSERT INTO `guilds` (`guildID`, `guildName`, `guildDefaultInvite`, `guildOwner`) VALUES (?, ?, NULL, ?) ON DUPLICATE KEY UPDATE guildOwner=guildOwner";
                    guildcmd.Parameters.Add("guildID", MySqlDbType.Int64).Value = guild.Value.Id;
                    guildcmd.Parameters.Add("guildName", MySqlDbType.VarChar).Value = guild.Value.Name;
                    guildcmd.Parameters.Add("guildOwner", MySqlDbType.Int64).Value = guild.Value.Owner.Id;
                    await guildcmd.ExecuteNonQueryAsync();
                    GuildsList.Add(guild.Value.Id, new Guilds
                    {
                        GuildName = guild.Value.Name,
                        GuildDefaultInvite = null,
                        GuildOwner = guild.Value.Owner.Id,
                        GuildMembers = new Dictionary<ulong, Members>(),
                        ChannelConfig = new ChannelConfig(),
                        ModuleConfig = new ModuleConfig()
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex);
                    Console.WriteLine(ex.StackTrace);
                } //Guild Top
                try
                {
                    MySqlCommand guildcmd = new MySqlCommand();
                    guildcmd.Connection = connection;
                    guildcmd.CommandText = $"INSERT INTO `guilds.config` (`guildID`, `ruleChannelID`, `infoChannelID`, `cmdChannelID`, `logChannelID`, `roleID`, `customInfo`) VALUES (?, 0, 0, 0, 0, 0, ?) ON DUPLICATE KEY UPDATE ruleChannelID=ruleChannelID";
                    guildcmd.Parameters.Add("guildID", MySqlDbType.Int64).Value = guild.Value.Id;
                    guildcmd.Parameters.Add("customInfo", MySqlDbType.VarChar).Value = "to be filled";
                    await guildcmd.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex);
                    Console.WriteLine(ex.StackTrace);
                } //Guild Chan Conf
                await connection.CloseAsync();
                await connection.OpenAsync();
                try
                {
                    MySqlCommand guildcmd = new MySqlCommand();
                    guildcmd.Connection = connection;
                    guildcmd.CommandText = $"SELECT * FROM `guilds.config` WHERE `guildID` = {guild.Value.Id}";
                    var reader = await guildcmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        GuildsList[guild.Value.Id].ChannelConfig = (new ChannelConfig
                        {
                            RuleChannelID = Convert.ToUInt64(reader["ruleChannelId"]),
                            InfoChannelID = Convert.ToUInt64(reader["infoChannelId"]),
                            CmdChannelID = Convert.ToUInt64(reader["cmdChannelId"]),
                            LogChannelID = Convert.ToUInt64(reader["logChannelID"]),
                            RoleID = Convert.ToUInt64(reader["roleID"]),
                            CustomInfo = reader["customInfo"].ToString()
                        });
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex);
                    Console.WriteLine(ex.StackTrace);
                } //Guild Channel Config Get
                try
                {
                    MySqlCommand guildcmd = new MySqlCommand();
                    guildcmd.Connection = connection;
                    guildcmd.CommandText = $"INSERT INTO `modules.config` (`guildID`, `adminModule`, `greetModule`, `birthdayModule`) VALUES (?, ?, ?, ?) ON DUPLICATE KEY UPDATE adminModule=adminModule";
                    guildcmd.Parameters.Add("guildID", MySqlDbType.Int64).Value = guild.Value.Id;
                    guildcmd.Parameters.Add("adminModule", MySqlDbType.Int16).Value = 0;
                    guildcmd.Parameters.Add("greetModule", MySqlDbType.Int16).Value = 0;
                    guildcmd.Parameters.Add("birthdayModule", MySqlDbType.Int16).Value = 0;
                    await guildcmd.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex);
                    Console.WriteLine(ex.StackTrace);
                } //Module Conf
                await connection.CloseAsync();
                await connection.OpenAsync();
                try
                {
                    MySqlCommand guildcmd = new MySqlCommand();
                    guildcmd.Connection = connection;
                    guildcmd.CommandText = $"SELECT * FROM `modules.config` WHERE `guildID` = {guild.Value.Id}";
                    var reader = await guildcmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        GuildsList[guild.Value.Id].ModuleConfig = new ModuleConfig
                        {
                            AdminModule = Convert.ToBoolean(reader["adminModule"]),
                            GreetModule = Convert.ToBoolean(reader["greetModule"]),
                            BirthdayModule = Convert.ToBoolean(reader["birthdayModule"])
                        };
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex);
                    Console.WriteLine(ex.StackTrace);
                } //Guild Module Config Get
                await connection.CloseAsync();
                await connection.OpenAsync();
                try
                {
                    MySqlCommand guildcmd = new MySqlCommand();
                    guildcmd.Connection = connection;
                    guildcmd.CommandText = $"SELECT * FROM `modules.greet` WHERE `guildID` = {guild.Value.Id}";
                    var reader = await guildcmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        GuildsList[guild.Value.Id].ModuleConfig.GreetMessages.Add(new GreetMessages
                        {
                            AnnounceString = reader["announceString"].ToString()
                        });
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex);
                    Console.WriteLine(ex.StackTrace);
                } //Get Annouce
                foreach (var emoji in guild.Value.Emojis)
                {
                    try
                    {
                        MySqlCommand cmd = new MySqlCommand();
                        cmd.Connection = connection;
                        cmd.CommandText = $"INSERT INTO `guilds.emotes` (`guildID`, `emoteID`, `emoteURL`, `emoteName`) VALUES (?, ?, ?, ?) ON DUPLICATE KEY UPDATE emoteName=emoteName";
                        cmd.Parameters.Add("guildID", MySqlDbType.Int64).Value = guild.Value.Id;
                        cmd.Parameters.Add("emoteID", MySqlDbType.Int64).Value = emoji.Id;
                        cmd.Parameters.Add("emoteURL", MySqlDbType.VarChar).Value = emoji.Url;
                        cmd.Parameters.Add("emoteName", MySqlDbType.VarChar).Value = emoji.Name;
                        await cmd.ExecuteNonQueryAsync();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + ex);
                        Console.WriteLine(ex.StackTrace);
                    }
                }
                foreach (var user in guild.Value.Members)
                {
                    try
                    {
                        MySqlCommand cmd = new MySqlCommand();
                        cmd.Connection = connection;
                        cmd.CommandText = $"INSERT INTO `guilds.users` (`userID`, `userName`, `userDiscriminator`, `Birthdate`, `changeDate`) VALUES (?, ?, ?, NULL, CURRENT_TIMESTAMP) ON DUPLICATE KEY UPDATE userName=userName";
                        cmd.Parameters.Add("userID", MySqlDbType.Int64).Value = user.Id;
                        cmd.Parameters.Add("userName", MySqlDbType.VarChar).Value = user.Username;
                        cmd.Parameters.Add("userDiscriminator", MySqlDbType.VarChar).Value = user.Discriminator;
                        await cmd.ExecuteNonQueryAsync();
                        GuildsList[guild.Value.Id].GuildMembers.Add(user.Id, new Members
                        {
                            UserName = user.Username,
                            UserDiscriminator = user.Discriminator,
                            BdaySent = false
                        });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + ex);
                        Console.WriteLine(ex.StackTrace);
                    }
                }
                await connection.CloseAsync();
            }
            Console.WriteLine("UserAdd done!");
        }

        public async Task BotGuildAdded(GuildCreateEventArgs e)
        {
            Console.WriteLine(e.Guild.Name);
            await connection.OpenAsync();
            try
            {
                MySqlCommand guildcmd = new MySqlCommand();
                guildcmd.Connection = connection;
                guildcmd.CommandText = $"INSERT INTO `guilds` (`guildID`, `guildName`, `guildDefaultInvite`, `guildOwner`) VALUES (?, ?, NULL, ?) ON DUPLICATE KEY UPDATE guildOwner=guildOwner";
                guildcmd.Parameters.Add("guildID", MySqlDbType.Int64).Value = e.Guild.Id;
                guildcmd.Parameters.Add("guildName", MySqlDbType.VarChar).Value = e.Guild.Name;
                guildcmd.Parameters.Add("guildOwner", MySqlDbType.Int64).Value = e.Guild.Owner.Id;
                await guildcmd.ExecuteNonQueryAsync();
                GuildsList.Add(e.Guild.Id, new Guilds
                {
                    GuildName = e.Guild.Name,
                    GuildDefaultInvite = null,
                    GuildOwner = e.Guild.Owner.Id,
                    GuildMembers = new Dictionary<ulong, Members>(),
                    ChannelConfig = new ChannelConfig(),
                    ModuleConfig = new ModuleConfig()
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
                Console.WriteLine(ex.StackTrace);
            } //Guild Top
            try
            {
                MySqlCommand guildcmd = new MySqlCommand();
                guildcmd.Connection = connection;
                guildcmd.CommandText = $"INSERT INTO `guilds.config` (`guildID`, `ruleChannelID`, `infoChannelID`, `cmdChannelID`, `logChannelID`, `roleID`, `customInfo`) VALUES (?, 0, 0, 0, 0, 0, ?) ON DUPLICATE KEY UPDATE ruleChannelID=ruleChannelID";
                guildcmd.Parameters.Add("guildID", MySqlDbType.Int64).Value = e.Guild.Id;
                guildcmd.Parameters.Add("customInfo", MySqlDbType.VarChar).Value = "to be filled";
                await guildcmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
                Console.WriteLine(ex.StackTrace);
            } //Guild Chan Conf
            await connection.CloseAsync();
            await connection.OpenAsync();
            try
            {
                MySqlCommand guildcmd = new MySqlCommand();
                guildcmd.Connection = connection;
                guildcmd.CommandText = $"SELECT * FROM `guilds.config` WHERE `guildID` = {e.Guild.Id}";
                var reader = await guildcmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    GuildsList[e.Guild.Id].ChannelConfig = (new ChannelConfig
                    {
                        RuleChannelID = Convert.ToUInt64(reader["ruleChannelId"]),
                        InfoChannelID = Convert.ToUInt64(reader["infoChannelId"]),
                        CmdChannelID = Convert.ToUInt64(reader["cmdChannelId"]),
                        LogChannelID = Convert.ToUInt64(reader["logChannelID"]),
                        RoleID = Convert.ToUInt64(reader["roleID"]),
                        CustomInfo = reader["customInfo"].ToString()
                    });
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
                Console.WriteLine(ex.StackTrace);
            } //Guild Channel Config Get
            try
            {
                MySqlCommand guildcmd = new MySqlCommand();
                guildcmd.Connection = connection;
                guildcmd.CommandText = $"INSERT INTO `modules.config` (`guildID`, `adminModule`, `greetModule`, `birthdayModule`) VALUES (?, ?, ?, ?) ON DUPLICATE KEY UPDATE adminModule=adminModule";
                guildcmd.Parameters.Add("guildID", MySqlDbType.Int64).Value = e.Guild.Id;
                guildcmd.Parameters.Add("adminModule", MySqlDbType.Int16).Value = 0;
                guildcmd.Parameters.Add("greetModule", MySqlDbType.Int16).Value = 0;
                guildcmd.Parameters.Add("birthdayModule", MySqlDbType.Int16).Value = 0;
                await guildcmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
                Console.WriteLine(ex.StackTrace);
            } //Module Conf
            await connection.CloseAsync();
            await connection.OpenAsync();
            try
            {
                MySqlCommand guildcmd = new MySqlCommand();
                guildcmd.Connection = connection;
                guildcmd.CommandText = $"SELECT * FROM `modules.config` WHERE `guildID` = {e.Guild.Id}";
                var reader = await guildcmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    GuildsList[e.Guild.Id].ModuleConfig = new ModuleConfig
                    {
                        AdminModule = Convert.ToBoolean(reader["adminModule"]),
                        GreetModule = Convert.ToBoolean(reader["greetModule"]),
                        BirthdayModule = Convert.ToBoolean(reader["birthdayModule"])
                    };
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
                Console.WriteLine(ex.StackTrace);
            } //Guild Module Config Get
            await connection.CloseAsync();
            await connection.OpenAsync();
            try
            {
                MySqlCommand guildcmd = new MySqlCommand();
                guildcmd.Connection = connection;
                guildcmd.CommandText = $"SELECT * FROM `modules.greet` WHERE `guildID` = {e.Guild.Id}";
                var reader = await guildcmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    GuildsList[e.Guild.Id].ModuleConfig.GreetMessages.Add(new GreetMessages
                    {
                        AnnounceString = reader["announceString"].ToString()
                    });
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
                Console.WriteLine(ex.StackTrace);
            } //Get Annouce
            foreach (var emoji in e.Guild.Emojis)
            {
                try
                {
                    MySqlCommand cmd = new MySqlCommand();
                    cmd.Connection = connection;
                    cmd.CommandText = $"INSERT INTO `guilds.emotes` (`guildID`, `emoteID`, `emoteURL`, `emoteName`) VALUES (?, ?, ?, ?) ON DUPLICATE KEY UPDATE emoteName=emoteName";
                    cmd.Parameters.Add("guildID", MySqlDbType.Int64).Value = e.Guild.Id;
                    cmd.Parameters.Add("emoteID", MySqlDbType.Int64).Value = emoji.Id;
                    cmd.Parameters.Add("emoteURL", MySqlDbType.VarChar).Value = emoji.Url;
                    cmd.Parameters.Add("emoteName", MySqlDbType.VarChar).Value = emoji.Name;
                    await cmd.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex);
                    Console.WriteLine(ex.StackTrace);
                }
            }
            foreach (var user in e.Guild.Members)
            {
                try
                {
                    MySqlCommand cmd = new MySqlCommand();
                    cmd.Connection = connection;
                    cmd.CommandText = $"INSERT INTO `guilds.users` (`userID`, `userName`, `userDiscriminator`, `Birthdate`, `changeDate`) VALUES (?, ?, ?, NULL, CURRENT_TIMESTAMP) ON DUPLICATE KEY UPDATE userName=userName";
                    cmd.Parameters.Add("userID", MySqlDbType.Int64).Value = user.Id;
                    cmd.Parameters.Add("userName", MySqlDbType.VarChar).Value = user.Username;
                    cmd.Parameters.Add("userDiscriminator", MySqlDbType.VarChar).Value = user.Discriminator;
                    await cmd.ExecuteNonQueryAsync();
                    GuildsList[e.Guild.Id].GuildMembers.Add(user.Id, new Members
                    {
                        UserName = user.Username,
                        UserDiscriminator = user.Discriminator,
                        BdaySent = false
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex);
                    Console.WriteLine(ex.StackTrace);
                }
            }
            await connection.CloseAsync();
        }

        public async Task MemberAdd(GuildMemberAddEventArgs e)
        {
            await connection.OpenAsync();
            try
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = connection;
                cmd.CommandText = $"INSERT INTO `guilds.users` (`userID`, `userName`, `userDiscriminator`, `Birthdate`, `changeDate`) VALUES (?, ?, ?, NULL, CURRENT_TIMESTAMP) ON DUPLICATE KEY UPDATE userName=userName";
                cmd.Parameters.Add("userID", MySqlDbType.Int64).Value = e.Member.Id;
                cmd.Parameters.Add("userName", MySqlDbType.VarChar).Value = e.Member.Username;
                cmd.Parameters.Add("userDiscriminator", MySqlDbType.VarChar).Value = e.Member.Discriminator;
                await cmd.ExecuteNonQueryAsync();
                GuildsList[e.Guild.Id].GuildMembers.Add(e.Member.Id, new Members
                {
                    UserName = e.Member.Username,
                    UserDiscriminator = e.Member.Discriminator,
                    BdaySent = false
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
                Console.WriteLine(ex.StackTrace);
            }
            await connection.CloseAsync();
        }

        /*public async Task GreetUser(GuildMemberAddEventArgs e)
        {
            if (GuildsList[e.Guild.Id].ModuleConfig.GreetModule)
            {
                await e.Member.SendMessageAsync($"Welcome {e.Member.Mention}\n" +
                $"You succesfully landed on {e.Guild.Name} \n\n" +
                $"Please take a look into {e.Guild.GetChannel(GuildsList[pos].ChannelConfig.InfoChannelID).Mention} for informations regarding this server.\n" +
                $"To accept the rules ({e.Guild.GetChannel(GuildsList[pos].ChannelConfig.RuleChannelID).Mention}), please write `$accept-rules` in {e.Guild.GetChannel(GuildsList[pos].ChannelConfig.CmdChannelID).Mention}.\n" +
                $"You will automatically get assigned to the role *{e.Guild.GetRole(GuildsList[pos].ChannelConfig.RoleID).Name}*.\n\n" +
                $"{GuildsList[pos].ChannelConfig.CustomInfo}");
                await e.Guild.GetChannel(GuildsList[pos].ChannelConfig.LogChannelID).SendMessageAsync($"User {e.Member.Mention} was greeted.");
            }
        }*/

        /*public async Task ReactAdd(MessageReactionAddEventArgs e)
        {
            if (e.Message.Id == rolemsg && e.Emoji.Id == hcRoleEmote)
            {
                var GMember = await e.Channel.Guild.GetMemberAsync(e.User.Id);
                await GMember.GrantRoleAsync(e.Channel.Guild.GetRole(testGroup), "Role react");
            }
        }

        public async Task ReactRemove(MessageReactionRemoveEventArgs e)
        {
            if (e.Message.Id == rolemsg && e.Emoji.Id == hcRoleEmote)
            {
                var GMember = await e.Channel.Guild.GetMemberAsync(e.User.Id);
                await GMember.RevokeRoleAsync(e.Channel.Guild.GetRole(testGroup), "Role react");
            }
        }*/
    }

    public class Guilds
    {
        public string GuildName { get; set; }
        public string GuildDefaultInvite { get; set; }
        public ulong GuildOwner { get; set; }
        public Dictionary<ulong, Members> GuildMembers { get; set; }
        public ChannelConfig ChannelConfig { get; set; }
        public ModuleConfig ModuleConfig { get; set; }
    }

    public class Members
    {
        public string UserName { get; set; }
        public string UserDiscriminator { get; set; }
        public bool BdaySent { get; set; }
    }

    public class ChannelConfig
    {
        public ulong RuleChannelID { get; set; }
        public ulong InfoChannelID { get; set; }
        public ulong CmdChannelID { get; set; }
        public ulong LogChannelID { get; set; }
        public ulong RoleID { get; set; }
        public string CustomInfo { get; set; }
    }

    public class ModuleConfig
    {
        public bool AdminModule { get; set; }
        public bool GreetModule { get; set; }
        public bool BirthdayModule { get; set; }
        public List<GreetMessages> GreetMessages { get; set; }
    }

    public class GreetMessages
    {
        public string AnnounceString { get; set; }
    }
}
