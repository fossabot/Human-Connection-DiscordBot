using HC_DBot.MainClasses;
using Newtonsoft.Json;
using System.IO;

namespace HC_DBot
{
    class Program
    {
        public static Data config;

        static void Main(string[] args)
        {
            using (StreamReader r = new StreamReader("config.json"))
            {
                string json = r.ReadToEnd();
                config = JsonConvert.DeserializeObject<Data>(json);
            }
            using (var b = new Bot(config.Token))
            {
                b.RunAsync().Wait();
            }
        }
    }

    public class Data
    {
        public string Token { get; set; }
        public Modules Modules { get; set; }
    }
    public class Modules
    {
        public bool Admin { get; set; }
        public bool Greet { get; set; }
    }
}
