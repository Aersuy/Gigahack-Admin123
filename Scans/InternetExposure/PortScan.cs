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

        // Events for progress reporting
        public event Action<int, bool>? PortScanned; // port, isOpen
        public event Action<int, int>? ProgressUpdated; // completed, total
        public event Action<string>? StatusChanged; // status message
        public async Task<bool> IsPortOpenAsync(string ipAddress, int port, int timeoutMs = 1000)
        {
            if (string.IsNullOrWhiteSpace(ipAddress))
                throw new ArgumentException("IP address cannot be null or empty", nameof(ipAddress));
            
            if (port < 0 || port > 65535)
                throw new ArgumentOutOfRangeException(nameof(port), "Port must be between 0 and 65535");

            using (TcpClient client = new TcpClient())
            {
                try
                {
                    var connectTask = client.ConnectAsync(ipAddress, port);
                    var timeoutTask = Task.Delay(timeoutMs);
                    
                    var completedTask = await Task.WhenAny(connectTask, timeoutTask);
                    
                    if (completedTask == connectTask)
                    {
                        return client.Connected;
                    }
                    else
                    {
                        return false; // Timeout
                    }
                }
                catch (SocketException ex) when (ex.SocketErrorCode == SocketError.ConnectionRefused)
                {
                    return false; // Port is closed
                }
                catch (SocketException ex) when (ex.SocketErrorCode == SocketError.TimedOut)
                {
                    return false; // Connection timed out
                }
                catch (SocketException ex) when (ex.SocketErrorCode == SocketError.HostUnreachable)
                {
                    throw new InvalidOperationException($"Host {ipAddress} is unreachable", ex);
                }
                catch (SocketException ex) when (ex.SocketErrorCode == SocketError.NetworkUnreachable)
                {
                    throw new InvalidOperationException($"Network is unreachable", ex);
                }
                catch (Exception ex) when (!(ex is SocketException))
                {
                    throw new InvalidOperationException($"Unexpected error occurred while scanning port {port}", ex);
                }
            }
        }
        public async Task<List<bool>> ScanPortsAsync(string ipAddress, int maxPorts = 0, int timeoutMs = 0)
        {
            if (maxPorts <= 0) maxPorts = defaultMaxPorts;
            if (timeoutMs <= 0) timeoutMs = defaultTimeoutMs;
            
            var ports = new List<bool>();
            var tasks = new List<Task<bool>>();
            
            for (int i = 0; i < maxPorts; i++)
            {
                tasks.Add(IsPortOpenAsync(ipAddress, i, timeoutMs));
            }

            var results = await Task.WhenAll(tasks);
            ports.AddRange(results);
            
            return ports;
        }

        public async Task<Dictionary<int, bool>> ScanPortsWithNumbersAsync(string ipAddress, int maxPorts = 0, int timeoutMs = 0)
        {
            if (maxPorts <= 0) maxPorts = defaultMaxPorts;
            if (timeoutMs <= 0) timeoutMs = defaultTimeoutMs;
            
            var portResults = new Dictionary<int, bool>();
            var tasks = new List<Task<(int port, bool isOpen)>>();
            
            for (int i = 0; i < maxPorts; i++)
            {
                int port = i; // Capture the loop variable
                tasks.Add(ScanSinglePortAsync(ipAddress, port, timeoutMs));
            }

            var results = await Task.WhenAll(tasks);
            foreach (var result in results)
            {
                portResults[result.port] = result.isOpen;
            }
            
            return portResults;
        }

        private async Task<(int port, bool isOpen)> ScanSinglePortAsync(string ipAddress, int port, int timeoutMs = 0)
        {
            if (timeoutMs <= 0) timeoutMs = defaultTimeoutMs;
            bool isOpen = await IsPortOpenAsync(ipAddress, port, timeoutMs);
            return (port, isOpen);
        }

        public async Task<List<int>> GetOpenPortsAsync(string ipAddress, int maxPorts = 0, int timeoutMs = 0)
        {
            var results = await ScanPortsWithNumbersAsync(ipAddress, maxPorts, timeoutMs);
            return results.Where(kvp => kvp.Value).Select(kvp => kvp.Key).ToList();
        }

        public async Task<ScanResult> ScanWithDetailsAsync(string ipAddress, int maxPorts = 0, int timeoutMs = 0)
        {
            var startTime = DateTime.Now;
            var results = await ScanPortsWithNumbersAsync(ipAddress, maxPorts, timeoutMs);
            var endTime = DateTime.Now;
            
            var openPorts = results.Where(kvp => kvp.Value).Select(kvp => kvp.Key).ToList();
            var closedPorts = results.Where(kvp => !kvp.Value).Select(kvp => kvp.Key).ToList();
            
            return new ScanResult
            {
                TargetIP = ipAddress,
                ScanDuration = endTime - startTime,
                TotalPortsScanned = results.Count,
                OpenPorts = openPorts,
                ClosedPorts = closedPorts,
                ScanCompleted = true
            };
        }

        public async Task ScanWithProgressAsync(string ipAddress, int maxPorts = 0, int timeoutMs = 0, CancellationToken cancellationToken = default)
        {
            if (maxPorts <= 0) maxPorts = defaultMaxPorts;
            if (timeoutMs <= 0) timeoutMs = defaultTimeoutMs;

            StatusChanged?.Invoke("Starting scan...");
            
            var tasks = new List<Task<(int port, bool isOpen)>>();
            
            // Create all scan tasks
            for (int port = 0; port < maxPorts; port++)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                int currentPort = port; // Capture the loop variable
                tasks.Add(ScanSinglePortWithProgressAsync(ipAddress, currentPort, timeoutMs, cancellationToken));
            }

            int completedTasks = 0;
            var allTasks = tasks.ToArray();

            StatusChanged?.Invoke("Scanning ports...");

            // Process completed tasks as they finish
            while (completedTasks < allTasks.Length && !cancellationToken.IsCancellationRequested)
            {
                var completedTask = await Task.WhenAny(allTasks);
                var result = await completedTask;
                
                // Report progress
                PortScanned?.Invoke(result.port, result.isOpen);
                ProgressUpdated?.Invoke(completedTasks + 1, allTasks.Length);
                
                completedTasks++;
            }

            if (cancellationToken.IsCancellationRequested)
            {
                StatusChanged?.Invoke("Scan cancelled");
            }
            else
            {
                StatusChanged?.Invoke("Scan completed");
            }
        }

        private async Task<(int port, bool isOpen)> ScanSinglePortWithProgressAsync(string ipAddress, int port, int timeoutMs, CancellationToken cancellationToken)
        {
            try
            {
                bool isOpen = await IsPortOpenAsync(ipAddress, port, timeoutMs);
                return (port, isOpen);
            }
            catch (OperationCanceledException)
            {
                return (port, false);
            }
            catch (Exception)
            {
                return (port, false);
            }
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

