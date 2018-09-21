using Discord;
using Discord.WebSocket;
using HumanConnection.DiscordBot.GUI;
using HumanConnection.DiscordBot.NativeConsole;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HumanConnection.DiscordBot
{
    public class Program
    {
        private static Mutex INSTANCE_MUTEX = new Mutex(true, "HC Control");

        public static HCBotGUI BOT_UI = new HCBotGUI();
        private static HCBotConsole BOT = new HCBotConsole();

        static void Main()
        {
            if (!INSTANCE_MUTEX.WaitOne(TimeSpan.Zero, false))
            {
                MessageBox.Show("The applicaton is already running.");
                return;
            }

            try {
                Application.Run(BOT_UI as Form);
            }
            catch {
                Console.WriteLine("Failed to run.");
            }
        }

        public static void Run() => Task.Run(() => BOT.RunAsync(BOT_UI.GetToken));
        public static void Cancel() => Task.Run(() => BOT.CancelAsync());
        public static void Stop() => Task.Run((Func<Task>)(() => BOT.StopAsync()));
        public static void Log(Discord.LogMessage log) => Task.Run(() => BOT.Log(log));
        public static void DelMsg(ISocketMessageChannel channel, ulong id) => Task.Run(() => BOT.DeleteMsgById(channel, id));
    }
}
