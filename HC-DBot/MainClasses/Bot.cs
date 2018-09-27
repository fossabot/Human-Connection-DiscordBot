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

namespace HC_DBot.MainClasses
{
    class Bot : IDisposable
    {
        private static DiscordClient Client { get; set; }
        private CommandsNextExtension CNext;
        private InteractivityExtension INext;
        public static MySqlConnection connection;
        public static List<Guilds> GuildsList = new List<Guilds>();
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
                            var Guild = await Client.GetGuildAsync(GuildsList[GuildsList.FindIndex(x => x.GuildMembers.Any(y => y.UserID == user))].GuildID);
                            var DM = await Guild.GetMemberAsync(user);
                            GuildsList[GuildsList.FindIndex(x => x.GuildMembers.Any(y => y.UserID == user))].GuildMembers.Find(x => x.UserID == user).BdaySent = true;
                            var resetTrigger = ResetBday(GuildsList[GuildsList.FindIndex(x => x.GuildMembers.Any(y => y.UserID == user))].GuildMembers.Find(x => x.UserID == user));
                            resetTrigger.Wait(1000);
                            await DM.SendMessageAsync("Congrats!");
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
                    GuildsList.Add(new Guilds
                    {
                        GuildID = guild.Value.Id,
                        GuildName = guild.Value.Name,
                        GuildDefaultInvite = null,
                        GuildOwner = guild.Value.Owner.Id,
                        GuildMembers = new List<Members>(),
                        ChannelConfig = new ChannelConfig(),
                        ModuleConfig = new ModuleConfig()
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex);
                    Console.WriteLine(ex.StackTrace);
                } //Guild Top
                int pos = GuildsList.FindIndex(x => x.GuildID == guild.Value.Id);
                try
                {
                    MySqlCommand guildcmd = new MySqlCommand();
                    guildcmd.Connection = connection;
                    guildcmd.CommandText = $"INSERT INTO `guilds.config` (`guildID`, `ruleChannelID`, `infoChannelID`, `cmdChannelID`, `roleID`, `customInfo`) VALUES (?, 0, 0, 0, 0, ?) ON DUPLICATE KEY UPDATE ruleChannelID=ruleChannelID";
                    guildcmd.Parameters.Add("guildID", MySqlDbType.Int64).Value = guild.Value.Id;
                    guildcmd.Parameters.Add("customInfo", MySqlDbType.VarChar).Value = "to be filled";
                    await guildcmd.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex);
                    Console.WriteLine(ex.StackTrace);
                }
                try
                {
                    MySqlCommand guildcmd = new MySqlCommand();
                    guildcmd.Connection = connection;
                    guildcmd.CommandText = $"SELECT * FROM `guilds.config` WHERE `guildID` = {guild.Value.Id}";
                    var reader = await guildcmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        GuildsList[pos].ChannelConfig = (new ChannelConfig
                        {
                            RuleChannelID = Convert.ToUInt64(reader["ruleChannelID"]),
                            InfoChannelID = Convert.ToUInt64(reader["infoChannelID"]),
                            CmdChannelID = Convert.ToUInt64(reader["cmdChannelID"]),
                            RoleID = Convert.ToUInt64(reader["roleID"]),
                            CustomInfo = reader["customInfo"].ToString()
                        });
                    }
                    await connection.CloseAsync();
                    await connection.OpenAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex);
                    Console.WriteLine(ex.StackTrace);
                }//Guild Channel Config
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
                }
                try
                {
                    MySqlCommand guildcmd = new MySqlCommand();
                    guildcmd.Connection = connection;
                    guildcmd.CommandText = $"SELECT * FROM `modules.config` WHERE `guildID` = {guild.Value.Id}";
                    var reader = await guildcmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        GuildsList[pos].ModuleConfig = new ModuleConfig
                        {
                            AdminModule = Convert.ToBoolean(reader["adminModule"]),
                            GreetModule = Convert.ToBoolean(reader["greetModule"]),
                            BirthdayModule = Convert.ToBoolean(reader["birthdayModule"])
                        };
                    }
                    await connection.CloseAsync();
                    await connection.OpenAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex);
                    Console.WriteLine(ex.StackTrace);
                }//Guild Module Config
                try
                {
                    MySqlCommand guildcmd = new MySqlCommand();
                    guildcmd.Connection = connection;
                    guildcmd.CommandText = $"SELECT * FROM `modules.greet` WHERE `guildID` = {guild.Value.Id}";
                    var reader = await guildcmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        GuildsList[pos].ModuleConfig.GreetMessages.Add(new GreetMessages
                        {
                            AnnounceString = reader["announceString"].ToString()
                        });
                    }
                    await connection.CloseAsync();
                    await connection.OpenAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex);
                    Console.WriteLine(ex.StackTrace);
                }
                //I guess this will be handled manually for WI access
                /*foreach (var admin in guild.Value.Members.Where(x => x.Roles.Any(y => y.CheckPermission(DSharpPlus.Permissions.Administrator) == DSharpPlus.PermissionLevel.Allowed)))
                {
                    try
                    {
                        MySqlCommand guildcmd = new MySqlCommand();
                        guildcmd.Connection = connection;
                        guildcmd.CommandText = $"INSERT INTO `guilds.admins` (`guildID`, `userID`, `userName`, `userDiscriminator`, `userPassword`, `userEmail`, `userDiscordEmail`, `changeDate`) VALUES (?, ?, ?, ?, ?, ?, ?, CURRENT_TIMESTAMP) ON DUPLICATE KEY UPDATE userName = userName";
                        guildcmd.Parameters.Add("guildID", MySqlDbType.Int64).Value = guild.Value.Id;
                        guildcmd.Parameters.Add("userID", MySqlDbType.Int64).Value = admin.Id;
                        guildcmd.Parameters.Add("userName", MySqlDbType.VarChar).Value = admin.Username;
                        guildcmd.Parameters.Add("userDiscriminator", MySqlDbType.VarChar).Value = admin.Discriminator;
                        guildcmd.Parameters.Add("userPassword", MySqlDbType.VarChar).Value = "soon™";
                        guildcmd.Parameters.Add("userEmail", MySqlDbType.VarChar).Value = admin.Email;
                        guildcmd.Parameters.Add("userDiscordEmail", MySqlDbType.VarChar).Value = admin.Email;
                        await guildcmd.ExecuteNonQueryAsync();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + ex);
                        Console.WriteLine(ex.StackTrace);
                    }
                }*/
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
                        GuildsList[pos].GuildMembers.Add(new Members
                        {
                            UserID = user.Id,
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
                int pos = GuildsList.FindIndex(x => x.GuildID == e.Guild.Id);
                GuildsList[pos].GuildMembers.Add(new Members
                {
                    UserID = e.Member.Id,
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

        public async Task GreetUser(GuildMemberAddEventArgs e)
        {
            int pos = GuildsList.FindIndex(x => x.GuildID == e.Guild.Id);
            if (pos == -1) return;
            if (GuildsList[pos].ModuleConfig.GreetModule)
            {
                await e.Member.SendMessageAsync($"Welcome {e.Member.Mention}\n" +
                $"You succesfully landed on {e.Guild.Name} \n\n" +
                $"Please take a look into {e.Guild.GetChannel(GuildsList[pos].ChannelConfig.InfoChannelID).Mention} for informations regarding this server.\n" +
                $"To accept the rules ({e.Guild.GetChannel(GuildsList[pos].ChannelConfig.RuleChannelID).Mention}), please write `$accept-rules` in {e.Guild.GetChannel(GuildsList[pos].ChannelConfig.CmdChannelID).Mention}.\n" +
                $"You will automatically get assigned to the role *{e.Guild.GetRole(GuildsList[pos].ChannelConfig.RoleID).Name}*.\n\n" +
                $"{GuildsList[pos].ChannelConfig.CustomInfo}");
            }
        }

        /*
        public async Task ReactAdd(MessageReactionAddEventArgs e)
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
        }
        */
    }

    public class Guilds
    {
        public ulong GuildID { get; set; }
        public string GuildName { get; set; }
        public string GuildDefaultInvite { get; set; }
        public ulong GuildOwner { get; set; }
        public List<Members> GuildMembers { get; set; }
        public ChannelConfig ChannelConfig { get; set; }
        public ModuleConfig ModuleConfig { get; set; }
    }

    public class Members
    {
        public ulong UserID { get; set; }
        public string UserName { get; set; }
        public string UserDiscriminator { get; set; }
        public bool BdaySent { get; set; }
    }

    public class ChannelConfig
    {
        public ulong RuleChannelID { get; set; }
        public ulong InfoChannelID { get; set; }
        public ulong CmdChannelID { get; set; }
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
