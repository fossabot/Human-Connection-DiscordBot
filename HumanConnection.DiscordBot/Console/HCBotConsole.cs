using Discord;
using Discord.WebSocket;
using Discord.Addons;
using Discord.Addons.PrefixService;
using Discord.Audio;
using Discord.Audio.Streams;
using Discord.Commands;
using Discord.Commands.Builders;
using Discord.Webhook;
using Discord.Rest;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Windows.Foundation;
using Windows.System;
using System.Diagnostics;
using Windows.Data.Xml.Dom;
using System.Security.Policy;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Text;
using Windows.UI.Notifications;
using HumanConnection.DiscordBot.Services;
using HumanConnection.DiscordBot.Modules;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using System.Linq;

namespace HumanConnection.DiscordBot
{
    public class HCBotConsole
    {
        private static bool visible = true;

        public static string ConnectionStatus = "Disconnected";

        public ulong hcGuildId = 443107904757694465;

        public string hcEmote = "<:hcwhite:491192363738333184>";
        public string botEmote = "<:bot:491234510659125271>";

        public string hcDeChannelMention = "<#443107905307410473>";
        public string hcEnChannelMention = "<#469161003511447572>";
        public string hcBotLogChannelMention = "<#490977974787768353>";
        public string hcBotRegelChannelMention = "<#490991963676344340>";

        public ulong hcMemberGroupId = 490613814916808718;

        public ulong hcDeChannelId = 443107905307410473;
        public ulong hcEnChannelId = 469161003511447572;
        public ulong hcBotLogChannelId = 490977974787768353;

        private DiscordSocketClient _client;
        //private CommandService _service;
        //private IServiceProvider _iservice;
        private ServerConfigService _serverconfig;
        private bool running = false;
        private static readonly bool DesktopNotify = true;

        public static bool GetDesktopNotifications() { return DesktopNotify; }

        #region Console
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool AllocConsole();

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

        public static bool ConsoleVisible()
        {
            return visible;
        }

        public static void ShowConsoleWindow()
        {
            var handle = GetConsoleWindow();

            if (handle == IntPtr.Zero)
            {
                AllocConsole();
            }
            else
            {
                ShowWindow(handle, SW_SHOW);
            }
            visible = true;
        }

        public static void HideConsoleWindow()
        {
            var handle = GetConsoleWindow();

            ShowWindow(handle, SW_HIDE);
            visible = false;
        }
        #endregion

        #region Run Async
        public async Task RunAsync(string token)
        {
            if (_client != null)
            {
                if (_client.ConnectionState == ConnectionState.Connecting || _client.ConnectionState == ConnectionState.Connected)
                    return;
            }

            await Log(new LogMessage(LogSeverity.Info, "RunAsync", "Creating new Discord-Client object."));
            _client = new DiscordSocketClient();
            _serverconfig = new ServerConfigService();
            //_service = new CommandService();
            //_iservice = InstallServices();
            running = false;
            
            SetConnectionStatus("Connecting", System.Drawing.Color.GreenYellow, 0);

            try
            {

                _client.Log += Log;
                _client.Ready += ReadyAsync;
                _client.MessageReceived += MessageReceivedAsync;
                //_client.ReactionAdded += ReactionAddedAsync;
                //_client.ReactionRemoved += ReactionRemovedAsync;
                _client.UserJoined += UserJoinedAsync;
                _client.UserLeft += LogUserLeaveAsync;

                await Log(new LogMessage(LogSeverity.Info, "RunAsync", "Starting"));
                await _client.LoginAsync(TokenType.Bot, token);

                await _client.StartAsync();
                await _client.SetGameAsync("$help");
                
                running = true;

                SetConnectionStatus("Connected", System.Drawing.Color.DarkGreen, 1);
            }
            catch
            {
                await Log(new LogMessage(LogSeverity.Error, "RunAsync", "Failed to connect."));

                SetConnectionStatus("Disconnected", System.Drawing.Color.MediumVioletRed, 2);
                Program.BOT_UI.ChangeLock(2);

                return;
            }

            Program.BOT_UI.ChangeLock(1);
            await Task.Delay(-1);
        }
        #endregion

        #region Cancel Async
        public async Task CancelAsync()
        {
            await Task.Delay(0);
        }
        #endregion

