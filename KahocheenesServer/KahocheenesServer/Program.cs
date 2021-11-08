using System;

namespace KahocheenesServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Kahocheenes Server";
            Server.Start(5046, 5047);
            Console.ReadKey();
        }
    }
}