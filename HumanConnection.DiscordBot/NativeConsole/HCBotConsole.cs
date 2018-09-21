#region Using directives
using Discord;
using Discord.Rest;
using Discord.Webhook;
using Discord.WebSocket;
using HumanConnection.DiscordBot.Modules;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.System;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Notifications;
using System.Windows.Forms;
#endregion

namespace HumanConnection.DiscordBot.NativeConsole
{
    public class HCBotConsole
    {
        #region Entrypoint - do not change
        public HCBotConsole()
        {
            HideConsoleWindow();
        }
        #endregion

        #region Global variables
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
        private bool running = false;
        private static readonly bool DesktopNotify = true;

        public static bool GetDesktopNotifications() { return DesktopNotify; }
        #endregion

        #region Module config - new layout
        public static AdminCommandModule adminCommand;

        public static bool AdminModuleEnabled = false;
        public static bool GreetModuleEnabled = false;
        public static bool ActivityModuleEnabled = false;
        public static bool BirthdayModuleEnabled = false;
        public static bool GuidanceModuleEnabled = false;

        public static string GetAdminModuleText()
        {
            var text = String.Empty;
            if(AdminModuleEnabled)
            {
                text = "Admin" + Environment.NewLine + Environment.NewLine + "enabled";
            } else
            {
                text = "Admin" + Environment.NewLine + Environment.NewLine + "disabled";
            }
            return text;
        }

        public static string GetGreetModuleText()
        {
            var text = String.Empty;
            if (GreetModuleEnabled)
            {
                text = "Greet" + Environment.NewLine + Environment.NewLine + "enabled";
            }
            else
            {
                text = "Greet" + Environment.NewLine + Environment.NewLine + "disabled";
            }
            return text;
        }
        
        public static string GetActivityModuleText()
        {
            var text = String.Empty;
            if (ActivityModuleEnabled)
            {
                text = "Activity" + Environment.NewLine + Environment.NewLine + "enabled";
            }
            else
            {
                text = "Activity" + Environment.NewLine + Environment.NewLine + "disabled";
            }
            return text;
        }
        
        public static string GetBirthdayModuleText()
        {
            var text = String.Empty;
            if (BirthdayModuleEnabled)
            {
                text = "Birthday" + Environment.NewLine + Environment.NewLine + "enabled";
            }
            else
            {
                text = "Birthday" + Environment.NewLine + Environment.NewLine + "disabled";
            }
            return text;
        }

        public static string GetGuidanceModuleText()
        {
            var text = String.Empty;
            if (GuidanceModuleEnabled)
            {
                text = "Guidance" + Environment.NewLine + Environment.NewLine + "enabled";
            }
            else
            {
                text = "Guidance" + Environment.NewLine + Environment.NewLine + "disabled";
            }
            return text;
        }
        #endregion

        #region Console - new layout
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

