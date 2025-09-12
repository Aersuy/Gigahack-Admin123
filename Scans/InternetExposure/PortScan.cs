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

        public List<bool> AllPortScan(string ip)
        {
            var ports = new List<bool>();
            for (int i = 0; i < defaultMaxPorts; i++)
            {
                using (TcpClient scan = new TcpClient())
                {
                    try
                    {
                        scan.Connect(ip, i);
                        ports.Add(scan.Connected);
                    } catch 
                    {
                        ports.Add(false);
                    }
                }
            }
            return ports;

        }
        public bool ScanPort(string ip,int port)
        {
            using (TcpClient scan = new TcpClient())
            {
                try
                {
                    scan.Connect(ip, port);
                    return scan.Connected;
                }
                catch
                {
                    return false;
                }
            }
        }

        public List<bool> ScanCommonPorts(string ip)
        {
            var ports = new List<bool>();
            // Common ports to scan
            int[] commonPorts = { 21, 22, 23, 25, 53, 80, 110, 143, 443, 993, 995, 3389, 5900, 8080 };
            
            foreach (int port in commonPorts)
            {
                using (TcpClient scan = new TcpClient())
                {
                    try
                    {
                        scan.Connect(ip, port);
                        ports.Add(scan.Connected);
                    }
                    catch
                    {
                        ports.Add(false);
                    }
                }
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