        #region Stop Async
        public async Task StopAsync()
        {
            if (_client.ConnectionState == ConnectionState.Connecting || _client.ConnectionState == ConnectionState.Connected)
            {
                SetConnectionStatus("Disconnecting..", System.Drawing.Color.Orange, 0);
                _client.LogoutAsync().Wait();
                _client.StopAsync().Wait();
                if (running) running = false;
                SetConnectionStatus("Disconnected", System.Drawing.Color.Red, 2);
                Program.BOT_UI.ChangeLock(2);
                await Task.Delay(0);
            } else
            {
                return;
            }
        }
        #endregion

        #region Ready Async
        private async Task ReadyAsync()
        {
            Console.WriteLine($"{_client.CurrentUser} is connected!");
            await LogReady();
        }
        #endregion

        #region User Join / Leave Asnyc
        private async Task UserJoinedAsync(SocketGuildUser guildUser)
        {
            var guild = _client.GetGuild(hcGuildId);
            //await SendGermanWelcomeMessage(guild.GetTextChannel(hcDeChannelId), guildUser, guild, hcEmote);
            //await sendEnglishWelcomeMessage(guild.GetTextChannel(hcEnChannelId), guildUser, guild, hcEmote);
            await LogUserJoin(guild.GetTextChannel(hcBotLogChannelId), guildUser, guild);
            //await testMsg(guild.GetTextChannel(hcBotLogChannelId), guildUser, guild, hcEmote);
            Console.WriteLine($"User {guildUser.Nickname} joined");
        }

        // Test Msg
        /*private async Task testMsg(SocketTextChannel channel, SocketGuildUser user, SocketGuild guild, String emote)
        {
            //
        }*/

        private async Task SendGermanWelcomeMessage(SocketTextChannel channel, SocketGuildUser user, IGuild guild, String emote)
        {
            await channel.SendMessageAsync($"Herzlich willkommen {user.Mention}\nDu bist auf dem Entwickler Discord von {guild.Name} gelandet :smile: \n\nSchau bitte in {hcBotLogChannelMention} für weiter Informationen, wie du mithelfen kannst.\nUm die Regeln ({hcBotRegelChannelMention}) zu akzeptieren, schreibe bitte `$accept-rules` in einen Channel deiner Wahl um die Rolle _{guild.GetRole(hcMemberGroupId).Name}_ zu bekommen.");
        }

        private async Task SendEnglishWelcomeMessage(SocketTextChannel channel, SocketGuildUser user, IGuild guild, String emote)
        {
            await channel.SendMessageAsync($"Welcome {user.Mention} on the developer discord by {guild.Name} {emote}");
        }
        #endregion

        #region Discord Logging
        private async Task LogUserJoin(SocketTextChannel channel, SocketGuildUser user, IGuild guild)
        {
            await channel.SendMessageAsync($"**Join**\n{user.Mention} ist dem Server beigetreten. ");// Eine Willkommensnachricht wurde sowohl in {hcDeChannelMention} als auch in {hcEnChannelMention} gesendet. User wird der Gruppe _{guild.GetRole(484463156219871232).Name}_ zugeteilt.");
            await user.AddRoleAsync(guild.GetRole(484463156219871232));
        }

        private async Task LogUserLeaveAsync(SocketGuildUser user)
        {
            SocketGuild guild = user.Guild;
            SocketTextChannel logChannel = guild.GetTextChannel(hcBotLogChannelId);
            await logChannel.SendMessageAsync($"**Leave**\n{user.Mention} ist vom Server gegangen.");
        }

        private async Task LogReady()
        {
            await _client.GetGuild(hcGuildId).GetTextChannel(hcBotLogChannelId).SendMessageAsync($"**Info**\nHC Control auf Posten.");
        }
        #endregion

