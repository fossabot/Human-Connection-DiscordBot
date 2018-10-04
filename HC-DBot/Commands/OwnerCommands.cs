using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Threading.Tasks;
using static HC_DBot.MainClasses.Bot;
using MySql.Data.MySqlClient;
using System;
using System.Net;
using System.IO;
using System.Linq;

namespace HC_DBot.Commands
{
    [Group("owner"), RequireOwner, RequirePrefixes("%")]
    class OwnerCommands : BaseCommandModule
    {
        public static void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[16 * 1024];
            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, read);
            }
        }

        [Command("shutdown"), RequireDirectMessage]
        public async Task BotShutdown(CommandContext ctx)
        {
            Console.WriteLine("shutdown request");
            await LogPrivate(ctx.Channel as DiscordDmChannel, "Shutdown Bot", "With this command the bot will be shut down", $"Shutting down {DiscordEmoji.FromGuildEmote(ctx.Client, 491234510659125271)}", DiscordColor.Orange);
            ShutdownRequest.Cancel();
        }

        [Command("statistic"), Aliases("stats"), RequireDirectMessage]
        public async Task Statistic(CommandContext ctx)
        {
            int servers = 0;
            int emotes = 0;
            int users = 0;
            await connection.OpenAsync();
            try
            {
                MySqlCommand cmd1 = new MySqlCommand
                {
                    Connection = connection,
                    CommandText = $"SELECT COUNT(*) FROM `guilds`"
                };
                MySqlCommand cmd2 = new MySqlCommand
                {
                    Connection = connection,
                    CommandText = $"SELECT COUNT(*) FROM `guilds.emotes`"
                };
                MySqlCommand cmd3 = new MySqlCommand
                {
                    Connection = connection,
                    CommandText = $"SELECT COUNT(*) FROM `guilds.users`"
                };
                servers = Convert.ToInt32(await cmd1.ExecuteScalarAsync());
                emotes = Convert.ToInt32(await cmd2.ExecuteScalarAsync());
                users = Convert.ToInt32(await cmd3.ExecuteScalarAsync());
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
                Console.WriteLine(ex.StackTrace);
            }
            await connection.CloseAsync();
            await LogPrivate(ctx.Channel as DiscordDmChannel, "Statistic", "Gets the statistic of the bot", $"Guilds: {servers} | Emotes: {emotes} | Users: {users}", DiscordColor.White);
        }

        [Command("transfer"), Aliases("copy", "cp"), Description("Transfer guild to guild"), RequirePermissions(Permissions.Administrator)]
        public async Task TransferGuild(CommandContext ctx, string pass, ulong guildId)
        {
            await ctx.Message.DeleteAsync();

            var source = ctx.Guild;
            var target = await ctx.Client.GetGuildAsync(guildId);

            var sourceName = source.Name;
            var targetName = target.Name;

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Request for copying {ctx.Guild.Name}. Checking pass!");
            if (pass == "Ratiasu7$Lala")
            {
                try
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Pass accepted.");
                    Console.WriteLine($"Copying {sourceName} to {targetName}");
                    DiscordMessage msg = await ctx.RespondAsync($"Pass accepted.");
                    await source.ModifyAsync(g => { g.Name = "Transfer in progress.."; g.AuditLogReason = "Transfer"; });
                    await target.ModifyAsync(g => { g.Name = "Transfer in progress.."; g.AuditLogReason = "Transfer"; });

                    await Task.Delay(1000);

                    // Clearing
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Voiding target");
                    Console.ForegroundColor = ConsoleColor.Green;
                    msg = await msg.ModifyAsync(msg.Content + $"\nVoiding **{targetName}**");
                    Console.WriteLine($"Voiding {target.Name}");
                    var TargetChannels = await target.GetChannelsAsync();
                    foreach (var chans in TargetChannels)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        if (chans.Type == ChannelType.Text)
                        {
                            Console.WriteLine($"Deleting text channel {chans.Name}");
                            await chans.DeleteAsync();
                        }
                        else if (chans.Type == ChannelType.Voice)
                        {
                            Console.WriteLine($"Deleting voice channel {chans.Name}");
                            await chans.DeleteAsync();
                        }
                        else if (chans.Type == ChannelType.Category)
                        {
                            Console.WriteLine($"Deleting category channel {chans.Name}");
                            await chans.DeleteAsync();
                        }
                        else if (chans.Type == ChannelType.Unknown)
                        {
                            Console.WriteLine($"Deleting unkown channel {chans.Name}");
                            await chans.DeleteAsync();
                        }
                        Console.ResetColor();
                    }
                    
                    await Task.Delay(1000);

                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine($"Getting all channels");
                    var SourceChannels = await source.GetChannelsAsync();

                    // Get Afk Channel
                    Console.WriteLine($"Getting afk channel");
                    DiscordChannel afkChannel = source.AfkChannel;
                    DiscordChannel newAfkChannel = null;

                    // Get System Channel
                    Console.WriteLine($"Getting system channel");
                    DiscordChannel systemChannel = source.SystemChannel;
                    DiscordChannel newSystemChannel = null;
                    
                    // Copy Channels
                    msg = await msg.ModifyAsync(msg.Content + $"\nCopying **{sourceName}** to **{targetName}**");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Copying Channels");
                    Console.ResetColor();
                    foreach (DiscordChannel chan in SourceChannels)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine($"Copying {chan.Name}");
                        DiscordChannel newChan = null;
                        switch (chan.Type)
                        {
                            case ChannelType.Category:
                                Console.WriteLine($"Creating category");
                                newChan = await target.CreateChannelCategoryAsync(chan.Name, null, $"Transfer of {chan.Name} without flags");
                                await newChan.ModifyAsync(ch =>
                                {
                                    ch.Topic = chan.Topic;
                                    ch.AuditLogReason = "Transfer topic";
                                });
                                Console.WriteLine($"Setting perms");
                                foreach (DiscordOverwrite overridePerm in chan.PermissionOverwrites)
                                {
                                    await newChan.AddOverwriteAsync(await overridePerm.GetMemberAsync(), overridePerm.Allowed, overridePerm.Denied, "Transfer flags");
                                }
                                break;
                            case ChannelType.Text:
                                Console.WriteLine($"Creating text channel");
                                newChan = await target.CreateTextChannelAsync(chan.Name, null, null, chan.IsNSFW, $"Transfer of {chan.Name} without flags");
                                await newChan.ModifyAsync(ch =>
                                {
                                    ch.Topic = chan.Topic;
                                    ch.AuditLogReason = "Transfer topic";
                                });
                                Console.WriteLine($"Setting perms");
                                foreach (DiscordOverwrite overridePerm in chan.PermissionOverwrites)
                                {
                                    await newChan.AddOverwriteAsync(await overridePerm.GetMemberAsync(), overridePerm.Allowed, overridePerm.Denied, "Transfer flags");
                                }
                                if (systemChannel != null)
                                {
                                    if (newChan.Name == systemChannel.Name)
                                    {
                                        Console.ForegroundColor = ConsoleColor.Gray;
                                        Console.WriteLine($"Found system channel. Saving this!");
                                        newSystemChannel = newChan;
                                    }
                                }
                                break;
                            case ChannelType.Voice:
                                Console.WriteLine($"Creating voice channel");
                                newChan = await target.CreateVoiceChannelAsync(chan.Name, null, chan.Bitrate, chan.UserLimit, null, $"Transfer of {chan.Name} without flags");
                                Console.WriteLine($"Setting perms");
                                foreach (DiscordOverwrite overridePerm in chan.PermissionOverwrites)
                                {
                                    await newChan.AddOverwriteAsync(await overridePerm.GetMemberAsync(), overridePerm.Allowed, overridePerm.Denied, "Transfer flags");
                                }
                                if (afkChannel != null)
                                {
                                    if (newChan.Name == afkChannel.Name)
                                    {
                                        Console.ForegroundColor = ConsoleColor.Gray;
                                        Console.WriteLine($"Found afk channel. Saving this!");
                                        newAfkChannel = newChan;
                                    }
                                }
                                break;
                        }
                        Console.WriteLine($"Copied {newChan.Name}");
                        Console.ResetColor();

                        await Task.Delay(1000);
                    }

                    // Sorting


                    // Setting Server settings
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Getting web stream");
                    WebRequest request = WebRequest.Create($"{source.IconUrl}");
                    WebResponse response = await request.GetResponseAsync();
                    Stream dataStream = response.GetResponseStream();
                    Console.WriteLine($"Making memory stream");
                    MemoryStream memoryStream = new MemoryStream();
                    Console.WriteLine($"Copying web stream to memory stream");
                    await dataStream.CopyToAsync(memoryStream);
                    Console.WriteLine($"Setting memory stream to pos 0");
                    memoryStream.Position = 0;
                    Console.ResetColor();

                    await Task.Delay(1000);

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Modifing guild config");
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    await target.ModifyAsync(g => {
                        Console.WriteLine($"Set mfa to {source.MfaLevel.ToString()}");
                        g.MfaLevel = source.MfaLevel;
                        Console.WriteLine($"Set Icon to {source.IconUrl}");
                        g.Icon = memoryStream;
                        Console.WriteLine($"Set name to {sourceName}");
                        g.Name = sourceName;
                        Console.WriteLine($"Set Voice region to {source.VoiceRegion.Name}");
                        g.Region = source.VoiceRegion;
                        if (newAfkChannel != null)
                        {
                            Console.WriteLine($"Set afk channel to {newAfkChannel.Name}");
                            g.AfkChannel = newAfkChannel;
                            Console.WriteLine($"Set afk zimeout to {source.AfkTimeout.ToString()}");
                            g.AfkTimeout = source.AfkTimeout;
                        }
                        if (newSystemChannel != null)
                        {
                            Console.WriteLine($"Set system channel to {newSystemChannel.Name}");
                            g.SystemChannel = newSystemChannel;
                        }
                        Console.WriteLine($"Set notification to {source.DefaultMessageNotifications.ToString()}");
                        g.DefaultMessageNotifications = source.DefaultMessageNotifications;
                        Console.WriteLine($"Set content filter to {source.ExplicitContentFilter.ToString()}");
                        g.ExplicitContentFilter = source.ExplicitContentFilter;
                        Console.WriteLine($"Set verification level to {source.VerificationLevel.ToString()}");
                        g.VerificationLevel = source.VerificationLevel;
                        Console.WriteLine($"Writing audit log");
                        g.AuditLogReason = $"Transfer of {sourceName} to {targetName}";
                    });
                    
                    await Task.Delay(1000);

                    await source.ModifyAsync(g => { g.Name = sourceName; g.AuditLogReason = "Transfer completed"; });
                    Console.ResetColor();
                    Console.ForegroundColor = ConsoleColor.Green;
                    await msg.ModifyAsync(msg.Content + $"\n**{sourceName}** is now copied to **{targetName}**. Done :heart:");
                    Console.WriteLine($"Done");
                    Console.ResetColor();
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine(ex.StackTrace);
                    Console.WriteLine(ex.Message);
                    Console.ResetColor();
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.Write("Pass denied. Abort.");
                Console.ResetColor();
                await ctx.RespondAsync($"Pass denied. Aborting copy {source.Name} to {target.Name}");
            }
            await Task.Delay(20);
        }

        [Command("void"), Description("Emptys guild"), RequirePermissions(Permissions.Administrator)]
        public async Task VoidGuild(CommandContext ctx, string pass, ulong guildId)
        {
            var guild = await ctx.Client.GetGuildAsync(guildId);

            Console.WriteLine($"Request for voiding {guild.Name}. Checking pass!");
            if (pass == "Ratiasu7$Lala")
            {
                await ctx.Message.DeleteAsync();
                await ctx.RespondAsync($"Pass accepted. Voiding {guild.Name}");
                Console.WriteLine($"Pass accepted. Voiding {guild.Name}");
                await Task.Delay(20);
                var channels = await guild.GetChannelsAsync();
                var defaultchannel = guild.GetDefaultChannel();
                foreach(var chan in channels)
                {
                    if (chan.Id != defaultchannel.Id)
                    {
                        if (chan.Type == ChannelType.Text)
                        {
                            Console.WriteLine($"Deleting text channel {chan.Name}");
                            await chan.DeleteAsync();
                        }
                        else if (chan.Type == ChannelType.Voice)
                        {
                            Console.WriteLine($"Deleting voice channel {chan.Name}");
                            await chan.DeleteAsync();
                        }
                        else if (chan.Type == ChannelType.Category)
                        {
                            Console.WriteLine($"Deleting category channel {chan.Name}");
                            await chan.DeleteAsync();
                        }
                        else if (chan.Type == ChannelType.Unknown)
                        {
                            Console.WriteLine($"Deleting unkown channel {chan.Name}");
                            await chan.DeleteAsync();
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Deleting default channel {chan.Name}");
                        await chan.DeleteAsync();
                    }
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.Write("Pass denied. Abort.");
                Console.ResetColor();
                return;
            }
        }
    }
}
