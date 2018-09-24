using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static HC_DBot.GuildStatics;
using System.IO;
using Newtonsoft.Json;
using DSharpPlus.EventArgs;
using DSharpPlus;

namespace HC_DBot.Module
{
    class RoleModule
    {
        public ulong targetMessage = rolemsg;
        public RoleModule()
        {
            // Empty
        }

        public async Task ReactOnAdd(MessageReactionAddEventArgs messageReactionAdd, DiscordClient client)
        {
            if(messageReactionAdd.Message.Id == rolemsg)
            {
                if(messageReactionAdd.Emoji.Id == hcRoleEmote)
                {
                    DiscordUser user = messageReactionAdd.User;
                    DiscordGuild guild = await client.GetGuildAsync(hcGuildId);
                    DiscordMember member = await guild.GetMemberAsync(user.Id);
                    await member.GrantRoleAsync(guild.GetRole(testGroup), "Role react");
                }
            }
        }


        public async Task ReactOnRemove(MessageReactionRemoveEventArgs messageReactionRemove, DiscordClient client)
        {
            if (messageReactionRemove.Message.Id == rolemsg)
            {
                if (messageReactionRemove.Emoji.Id == hcRoleEmote)
                {
                    DiscordUser user = messageReactionRemove.User;
                    DiscordGuild guild = await client.GetGuildAsync(hcGuildId);
                    DiscordMember member = await guild.GetMemberAsync(user.Id);
                    await member.RevokeRoleAsync(guild.GetRole(testGroup), "Role react");
                }
            }
        }
    }
}