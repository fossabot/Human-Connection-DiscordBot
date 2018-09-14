using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using HumanConnection.DiscordBot;

namespace HumanConnection.DiscordBot
{
    public class Program
    {
        private static Mutex INSTANCE_MUTEX = new Mutex(true, "Peke Bot");
        public static HCBotGUI BOT_UI = new HCBotGUI();
        private static HCBotConsole BOT = new HCBotConsole();
        static void Main()
        {
            // Check if an instance is already running. Remove this block if you want to run multiple instances.
            if (!INSTANCE_MUTEX.WaitOne(TimeSpan.Zero, false))
            {
                MessageBox.Show("The applicaton is already running.");
                return;
            }
            // Start the UI.
            try { Application.Run(BOT_UI as Form); }
            catch { Console.WriteLine("Failed to run."); }
        }
        // Connect to the bot, or cancel before the connection happens.
        public static void Run() => Task.Run(() => BOT.RunAsync(BOT_UI.GetToken));
        public static void Cancel() => Task.Run(() => BOT.CancelAsync());
        public static void Stop() => Task.Run(() => BOT.StopAsync());
    }
}
