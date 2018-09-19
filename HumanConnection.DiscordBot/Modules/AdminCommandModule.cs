using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanConnection.DiscordBot.Modules
{
    class AdminCommandModule
    {
        public AdminCommandModule()
        {
            Program.Log(new Discord.LogMessage(Discord.LogSeverity.Info, "AdminCommandModule", "Admin module initialized"));
        }
    }
}
