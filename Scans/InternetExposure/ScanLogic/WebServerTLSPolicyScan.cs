using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Scans.InternetExposure.DataClases;

namespace Scans.InternetExposure.ScanLogic
{
    public class WebServerTLSPolicyScan
    {
        public static WebServerScanResult ScanWebServer(string host, int port = 443)
        { 
        var results = new WebServerScanResult
        {
            Host = host,
            Port = port,
            
            // TLS Versions
            Tls10 = TestTLS(host, port, SslProtocols.Tls),
            Tls11 = TestTLS(host, port, SslProtocols.Tls11),
            Tls12 = TestTLS(host, port, SslProtocols.Tls12),
            Tls13 = TestTLS(host, port, SslProtocols.Tls13)
        };

        // Certificate
        try
        {
            var cert = GetCertificate(host, port);
            results.CertificateVersion = cert.Version;
            results.CertificateIssuer = cert.Issuer;
            results.CertificateSubject = cert.Subject;
            results.CertificateValidFrom = cert.NotBefore;
            results.CertificateValidTo = cert.NotAfter;
        }
        catch
        {
            results.CertificateVersion = 0;
            results.CertificateIssuer = "Error retrieving certificate";
            results.CertificateSubject = "";
        }

        // HSTS
        results.HSTS = CheckHSTS(host);
        return results;
        }
        public static bool  TestTLS(string host, int port, SslProtocols protocol)
        {
            try
            {
                using (var client = new TcpClient(host, port))
                using (var sslStream = new SslStream(client.GetStream(), false,
                    (sender, certificate, chain, sslPolicyErrors) => true))
                {
                    sslStream.AuthenticateAsClient(host, null, protocol, false);
                    return true; // Handshake succeeded
                }
            }
            catch
            {
                return false; // Failed handshake
            }
        }
        public static X509Certificate2 GetCertificate(string host, int port = 443)
        {
            using (var client = new TcpClient(host, port))
            using (var sslStream = new SslStream(client.GetStream(), false,
                (sender, certificate, chain, sslPolicyErrors) => true))
            {
                sslStream.AuthenticateAsClient(host);
                return new X509Certificate2(sslStream.RemoteCertificate);
            }
        }
        public static string CheckHSTS(string host)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(5);

                    var request = new HttpRequestMessage(HttpMethod.Get, "https://" + host);
                    var response = client.Send(request); // Synchronous

                    if (response.Headers.Contains("Strict-Transport-Security"))
                    {
                        return string.Join(";", response.Headers.GetValues("Strict-Transport-Security"));
                    }
                    else
                    {
                        return "Not Enabled";
                    }
                }
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }
    }
}
