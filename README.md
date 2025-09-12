# Gigahack Port Scanner

A Windows Forms application for network port scanning built with C# and .NET 8.

## Features

- **Graphical User Interface**: Easy-to-use Windows Forms interface
- **Parallel Port Scanning**: Scans multiple ports simultaneously for faster results
- **Configurable Parameters**: 
  - Target IP address input
  - Maximum number of ports to scan (1-65535)
  - Real-time progress tracking
- **Real-time Results**: Live display of open and closed ports
- **Cancellation Support**: Ability to stop scans in progress
- **Error Handling**: Comprehensive error handling for network issues

## Usage

1. **Enter Target IP**: Input the IP address you want to scan (default: 127.0.0.1)
2. **Set Max Ports**: Choose how many ports to scan (default: 500)
3. **Start Scan**: Click "Start Scan" to begin the port scan
4. **View Results**: Watch as open ports are displayed in real-time
5. **Stop if Needed**: Use "Stop Scan" to cancel the operation

## Project Structure

- `Gigahack-Admin123/`: Main Windows Forms application
- `Scans/`: Core port scanning library
  - `InternetExposure/PortScan.cs`: Port scanner implementation
- `Reports/`: Reporting functionality (future enhancement)

## Technical Details

- **Framework**: .NET 8.0
- **UI**: Windows Forms
- **Networking**: System.Net.Sockets.TcpClient
- **Async/Await**: Full async support for non-blocking UI
- **Error Handling**: Comprehensive exception handling for various network conditions

## Building and Running

```bash
dotnet build
dotnet run --project Gigahack-Admin123
```

## Port Scanner Features

The `PortScanner` class provides:
- `IsPortOpenAsync()`: Check if a single port is open
- `ScanPortsWithNumbersAsync()`: Scan multiple ports and return results with port numbers
- `GetOpenPortsAsync()`: Get only the list of open ports
- `ScanWithDetailsAsync()`: Get detailed scan results including timing and statistics

## Error Handling

The scanner handles various network conditions:
- Connection refused (port closed)
- Connection timeout
- Host unreachable
- Network unreachable
- Invalid IP addresses
- Invalid port numbers