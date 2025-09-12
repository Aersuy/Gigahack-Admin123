using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace Scans.InternetExposure
{
    public class PortScanner
    {
        private int defaultMaxPorts = 500;
        private int defaultTimeoutMs = 1000;

        public List<PortScanItem> AllPortScan(string ip)
        {
            var ports = new List<PortScanItem>();
            for (int i = 0; i < defaultMaxPorts; i++)
            {
                ports.Add(ScanPort(ip, i));
            }
            return ports;

        }
        private string DetectService(int port, string banner)
        {
            // First try to detect by banner content
            string bannerLower = banner.ToLower();

            if (bannerLower.Contains("ssh"))
                return "SSH";
            if (bannerLower.Contains("ftp"))
                return "FTP";
            if (bannerLower.Contains("smtp"))
                return "SMTP";
            if (bannerLower.Contains("http"))
                return "HTTP";
            if (bannerLower.Contains("pop3"))
                return "POP3";
            if (bannerLower.Contains("imap"))
                return "IMAP";
            if (bannerLower.Contains("telnet"))
                return "Telnet";
            if (bannerLower.Contains("dns"))
                return "DNS";

            // If banner doesn't help, detect by port
            return DetectServiceByPort(port);
        }
        private string DetectServiceByPort(int port)
        {
            return port switch
            {
                21 => "FTP",
                22 => "SSH",
                23 => "Telnet",
                25 => "SMTP",
                53 => "DNS",
                80 => "HTTP",
                110 => "POP3",
                143 => "IMAP",
                443 => "HTTPS",
                993 => "IMAPS",
                995 => "POP3S",
                3389 => "RDP",
                5900 => "VNC",
                8080 => "HTTP-Alt",
                _ => "Unknown"
            };
        }
        public PortScanItem ScanPort(string ip,int port)
        {   var item = new PortScanItem();
            item.portNumber = port;
            using (TcpClient scan = new TcpClient())
            {
                try
                {
                    scan.Connect(ip, port);
                    item.open = true;
                    try
                    {
                        using (NetworkStream stream = scan.GetStream())
                        {
                            stream.ReadTimeout = 2000; // 2 seconds
                            stream.WriteTimeout = 2000; // 2 seconds

                            // Send a simple probe
                            byte[] probe = System.Text.Encoding.ASCII.GetBytes("\r\n");
                            stream.Write(probe, 0, probe.Length);

                            // Try to read response with timeout
                            byte[] buffer = new byte[1024];
                            int bytesRead = 0;

                            try
                            {
                                bytesRead = stream.Read(buffer, 0, buffer.Length);
                            }
                            catch (System.IO.IOException)
                            {
                                // Timeout occurred, which is normal for many services
                                bytesRead = 0;
                            }

                            if (bytesRead > 0)
                            {
                                item.banner = System.Text.Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim();
                                item.service = DetectService(item.portNumber, item.banner);
                            }
                            else
                            {
                                // No response, detect service by port only
                                item.service = DetectServiceByPort(port);
                            }
                        }
                    }
                    catch
                    {
                        // If banner grabbing fails, just detect by port
                        item.service = DetectServiceByPort(port);
                    }
                    return item;
                }
                catch
                {
                    item.open = false;
                    item.service = DetectServiceByPort(port);
                    return item;
                }
            }
        }

        public List<PortScanItem> ScanCommonPorts(string ip)
        {
            var ports = new List<PortScanItem>();
            // Common ports to scan
            int[] commonPorts = { 21, 22, 23, 25, 53, 80, 110, 143, 443, 993, 995, 3389, 5900, 8080 };
            
            foreach (int port in commonPorts)
            {
                ports.Add(ScanPort(ip, port));
            }
            return ports;
        }
    }
       

    public class ScanResult
    {
        public string TargetIP { get; set; } = string.Empty;
        public TimeSpan ScanDuration { get; set; }
        public int TotalPortsScanned { get; set; }
        public List<int> OpenPorts { get; set; } = new List<int>();
        public List<int> ClosedPorts { get; set; } = new List<int>();
        public bool ScanCompleted { get; set; }
    }
}

