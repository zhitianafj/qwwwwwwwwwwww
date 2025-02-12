using System;
using System.Threading;

namespace SimpleC2.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Client started...");
            ClientSocket.ConnectToServer();
            while (true)
            {
                Thread.Sleep(1000);
            }
        }
    }
}