        #region Run Async - new layout
        public async Task RunAsync(string token)
        {
            if (_client != null)
            {
                if (_client.ConnectionState == ConnectionState.Connecting || _client.ConnectionState == ConnectionState.Connected)
                    return;
            }

            await Log(new LogMessage(LogSeverity.Info, "RunAsync", "Creating new Discord-Client object."));
            Program.Log(new LogMessage(LogSeverity.Info, "Admin Module", "Admin modul: " + AdminModuleEnabled.ToString()));
            Program.Log(new LogMessage(LogSeverity.Info, "Greet Module", "Greet modul: " + GreetModuleEnabled.ToString()));
            Program.Log(new LogMessage(LogSeverity.Info, "Guidance Module", "Guidance modul: " + GuidanceModuleEnabled.ToString()));
            _client = new DiscordSocketClient();
            running = false;
            SetConnectionStatus("Connecting", System.Drawing.Color.GreenYellow, 0);

            try
            {

                _client.Log += Log;
                _client.Ready += ReadyAsync;
                _client.MessageReceived += MessageHandle;
                /*_client.UserJoined += UserJoinedAsync;
                _client.UserLeft += LogUserLeaveAsync;*/

                await Log(new LogMessage(LogSeverity.Info, "RunAsync", "Starting"));
                
                await _client.LoginAsync(TokenType.Bot, token);

                await _client.StartAsync();
                await _client.SetStatusAsync(UserStatus.Online);
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

        #region Stop Async - new layout
        public async Task StopAsync()
        {
            if (_client.ConnectionState == ConnectionState.Connecting || _client.ConnectionState == ConnectionState.Connected)
            {
                SetConnectionStatus("Disconnecting..", System.Drawing.Color.Orange, 0);
                await _client.SetGameAsync("Offline right now");
                await _client.SetStatusAsync(UserStatus.Offline);
                _client.LogoutAsync().Wait();
                _client.StopAsync().Wait();
                if (running) running = false;
                SetConnectionStatus("Disconnected", System.Drawing.Color.Red, 2);
                Program.BOT_UI.ChangeLock(2);
                await Task.Delay(0);
            }
            else
            {
                return;
            }
        }
        #endregion

        #region Ready Async - new layout
        private async Task ReadyAsync()
        {
            Console.WriteLine($"{_client.CurrentUser} is connected!");
            if (AdminModuleEnabled)
            {
                adminCommand = new AdminCommandModule(_client, _client.GetGuild(hcGuildId).GetTextChannel(hcBotLogChannelId));
            }
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

        #region Message handle - new layout
        private async Task MessageHandle(SocketMessage msg)
        {
            if(msg.Content.StartsWith("!"))
            {
                await adminCommand.HandleMessage(msg);
            }
        }
        #endregion

        #region Discord Logging
        private async Task LogUserJoin(SocketTextChannel channel, SocketGuildUser user, IGuild guild)
        {
            await channel.SendMessageAsync($"**Join**\n{user.Mention} ({user.Username}) ist dem Server beigetreten. ");// Eine Willkommensnachricht wurde sowohl in {hcDeChannelMention} als auch in {hcEnChannelMention} gesendet. User wird der Gruppe _{guild.GetRole(484463156219871232).Name}_ zugeteilt.");
            await user.AddRoleAsync(guild.GetRole(484463156219871232));
        }

        private async Task LogUserLeaveAsync(SocketGuildUser user)
        {
            SocketGuild guild = user.Guild;
            SocketTextChannel logChannel = guild.GetTextChannel(hcBotLogChannelId);
            await logChannel.SendMessageAsync($"**Leave**\n{user.Mention} ( {user.Username} | {user.Nickname} ) ist vom Server gegangen.");
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

            // Fun command
            if (message.Content.StartsWith("$ping"))
            {
                await message.Channel.TriggerTypingAsync();

                await message.Channel.SendMessageAsync(mention + ", Pong! :3");
            }
            // Shutdown command (admin required)
            else if (message.Content.StartsWith("!shutdown"))
            {
                await DeleteMsgById(message.Channel, message.Id);

                if (IsAdmin(message))
                {
                    var guild = _client.GetGuild(hcGuildId);
                    var chan = guild.GetTextChannel(hcBotLogChannelId);

                    await chan.SendMessageAsync($"Shuting down {botEmote}");
                    Application.Exit();
                }
            }
            // Command to accept rules.
            else if (message.Content.StartsWith("$accept-rules"))
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
            else if (message.Content.Contains("Human-Connection") || message.Content.Contains("HC") || message.Content.Contains("Human Connection") || message.Content.Contains("hc"))
            {
                SocketGuild guild = ((SocketGuildChannel)message.Channel).Guild;
                IUserMessage msg = (IUserMessage)await message.Channel.GetMessageAsync(message.Id);
                IEmote emote = guild.Emotes.First(e => e.Name == "hcwhite");

                await msg.AddReactionAsync(emote);
            }
            /*
            #region test failed
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
            }
            #endregion
            */
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

                if (IsAdmin(message))
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
                    adminBuilder.AddField("!shutdown", "Stop's the bot and exits the application on bot side");
                    adminBuilder.AddField("!warn <usermention> <message>", "Warn mentioned *user* with *message*");
                    adminBuilder.WithFooter(embedAdminFooter);
                    adminBuilder.WithColor(Color.DarkRed);

                    await message.Author.SendMessageAsync("", false, adminBuilder);

                    await Task.Delay(2000);
                    await rAMsg.DeleteAsync();
                }
            }
            else if (message.Content.StartsWith("$author"))
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
            }/*
            else if (message.Content.StartsWith("$") || message.Content.StartsWith("!"))
            {
                IUserMessage msg = (IUserMessage)await message.Channel.GetMessageAsync(message.Id);

                await message.Channel.TriggerTypingAsync();
                RestUserMessage rMsg = await message.Channel.SendMessageAsync($"{mention} \"{message.Content}\" command is not implemented :(");

                await Task.Delay(5000);
                await msg.DeleteAsync();
                await rMsg.DeleteAsync();
            }*/
        }
        #endregion
        
        #region Logging
        /// <summary>
        /// 
        /// </summary>Log's given LogMessage to GUI Console
        /// <param name="msg">LogMessage</param>
        /// <returns></returns>
        public async Task Log(LogMessage msg)
        {
            Program.BOT_UI.AppendConsole(msg.Message);
            await Task.Delay(0);
        }
        #endregion

        #region Connection status update - new layout
        /// <summary>
        /// Set's the status of the connection in the GUI
        /// </summary>
        /// <param name="s">string</param>
        /// <param name="color">System.Drawing.Color</param>
        /// <param name="Lock">int</param>
        /// <param name="arg">Exception</param>
        private void SetConnectionStatus(string s, System.Drawing.Color color, int Lock, Exception arg = null)
        {
            ConnectionStatus = s;
            if (arg != null) Console.WriteLine(arg);
            Program.BOT_UI.SetConnectionStatus(s, color);
            if (Lock == 0)
            {
                Program.BOT_UI.ChangeLock(Lock);
            }
        }
        #endregion

        #region Admin check - new layout
        /// <summary>
        /// Checks if author is admin
        /// </summary>
        /// <param name="msg">SocketMessage</param>
        /// <returns>bool</returns>
        private bool IsAdmin(SocketMessage msg)
        {
            SocketGuildUser guildUser = msg.Author as SocketGuildUser;

            if (guildUser.GuildPermissions.Administrator)
            {
                return true;
            }
            return false;
        }
        #endregion

        #region Message actions - new layout
        /// <summary>
        /// Deletes the given message in the given channel, by id
        /// </summary>
        /// <param name="channel">ISocketMessageChannel</param>
        /// <param name="msgId">ulonb</param>
        public async Task DeleteMsgById(ISocketMessageChannel channel, ulong msgId)
        {
            IUserMessage message = (IUserMessage)await channel.GetMessageAsync(msgId);
            await message.DeleteAsync();
        }
        #endregion
    }
}