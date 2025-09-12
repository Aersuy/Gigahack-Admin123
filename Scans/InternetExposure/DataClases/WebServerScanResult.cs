using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Scans.InternetExposure.DataClases
{
    public class WebServerScanResult
    {
        public X509Certificate2 TLSCertificate {  get; set; }
        public string Host {  get; set; }
        public int Port { get; set; }
        public bool Tls10 { get; set; }
        public bool Tls11 { get; set; }
        public bool Tls12 { get; set; }
        public bool Tls13 { get; set; }

        public int CertificateVersion { get; set; }
        public string CertificateIssuer { get; set; }
        public string CertificateSubject { get; set; }
        public DateTime CertificateValidFrom { get; set; }
        public DateTime CertificateValidTo { get; set; }

        public string HSTS { get; set; }


    }
    
}
