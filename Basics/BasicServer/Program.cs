using System;

namespace BasicServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = new ConsoleKeyInfo();
            Console.WriteLine("Press Enter for server to start receiving messages...");
            Console.ReadLine();

            var server = new Server();

            Console.WriteLine("Press 'x' to stop server...");

            while (input.KeyChar != 'x')
            {
                input = Console.ReadKey();
            }

            server.Dispose();
        }
    }
}