        #region Message received Async - Commands
        private async Task MessageReceivedAsync(SocketMessage message)
        {

            if (message.Author.Id == _client.CurrentUser.Id)
                return;

            String nickname;
            if (message.Channel.Name.StartsWith("@"))
            {
                var chan = message.Channel as SocketDMChannel;
                var DM = chan.GetUser(_client.CurrentUser.Id);
                nickname = DM.Username;
            }
            else
            {
                var chan = message.Channel as SocketGuildChannel;
                var Guild = chan.Guild.Id;
                nickname = _client.GetGuild(Guild).GetUser(_client.CurrentUser.Id).Nickname;
            }

            String username = message.Author.Username;
            String channel = message.Channel.Name;
            String mention = message.Author.Mention;

            Console.WriteLine("Received message from " + username + " in " + channel + ": " + message.Content);

            if (message.Content.StartsWith("$ping"))
            {
                await message.Channel.TriggerTypingAsync();

                await message.Channel.SendMessageAsync(mention + ", Pong! :3");
            }
            else if (message.Content.StartsWith("!shutdown"))
            {
                SocketGuildUser guildUser = message.Author as SocketGuildUser;
                IUserMessage msg = (IUserMessage)await message.Channel.GetMessageAsync(message.Id);
                await msg.DeleteAsync();

                if (guildUser.GuildPermissions.Administrator)
                {
                    var guild = _client.GetGuild(hcGuildId);
                    var chan = guild.GetTextChannel(hcBotLogChannelId);

                    await chan.SendMessageAsync($"Shuting down {botEmote}");
                    await StopAsync();
                    Application.Exit();
                }
            }
            else if(message.Content.StartsWith("$accept-rules"))
            {
                SocketGuildUser guildUser = message.Author as SocketGuildUser;
                SocketGuild guild = ((SocketGuildChannel)message.Channel).Guild;

                IUserMessage msg = (IUserMessage)await message.Channel.GetMessageAsync(message.Id);
                IEmote emote = guild.Emotes.First(e => e.Name == "hcwhite");
                
                await msg.AddReactionAsync(emote);
                await guildUser.RemoveRoleAsync(guild.GetRole(484463156219871232));
                await guildUser.AddRoleAsync(guild.GetRole(hcMemberGroupId));
                await msg.DeleteAsync();
            }
            else if(message.Content.Contains("Human-Connection") || message.Content.Contains("HC") || message.Content.Contains("Human Connection") || message.Content.Contains("hc"))
            {
                SocketGuild guild = ((SocketGuildChannel)message.Channel).Guild;
                IUserMessage msg = (IUserMessage)await message.Channel.GetMessageAsync(message.Id);
                IEmote emote = guild.Emotes.First(e => e.Name == "hcwhite");

                await msg.AddReactionAsync(emote);
            }
            #region test failed
            /*
            else if(message.Content.StartsWith("$server update name"))
            {
                var newname = message.Content.Replace("^server update name ", "");
                var chan = message.Channel as SocketGuildChannel;
                var Guild = chan.Guild;
                var updateWorked = await _serverconfig.UpdateServerName(, newname);
                if (updateWorked)
                {
                    await message.Channel.SendMessageAsync($"{mention}, updated worked :3");
                }
                else
                {
                    await message.Channel.SendMessageAsync($"Oh no!!! {mention}, update failed..");
                }
            }*/
            #endregion
            else if (message.Content.StartsWith("$help"))
            {
                await message.Channel.TriggerTypingAsync();

                ulong authorId = message.Author.Id;

                SocketGuild guild = _client.GetGuild(hcGuildId);
                SocketGuildUser user = guild.GetUser(authorId);

                await message.Channel.TriggerTypingAsync();

                RestUserMessage rMsg = await message.Channel.SendMessageAsync(mention + " here is my help page " + botEmote);

                await message.Channel.TriggerTypingAsync();

                var embedFooter = new EmbedFooterBuilder();
                embedFooter.WithText($"©2018 Lala Sabathil | {Application.ProductName}");

                var builder = new EmbedBuilder();
                builder.WithTitle("Commands of HC Control");
                builder.WithThumbnailUrl("https://cdn.pbrd.co/images/HEjzSg5.png");
                builder.WithImageUrl("https://cdn.pbrd.co/images/HEjzvIZ.png");
                builder.WithDescription($"This are the commands for the **HC Control**. The prefix is **$**");
                builder.AddField("$accept-rules", "Accept the rules of this server");
                builder.AddField("$author", "Information about the author");
                builder.AddField("$gpdr", "GPDR of this Server - Not implemented");
                builder.AddField("$help", "This help");
                builder.AddField("$info", "Info about the server - Not implemented");
                builder.AddField("$ping", "Returns a friendly \"Pong\"");
                builder.WithFooter(embedFooter);
                builder.WithColor(Color.Blue);

                await message.Channel.SendMessageAsync("", false, builder);
                
                if (user.GuildPermissions.Administrator)
                {
                    RestUserMessage rAMsg = await message.Channel.SendMessageAsync(mention + " please look my pm for admin commands " + botEmote);

                    var embedAuthor = new EmbedAuthorBuilder();
                    embedAuthor.WithName("HC Bot");
                    embedAuthor.WithUrl("https://github.com/Lulalaby/Human-Connection-DiscordBot/");

                    var embedAdminFooter = new EmbedFooterBuilder();
                    embedAdminFooter.WithText($"©2018 Lala Sabathil | {Application.ProductName}");

                    var adminBuilder = new EmbedBuilder();
                    adminBuilder.WithAuthor(embedAuthor);
                    adminBuilder.WithTitle("Admin commands of HC Control - Not implemented right now");
                    adminBuilder.WithThumbnailUrl("https://cdn.pbrd.co/images/HEjzSg5.png");
                    adminBuilder.WithImageUrl("https://cdn.pbrd.co/images/HEjzvIZ.png");
                    adminBuilder.WithDescription($"This are the admin commands for the **HC Control**.\nSince you have Administrator Privilege, you get this message.\nAdmin prefix is **!**");
                    adminBuilder.AddField("!ban <usermention> <reason>", "Ban the mentioned *user* with *reason*");
                    adminBuilder.AddField("!info", "Info about the server for admins");
                    adminBuilder.AddField("!kick <usermention> <reason>", "Kick the mentioned *user* with *reason*");
                    adminBuilder.AddField("!role-add <usermention> <rolename>", "Add *user* to *role*");
                    adminBuilder.AddField("!role-remove <usermention> <rolename>", "Remove *user* from *role*");
                    adminBuilder.AddField("!warn <usermention> <message>", "Warn mentioned *user* with *message*");
                    adminBuilder.WithFooter(embedAdminFooter);
                    adminBuilder.WithColor(Color.DarkRed);
                    
                    await message.Author.SendMessageAsync("", false, adminBuilder);

                    await Task.Delay(2000);
                    await rAMsg.DeleteAsync();
                }
            }
            else if(message.Content.StartsWith("$author"))
            {
                IUserMessage msg = (IUserMessage)await message.Channel.GetMessageAsync(message.Id);

                SocketGuildUser authorUser = _client.GetGuild(hcGuildId).Users.First(a => a.Nickname == "Lala");

                var embedFooter = new EmbedFooterBuilder();
                embedFooter.WithText($"©2018 Lala Sabathil | {Application.ProductName}");

                RestInvite invite = await _client.GetInviteAsync("THZue3w");
                String inviteUrl = invite.Url;

                var builder = new EmbedBuilder();
                builder.WithTitle("Author contact data of HC Control");
                builder.WithThumbnailUrl("https://cdn.pbrd.co/images/HEjzSg5.png");
                builder.WithImageUrl("https://cdn.pbrd.co/images/HEjzvIZ.png");
                builder.WithDescription("The author of the HC Control is Lala Sabathil");
                builder.AddField("Discord server", inviteUrl);
                builder.AddField("Discord user", authorUser.Mention);
                builder.AddField("Facebook", "https://www.facebook.com/LalaDeviChan");
                builder.AddField("Twitter", "https://twitter.com/Lala_devi_chan");
                builder.AddField("Mail", "admin@latias.eu");
                builder.AddField("Telegram", "https://telegram.me/Lulalaby");
                builder.WithUrl("https://www.latias.eu");
                builder.WithFooter(embedFooter);
                builder.WithColor(Color.DarkBlue);

                await message.Author.SendMessageAsync("", false, builder);
                await msg.DeleteAsync();
            }
            else if (message.Content.StartsWith("$") || message.Content.StartsWith("!"))
            {
                IUserMessage msg = (IUserMessage)await message.Channel.GetMessageAsync(message.Id);

                await message.Channel.TriggerTypingAsync();
                RestUserMessage rMsg = await message.Channel.SendMessageAsync($"{mention} \"{message.Content}\" command is not implemented :(");

                await Task.Delay(5000);
                await msg.DeleteAsync();
                await rMsg.DeleteAsync();
            }
        }
        #endregion

