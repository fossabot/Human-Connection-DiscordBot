using HC_DBot.MainClasses;
using Newtonsoft.Json;
using System;
using System.IO;

namespace HC_DBot
{
    class Program
    {
        private static Data config;

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

    class Data
    {
        [JsonProperty("Token")]
        public string Token { get; set; }
        //Added it as class cause maybe some more config stuff gets added!
    }
}
