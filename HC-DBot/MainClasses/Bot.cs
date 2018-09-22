using DSharpPlus;
using DSharpPlus.EventArgs;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using System.Threading.Tasks;
using System;
using static HC_DBot.GuildStatics;
using System.Threading;
using DSharpPlus.Entities;
using MySql.Data.MySqlClient;

namespace HC_DBot.MainClasses
{
    class Bot : IDisposable
    {
        private DiscordClient Client { get; }
        private CommandsNextExtension CNext;
        private InteractivityExtension INext;
        public static MySqlConnection connection = new MySqlConnection("SERVER=meek.moe;DATABASE=HC-DBot_;UID=HD-DBot-User;PASSWORD=idontknow;SsLMode=none");

        public Bot(string Token)
        {
            ShutdownRequest = new CancellationTokenSource();
            Console.WriteLine(connection.ServerVersion);
            var cfg = new DiscordConfiguration
            {
                Token = Token,                
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                LogLevel = LogLevel.Debug,
                UseInternalLogHandler = true
            };
            Client = new DiscordClient(cfg);
            //Client.GuildMemberAdded += JoinMSG;
            Client.GuildDownloadCompleted += AddUsers;
            CNext = this.Client.UseCommandsNext(new CommandsNextConfiguration {
                StringPrefixes = new string[] { "$", "!" },
                EnableDefaultHelp = false
            });
            CNext.RegisterCommands<Commands.UserCommands>();
            CNext.RegisterCommands<Commands.AdminCommands>();
            CNext.RegisterCommands<Commands.UserConfig>();
            INext = this.Client.UseInteractivity(new InteractivityConfiguration { });
        }

        public static async Task JoinMSG(GuildMemberAddEventArgs e)
        {
            await e.Guild.GetChannel(hcDeChannelId).SendMessageAsync($"Herzlich willkommen {e.Member.Mention}\n" +
                $"Du bist auf dem Entwickler Discord von {e.Member.Username} gelandet :smile: \n\n" +
                $"Schau bitte in {e.Guild.GetChannel(hcBotLogChannelId).Mention} für weiter Informationen, wie du mithelfen kannst.\n" +
                $"Um die Regeln ({e.Guild.GetChannel(hcBotRegelChannelId).Mention}) zu akzeptieren, schreibe bitte `$accept-rules` in einen Channel deiner Wahl um die Rolle _{e.Guild.GetRole(hcMemberGroupId).Name}_ zu bekommen.");
            await e.Guild.GetChannel(hcEnChannelId).SendMessageAsync($"Welcome {e.Member.Mention} on the developer discord by {e.Guild.Name} {DiscordEmoji.FromGuildEmote(e.Client, hcEmote)}");
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
                await connection.OpenAsync();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = connection;
                cmd.CommandText = $"INSERT INTO `DiscordUsers` (DiscordId, Birthdate, AnsweredQuestions) "
                                                 + $" values (?, null, null) "
                                                 + $" ON DUPLICATE KEY UPDATE DiscordId=DiscordId";
                cmd.Parameters.Add("DiscordId", MySqlDbType.Int64).Value = Convert.ToInt64(e.Member.Id);
                await cmd.ExecuteNonQueryAsync();
                await connection.CloseAsync();
            }
            catch (Exception ey)
            {
                Console.WriteLine("Error: " + ey);
                Console.WriteLine(ey.StackTrace);
            }
        }

        public async Task AddUsers(GuildDownloadCompletedEventArgs e)
        {
            foreach (var guild in e.Guilds) //my test bot was in 2 guilds is i did this, should still work with just one
            {
                foreach (var user in guild.Value.Members)
                {
                    try
                    {
                        await connection.OpenAsync();
                        MySqlCommand cmd = new MySqlCommand();
                        cmd.Connection = connection;
                        cmd.CommandText = $"INSERT INTO `DiscordUsersBeta` (DiscordId, Birthdate, AnsweredQuestions) "
                                                         + $" values (?, null, null) "
                                                         + $" ON DUPLICATE KEY UPDATE DiscordId=DiscordId";
                        cmd.Parameters.Add("DiscordId", MySqlDbType.Int64).Value = Convert.ToInt64(user.Id);
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
