using HC_DBot.MainClasses;
using System;

namespace HC_DBot
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Console.ReadKey();
            using (var b = new Bot("NDg3OTEwODYzNjk3NzM5Nzk3.Dn3dUQ.nmyHskPUyQ0kBqM_gpUBEbd_wGQ"))
            {
                b.RunAsync().Wait();
            }
        }
    }
}