        #region Logging
        private async Task Log(LogMessage msg)
        {
            Program.BOT_UI.AppendConsole(msg.Message);
            await Task.Delay(0);
        }
        #endregion

        private void SetConnectionStatus(string s, System.Drawing.Color color, int Lock, Exception arg = null)
        {
            /*string tokenRaw = "30 82 02 0a 02 82 02 01 00 ca 50 92 61 40 89 ff c8 b9 5b 45 69 f2 28 e6 5c 07 51 f7 75 3a 66 6b 86 45 53 cb b6 82 06 98 ce 60 0f f0 34 32 1d 3d c9 f0 cc e8 74 1c 15 2a e5 ca 6c 6d 3d 40 bf 80 2b e8 63 ee 84 5f ea f5 ff f6 d5 ce f7 59 d8 aa cd bc 80 8c fa 5c 5a c1 95 2a eb 36 25 74 d3 59 e9 56 d4 cd 99 e2 34 b7 89 59 a4 23 ea 9c bf cd b3 e9 48 19 6b 20 da e8 b9 bc 8c 8c e4 6c ba f6 53 c7 5d 7c ab 67 4f 0a 44 99 9e 84 15 9e 1c 9b 43 ac 84 91 66 a2 35 66 3e d4 f4 c5 9b 0c 6b ad 76 45 2a b5 5b 06 ce ab 3d 79 23 1d a0 00 e8 0f bb e4 41 60 c5 ff 21 8f b7 af 10 03 60 69 fb 12 05 c6 b2 0a bd be ef 9f 99 b8 0a 29 2b 15 83 45 27 1f 14 16 48 46 7d cc 12 98 1c 88 48 85 3a c6 7d c6 40 06 91 29 cd ab 5a 25 1d 1a 37 49 3b b0 a1 b9 d5 6b 0d 42 e3 6f d9 78 32 00 5c d4 f8 c0 a1 51 e9 02 a0 53 68 1a 1e ea e7 69 81 5d a6 d9 48 2b 7d e9 13 e0 6e 48 17 a8 c2 35 ab 1c c5 87 c1 35 36 04 aa 40 e0 74 24 66 a4 66 69 8a aa fd 96 27 31 fb d3 30 72 bd 74 ca ad dd 3d b8 72 de c2 93 5c 37 e8 80 4a 6a 1a 1b 7b 8d 6a 6c a5 80 e7 62 68 28 36 17 fd 00 68 ba ee 30 e7 e0 6e 8e 85 51 59 3e f0 fb 7e 7c d1 d6 a2 ee e0 0b b8 9d 03 a9 a3 2e 48 3c 7b 1a 5e e8 26 29 67 8c 79 7d 14 94 73 6f cc 66 4a 97 c2 3f 08 95 ef 46 51 94 e2 ad df 8d 23 fd 08 fa a3 9f b1 1f ac f9 a3 c5 23 8a ee e8 98 28 7a f4 aa 3a 91 2d a5 77 d3 bf b8 7c 43 e9 27 af 57 d9 39 39 49 2a 86 68 ac 8d d1 29 40 4b 7e 8a 8f 60 c5 36 ef 97 69 88 91 7c ab 92 80 d4 64 6f f3 50 87 14 6c a7 86 64 84 a0 e6 01 05 0e a4 b9 3a 7a 77 b4 09 9b ea 2b 03 71 35 56 43 b7 ab f0 d7 f2 c5 2f 69 87 9b 3e 61 0a 7c 47 12 a9 d1 60 da cd b7 02 03 01 00 01";
            byte[] keyToken = Encoding.ASCII.GetBytes(tokenRaw);
            ApplicationId id = new ApplicationId(keyToken, "Peke Bot", Version.Parse(Application.ProductVersion), "x64", Application.CurrentCulture.Name);*/

            /*string title = "Peke Bot - Status";
            string content = status;
            string image = "https://img.bb-official.com/PekeBotApp.jpg";*/

            ConnectionStatus = s;
            if (arg != null) Console.WriteLine(arg);
            Program.BOT_UI.SetConnectionStatus(s, color);
            if (Lock == 0)
            {
                Program.BOT_UI.ChangeLock(Lock);
            }
        }

        private IServiceProvider InstallServices()
        {
            ServiceCollection services = new ServiceCollection();

            // Add all additional services here.
            //services.AddSingleton<AdminService>(); // AdminModule : AdminService
            //services.AddSingleton<AudioService>(); // AudioModule : AudioService
            //services.AddSingleton<ChatService>(); // ChatModule : ChatService

            // Return the service provider.
            return services.BuildServiceProvider();
        }

        private async Task InstallCommands()
        {
            // Before we install commands, we should check if everything was set up properly. Check if logged in.
            if (_client.LoginState != LoginState.LoggedIn) return;

            // Hook the MessageReceived Event into our Command Handler
            _client.MessageReceived += MessageReceivedAsync;

            // Add tasks to send Messages, and userJoined to appropriate places
            _client.Ready += ReadyAsync;
            /*_client.UserJoined += UserJoined;
            _client.UserLeft += UserLeft;
            _client.Connected += Connected;
            _client.Disconnected += Disconnected;*/
            _client.Log += Log;

            // Discover all of the commands in this assembly and load them.
            //await _service.AddModulesAsync(Assembly.GetEntryAssembly());
            await Task.Delay(-1);
        }
    }
}