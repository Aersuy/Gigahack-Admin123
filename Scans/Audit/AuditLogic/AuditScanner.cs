using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Scans.Audit.DataClasses;
using Scans.InternetExposure.ScanLogic;
using Scans.InternetExposure.DataClases;
using Scans.CVE.ScanLogic;
using Scans.CVE.DataClasses;
using Scans.Password.PasswordLogic;
using Scans.Password.DataClasses;

namespace Scans.Audit.AuditLogic
{
    public class AuditScanner
    {
        private readonly PortScanner portScanner;
        private readonly EmailAuthScanner emailAuthScanner;
        private readonly CVEScanner cveScanner;
        private readonly PasswordChangeScan passwordPolicyScanner;
        private readonly WebServerTLSPolicyScan webServerScanner;

        public AuditScanner()
        {
            portScanner = new PortScanner();
            emailAuthScanner = new EmailAuthScanner();
            cveScanner = new CVEScanner();
            passwordPolicyScanner = new PasswordChangeScan();
            webServerScanner = new WebServerTLSPolicyScan();
        }

        public async Task<AuditResult> PerformMiniAudit(string targetIP)
        {
            var result = new AuditResult
            {
                Target = targetIP,
                ScanTime = DateTime.Now
            };

            try
            {
                // Run all scans in parallel for efficiency
                var tasks = new List<Task>
                {
                    Task.Run(() => PerformPortScanAudit(result, targetIP)),
                    Task.Run(() => PerformEmailAuthAudit(result, targetIP)),
                    Task.Run(async () => await PerformCVEAudit(result, targetIP)),
                    Task.Run(async () => await PerformPasswordPolicyAudit(result, targetIP)),
                    Task.Run(() => PerformWebServerAudit(result, targetIP))
                };

                await Task.WhenAll(tasks);

                // Calculate overall compliance score
                CalculateOverallCompliance(result);

                // Generate recommendations
                GenerateOverallRecommendations(result);

                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = ex.Message;
                result.Errors.Add($"Audit failed: {ex.Message}");
            }

            return result;
        }

        private void PerformPortScanAudit(AuditResult result, string targetIP)
        {
            try
            {
                // Scan only important/security-relevant ports
                var importantPorts = new[] { 21, 22, 23, 25, 53, 80, 110, 143, 443, 993, 995, 3389, 5985, 5986, 8080, 8443 };
                var portResults = new List<PortScanItem>();
                
                foreach (var port in importantPorts)
                {
                    portResults.Add(portScanner.ScanPort(targetIP, port));
                }
                
                var openPorts = portResults.Where(p => p.open).ToList();

                // Network Security Category
                result.NetworkSecurity.Items.Add(new AuditItem
                {
                    Name = "Important Ports Scan",
                    Description = "Scan for open important/security-relevant ports",
                    Passed = openPorts.Count > 0,
                    Status = $"Found {openPorts.Count} open important ports",
                    Details = string.Join(", ", openPorts.Take(5).Select(p => $"{p.portNumber}({p.service})")),
                    Weight = 3
                });

                // Check for common security ports
                var securityPorts = new[] { 22, 23, 21, 25, 53, 80, 443, 3389, 5985, 5986 };
                var exposedSecurityPorts = openPorts.Where(p => securityPorts.Contains(p.portNumber)).ToList();

                result.NetworkSecurity.Items.Add(new AuditItem
                {
                    Name = "Security Port Exposure",
                    Description = "Check for exposed security-sensitive ports",
                    Passed = exposedSecurityPorts.Count == 0,
                    Status = exposedSecurityPorts.Count == 0 ? "No exposed security ports" : $"{exposedSecurityPorts.Count} security ports exposed",
                    Details = string.Join(", ", exposedSecurityPorts.Select(p => $"{p.portNumber}({p.service})")),
                    Weight = 4
                });

                // Check for unknown services
                var unknownServices = openPorts.Where(p => p.service == "Unknown").ToList();
                result.NetworkSecurity.Items.Add(new AuditItem
                {
                    Name = "Service Identification",
                    Description = "Identify all running services",
                    Passed = unknownServices.Count == 0,
                    Status = unknownServices.Count == 0 ? "All services identified" : $"{unknownServices.Count} unknown services",
                    Details = string.Join(", ", unknownServices.Select(p => $"Port {p.portNumber}")),
                    Weight = 2
                });

                CalculateCategoryScore(result.NetworkSecurity);
            }
            catch (Exception ex)
            {
                result.NetworkSecurity.Items.Add(new AuditItem
                {
                    Name = "Port Scan",
                    Description = "Port scanning failed",
                    Passed = false,
                    Status = "Failed",
                    Details = ex.Message,
                    Weight = 3
                });
            }
        }

        private void PerformEmailAuthAudit(AuditResult result, string targetIP)
        {
            try
            {
                var emailResult = emailAuthScanner.ScanEmailAuth(targetIP);

                // System Security Category
                result.SystemSecurity.Items.Add(new AuditItem
                {
                    Name = "Email Authentication",
                    Description = "Check email server authentication methods",
                    Passed = emailResult.SPF.RecordExists || emailResult.DKIM.Any(d => d.RecordExists) || emailResult.DMARC.RecordExists,
                    Status = "Email auth scan completed",
                    Details = $"SPF: {(emailResult.SPF.RecordExists ? "Found" : "Not found")}, DKIM: {(emailResult.DKIM.Any(d => d.RecordExists) ? "Found" : "Not found")}, DMARC: {(emailResult.DMARC.RecordExists ? "Found" : "Not found")}",
                    Weight = 3
                });

                // SPF Record Check
                result.SystemSecurity.Items.Add(new AuditItem
                {
                    Name = "SPF Record",
                    Description = "Check for SPF record presence",
                    Passed = emailResult.SPF.RecordExists,
                    Status = emailResult.SPF.RecordExists ? "SPF record found" : "No SPF record",
                    Details = emailResult.SPF.RecordExists ? emailResult.SPF.Record : "Not found",
                    Weight = 2
                });

                // DKIM Record Check
                result.SystemSecurity.Items.Add(new AuditItem
                {
                    Name = "DKIM Record",
                    Description = "Check for DKIM record presence",
                    Passed = emailResult.DKIM.Any(d => d.RecordExists),
                    Status = emailResult.DKIM.Any(d => d.RecordExists) ? "DKIM record found" : "No DKIM record",
                    Details = emailResult.DKIM.Any(d => d.RecordExists) ? $"Found {emailResult.DKIM.Count(d => d.RecordExists)} DKIM records" : "Not found",
                    Weight = 2
                });

                // DMARC Record Check
                result.SystemSecurity.Items.Add(new AuditItem
                {
                    Name = "DMARC Record",
                    Description = "Check for DMARC record presence",
                    Passed = emailResult.DMARC.RecordExists,
                    Status = emailResult.DMARC.RecordExists ? "DMARC record found" : "No DMARC record",
                    Details = emailResult.DMARC.RecordExists ? emailResult.DMARC.Record : "Not found",
                    Weight = 2
                });

                CalculateCategoryScore(result.SystemSecurity);
            }
            catch (Exception ex)
            {
                result.SystemSecurity.Items.Add(new AuditItem
                {
                    Name = "Email Authentication",
                    Description = "Email authentication scan failed",
                    Passed = false,
                    Status = "Failed",
                    Details = ex.Message,
                    Weight = 3
                });
            }
        }

        private async Task PerformCVEAudit(AuditResult result, string targetIP)
        {
            try
            {
                // First get port scan results for CVE analysis
                var portResults = portScanner.ScanCommonPorts(targetIP);
                var portScanResults = new List<PortScanResult> { new PortScanResult { Ports = portResults } };
                var cveResult = await cveScanner.SearchCVEsForPortScanResults(portScanResults);

                // Vulnerability Management Category
                result.VulnerabilityManagement.Items.Add(new AuditItem
                {
                    Name = "CVE Scan",
                    Description = "Scan for known vulnerabilities",
                    Passed = cveResult.Success,
                    Status = cveResult.Success ? $"Found {cveResult.TotalResults} CVEs" : "CVE scan failed",
                    Details = cveResult.Success ? $"Search query: {cveResult.SearchQuery}" : cveResult.ErrorMessage,
                    Weight = 4
                });

                if (cveResult.Success && cveResult.CVEs.Any())
                {
                    var criticalCVEs = cveResult.CVEs.Where(c => c.Severity == "CRITICAL").Count();
                    var highCVEs = cveResult.CVEs.Where(c => c.Severity == "HIGH").Count();
                    var mediumCVEs = cveResult.CVEs.Where(c => c.Severity == "MEDIUM").Count();

                    result.VulnerabilityManagement.Items.Add(new AuditItem
                    {
                        Name = "Critical Vulnerabilities",
                        Description = "Check for critical severity CVEs",
                        Passed = criticalCVEs == 0,
                        Status = criticalCVEs == 0 ? "No critical CVEs" : $"{criticalCVEs} critical CVEs found",
                        Details = criticalCVEs > 0 ? string.Join(", ", cveResult.CVEs.Where(c => c.Severity == "CRITICAL").Take(3).Select(c => c.CVEId)) : "None",
                        Weight = 5
                    });

                    result.VulnerabilityManagement.Items.Add(new AuditItem
                    {
                        Name = "High Severity Vulnerabilities",
                        Description = "Check for high severity CVEs",
                        Passed = highCVEs <= 5, // Allow up to 5 high severity CVEs
                        Status = highCVEs == 0 ? "No high severity CVEs" : $"{highCVEs} high severity CVEs found",
                        Details = highCVEs > 0 ? string.Join(", ", cveResult.CVEs.Where(c => c.Severity == "HIGH").Take(3).Select(c => c.CVEId)) : "None",
                        Weight = 3
                    });

                    result.VulnerabilityManagement.Items.Add(new AuditItem
                    {
                        Name = "Vulnerability Count",
                        Description = "Total number of vulnerabilities found",
                        Passed = cveResult.TotalResults <= 20, // Allow up to 20 total CVEs
                        Status = $"{cveResult.TotalResults} total vulnerabilities",
                        Details = $"Critical: {criticalCVEs}, High: {highCVEs}, Medium: {mediumCVEs}",
                        Weight = 2
                    });
                }
                else
                {
                    result.VulnerabilityManagement.Items.Add(new AuditItem
                    {
                        Name = "No Vulnerabilities Found",
                        Description = "No known vulnerabilities detected",
                        Passed = true,
                        Status = "No CVEs found",
                        Details = "System appears to be free of known vulnerabilities",
                        Weight = 3
                    });
                }

                CalculateCategoryScore(result.VulnerabilityManagement);
            }
            catch (Exception ex)
            {
                result.VulnerabilityManagement.Items.Add(new AuditItem
                {
                    Name = "CVE Scan",
                    Description = "CVE scanning failed",
                    Passed = false,
                    Status = "Failed",
                    Details = ex.Message,
                    Weight = 4
                });
            }
        }

        private async Task PerformPasswordPolicyAudit(AuditResult result, string targetIP)
        {
            try
            {
                var domainController = !string.IsNullOrEmpty(targetIP) && targetIP != "127.0.0.1" ? targetIP : null;
                var passwordResult = await passwordPolicyScanner.ScanPasswordPolicies(domainController);

                // Password Security Category
                result.PasswordSecurity.Items.Add(new AuditItem
                {
                    Name = "Password Policy Scan",
                    Description = "Scan password policy configuration",
                    Passed = passwordResult.Success,
                    Status = passwordResult.Success ? $"Score: {passwordResult.SecurityScore}/100" : "Password policy scan failed",
                    Details = passwordResult.Success ? $"Grade: {passwordResult.SecurityGrade}" : passwordResult.ErrorMessage,
                    Weight = 4
                });

                if (passwordResult.Success)
                {
                    // Password Complexity
                    result.PasswordSecurity.Items.Add(new AuditItem
                    {
                        Name = "Password Complexity",
                        Description = "Check password complexity requirements",
                        Passed = passwordResult.Complexity.Enabled && passwordResult.Complexity.MinimumLength >= 8,
                        Status = passwordResult.Complexity.Enabled ? $"Enabled (Min: {passwordResult.Complexity.MinimumLength} chars)" : "Disabled",
                        Details = passwordResult.Complexity.Enabled ? 
                            $"Uppercase: {passwordResult.Complexity.RequireUppercase}, Lowercase: {passwordResult.Complexity.RequireLowercase}, Numbers: {passwordResult.Complexity.RequireNumbers}, Special: {passwordResult.Complexity.RequireSpecialCharacters}" :
                            "Password complexity is disabled",
                        Weight = 3
                    });

                    // Password History
                    result.PasswordSecurity.Items.Add(new AuditItem
                    {
                        Name = "Password History",
                        Description = "Check password history enforcement",
                        Passed = passwordResult.History.Enabled && passwordResult.History.RememberedPasswords >= 12,
                        Status = passwordResult.History.Enabled ? $"Enabled ({passwordResult.History.RememberedPasswords} passwords)" : "Disabled",
                        Details = passwordResult.History.Enabled ? $"Remembered passwords: {passwordResult.History.RememberedPasswords}" : "Password history is disabled",
                        Weight = 2
                    });

                    // Password Age
                    result.PasswordSecurity.Items.Add(new AuditItem
                    {
                        Name = "Password Expiration",
                        Description = "Check password expiration policy",
                        Passed = passwordResult.Age.Enabled && passwordResult.Age.MaximumAge <= 90,
                        Status = passwordResult.Age.Enabled ? $"Enabled ({passwordResult.Age.MaximumAge} days)" : "Disabled",
                        Details = passwordResult.Age.Enabled ? $"Max age: {passwordResult.Age.MaximumAge} days, Min age: {passwordResult.Age.MinimumAge} days" : "Password expiration is disabled",
                        Weight = 2
                    });

                    // Account Lockout
                    result.PasswordSecurity.Items.Add(new AuditItem
                    {
                        Name = "Account Lockout",
                        Description = "Check account lockout policy",
                        Passed = passwordResult.Lockout.Enabled && passwordResult.Lockout.LockoutThreshold <= 5,
                        Status = passwordResult.Lockout.Enabled ? $"Enabled ({passwordResult.Lockout.LockoutThreshold} attempts)" : "Disabled",
                        Details = passwordResult.Lockout.Enabled ? $"Threshold: {passwordResult.Lockout.LockoutThreshold}, Duration: {passwordResult.Lockout.LockoutDuration} min" : "Account lockout is disabled",
                        Weight = 3
                    });
                }

                CalculateCategoryScore(result.PasswordSecurity);
            }
            catch (Exception ex)
            {
                result.PasswordSecurity.Items.Add(new AuditItem
                {
                    Name = "Password Policy",
                    Description = "Password policy scan failed",
                    Passed = false,
                    Status = "Failed",
                    Details = ex.Message,
                    Weight = 4
                });
            }
        }

        private void CalculateCategoryScore(SecurityCategory category)
        {
            if (!category.Items.Any())
            {
                category.Score = 0;
                category.Level = ComplianceLevel.Red;
                return;
            }

            var totalWeight = category.Items.Sum(item => item.Weight);
            var passedWeight = category.Items.Where(item => item.Passed).Sum(item => item.Weight);
            
            category.Score = totalWeight > 0 ? (int)((double)passedWeight / totalWeight * 100) : 0;
            category.Level = GetComplianceLevel(category.Score);
        }

        private void CalculateOverallCompliance(AuditResult result)
        {
            var categories = new[] { result.NetworkSecurity, result.SystemSecurity, result.VulnerabilityManagement, result.PasswordSecurity, result.WebSecurity };
            
            // Only include categories that have been scored (exclude Web Security if no web server detected)
            var scoredCategories = categories.Where(cat => cat.Score > 0 || cat.Name != "Web Security").ToArray();
            
            if (scoredCategories.Any())
            {
                var totalScore = scoredCategories.Sum(cat => cat.Score);
                result.OverallScore = totalScore / scoredCategories.Length;
            }
            else
            {
                result.OverallScore = 0;
            }
            
            result.OverallCompliance = GetComplianceLevel(result.OverallScore);
        }

        private ComplianceLevel GetComplianceLevel(int score)
        {
            return score switch
            {
                >= 71 => ComplianceLevel.Green,
                >= 41 => ComplianceLevel.Yellow,
                _ => ComplianceLevel.Red
            };
        }

        private void GenerateOverallRecommendations(AuditResult result)
        {
            // Network Security Recommendations
            if (result.NetworkSecurity.Level != ComplianceLevel.Green)
            {
                result.Recommendations.Add("🔒 Network Security: Close unnecessary ports and secure exposed services");
            }

            // System Security Recommendations
            if (result.SystemSecurity.Level != ComplianceLevel.Green)
            {
                result.Recommendations.Add("📧 Email Security: Implement SPF, DKIM, and DMARC records");
            }

            // Vulnerability Management Recommendations
            if (result.VulnerabilityManagement.Level != ComplianceLevel.Green)
            {
                result.Recommendations.Add("🛡️ Vulnerability Management: Patch critical and high-severity vulnerabilities");
            }

            // Password Security Recommendations
            if (result.PasswordSecurity.Level != ComplianceLevel.Green)
            {
                result.Recommendations.Add("🔑 Password Security: Strengthen password policies and enable account lockout");
            }

            // General Recommendations
            if (result.OverallCompliance == ComplianceLevel.Red)
            {
                result.Recommendations.Add("🚨 Overall: Critical security issues detected - immediate action required");
            }
            else if (result.OverallCompliance == ComplianceLevel.Yellow)
            {
                result.Recommendations.Add("⚠️ Overall: Security improvements needed - address identified issues");
            }
            else
            {
                result.Recommendations.Add("✅ Overall: Good security posture - maintain current practices");
            }
        }

        private void PerformWebServerAudit(AuditResult result, string targetIP)
        {
            try
            {
                // Check if web ports (80, 443) are open
                var webPorts = new[] { 80, 443 };
                var openWebPorts = new List<int>();
                
                foreach (var port in webPorts)
                {
                    var portResult = portScanner.ScanPort(targetIP, port);
                    if (portResult.open)
                    {
                        openWebPorts.Add(port);
                    }
                }

                if (!openWebPorts.Any())
                {
                    // No web ports open - not a web server
                    result.WebSecurity.Items.Add(new AuditItem
                    {
                        Name = "Web Server Detection",
                        Description = "Check if target is running a web server",
                        Passed = false,
                        Status = "No web server detected",
                        Details = "Ports 80 (HTTP) and 443 (HTTPS) are closed",
                        Weight = 0 // Set weight to 0 so it doesn't affect scoring
                    });
                    
                    result.WebSecurity.Recommendations.Add("Target does not appear to be running a web server");
                    // Don't calculate score for web security when no web server is detected
                    result.WebSecurity.Score = 100;
                    result.WebSecurity.Level = ComplianceLevel.Green; // Neutral level when not applicable
                    return;
                }

                // Web server detected - perform comprehensive scan
                result.WebSecurity.Items.Add(new AuditItem
                {
                    Name = "Web Server Detection",
                    Description = "Check if target is running a web server",
                    Passed = true,
                    Status = "Web server detected",
                    Details = $"Open web ports: {string.Join(", ", openWebPorts)}",
                    Weight = 1
                });

                // Scan each open web port
                foreach (var port in openWebPorts)
                {
                    try
                    {
                        var webResult = WebServerTLSPolicyScan.ScanWebServer(targetIP, port);
                        
                        // TLS/SSL Analysis
                        result.WebSecurity.Items.Add(new AuditItem
                        {
                            Name = $"TLS/SSL Analysis (Port {port})",
                            Description = "Analyze TLS/SSL configuration and security",
                            Passed = webResult.Tls12 || webResult.Tls13, // Good if TLS 1.2 or 1.3 supported
                            Status = GetTLSStatus(webResult),
                            Details = GetTLSDetails(webResult),
                            Weight = 3
                        });

                        // Certificate Analysis
                        result.WebSecurity.Items.Add(new AuditItem
                        {
                            Name = $"Certificate Analysis (Port {port})",
                            Description = "Analyze SSL/TLS certificate security",
                            Passed = IsCertificateValid(webResult),
                            Status = GetCertificateStatus(webResult),
                            Details = GetCertificateDetails(webResult),
                            Weight = 3
                        });

                        // Security Headers Analysis
                        result.WebSecurity.Items.Add(new AuditItem
                        {
                            Name = $"Security Headers (Port {port})",
                            Description = "Check for important security headers",
                            Passed = HasSecurityHeaders(webResult),
                            Status = GetSecurityHeadersStatus(webResult),
                            Details = GetSecurityHeadersDetails(webResult),
                            Weight = 2
                        });

                        // Add recommendations based on findings
                        AddWebSecurityRecommendations(result.WebSecurity, webResult);
                    }
                    catch (Exception ex)
                    {
                        result.WebSecurity.Items.Add(new AuditItem
                        {
                            Name = $"Web Server Scan (Port {port})",
                            Description = "Scan web server security configuration",
                            Passed = false,
                            Status = "Scan failed",
                            Details = $"Error: {ex.Message}",
                            Weight = 1
                        });
                    }
                }

                CalculateCategoryScore(result.WebSecurity);
            }
            catch (Exception ex)
            {
                result.WebSecurity.Errors.Add($"Web server audit failed: {ex.Message}");
                result.WebSecurity.Level = ComplianceLevel.Red;
            }
        }

        private string GetTLSStatus(WebServerScanResult webResult)
        {
            if (webResult.Tls13) return "TLS 1.3 supported (Excellent)";
            if (webResult.Tls12) return "TLS 1.2 supported (Good)";
            if (webResult.Tls11) return "TLS 1.1 supported (Weak)";
            if (webResult.Tls10) return "TLS 1.0 supported (Very Weak)";
            return "No TLS support detected";
        }

        private string GetTLSDetails(WebServerScanResult webResult)
        {
            var details = new List<string>();
            if (webResult.Tls10) details.Add("TLS 1.0: Supported");
            if (webResult.Tls11) details.Add("TLS 1.1: Supported");
            if (webResult.Tls12) details.Add("TLS 1.2: Supported");
            if (webResult.Tls13) details.Add("TLS 1.3: Supported");
            return string.Join(" | ", details);
        }

        private bool IsCertificateValid(WebServerScanResult webResult)
        {
            if (webResult.CertificateValidTo == DateTime.MinValue) return false;
            return webResult.CertificateValidTo > DateTime.Now;
        }

        private string GetCertificateStatus(WebServerScanResult webResult)
        {
            if (webResult.CertificateValidTo == DateTime.MinValue) return "No certificate found";
            if (webResult.CertificateValidTo <= DateTime.Now) return "Certificate expired";
            if (webResult.CertificateValidTo <= DateTime.Now.AddDays(30)) return "Certificate expires soon";
            return "Certificate valid";
        }

        private string GetCertificateDetails(WebServerScanResult webResult)
        {
            if (webResult.CertificateValidTo == DateTime.MinValue) return "No certificate information available";
            
            var daysUntilExpiry = (webResult.CertificateValidTo - DateTime.Now).Days;
            return $"Issuer: {webResult.CertificateIssuer} | Expires: {webResult.CertificateValidTo:yyyy-MM-dd} ({daysUntilExpiry} days)";
        }

        private bool HasSecurityHeaders(WebServerScanResult webResult)
        {
            return !string.IsNullOrEmpty(webResult.HSTS) || 
                   !string.IsNullOrEmpty(webResult.ContentSecurityPolicy) ||
                   !string.IsNullOrEmpty(webResult.XFrameOptions);
        }

        private string GetSecurityHeadersStatus(WebServerScanResult webResult)
        {
            var headers = new List<string>();
            if (!string.IsNullOrEmpty(webResult.HSTS)) headers.Add("HSTS");
            if (!string.IsNullOrEmpty(webResult.ContentSecurityPolicy)) headers.Add("CSP");
            if (!string.IsNullOrEmpty(webResult.XFrameOptions)) headers.Add("X-Frame-Options");
            if (!string.IsNullOrEmpty(webResult.XContentTypeOptions)) headers.Add("X-Content-Type-Options");
            if (!string.IsNullOrEmpty(webResult.ReferrerPolicy)) headers.Add("Referrer-Policy");
            
            return headers.Any() ? $"Security headers found: {string.Join(", ", headers)}" : "No security headers detected";
        }

        private string GetSecurityHeadersDetails(WebServerScanResult webResult)
        {
            var details = new List<string>();
            if (!string.IsNullOrEmpty(webResult.HSTS)) details.Add($"HSTS: {webResult.HSTS}");
            if (!string.IsNullOrEmpty(webResult.ContentSecurityPolicy)) details.Add($"CSP: {webResult.ContentSecurityPolicy}");
            if (!string.IsNullOrEmpty(webResult.XFrameOptions)) details.Add($"X-Frame-Options: {webResult.XFrameOptions}");
            if (!string.IsNullOrEmpty(webResult.XContentTypeOptions)) details.Add($"X-Content-Type-Options: {webResult.XContentTypeOptions}");
            if (!string.IsNullOrEmpty(webResult.ReferrerPolicy)) details.Add($"Referrer-Policy: {webResult.ReferrerPolicy}");
            
            return details.Any() ? string.Join(" | ", details) : "No security headers configured";
        }

        private void AddWebSecurityRecommendations(SecurityCategory webSecurity, WebServerScanResult webResult)
        {
            // TLS Recommendations
            if (!webResult.Tls12 && !webResult.Tls13)
            {
                webSecurity.Recommendations.Add("Enable TLS 1.2 or higher for secure communication");
            }
            if (webResult.Tls10 || webResult.Tls11)
            {
                webSecurity.Recommendations.Add("Disable TLS 1.0 and 1.1 as they are deprecated and insecure");
            }

            // Certificate Recommendations
            if (webResult.CertificateValidTo != DateTime.MinValue)
            {
                var daysUntilExpiry = (webResult.CertificateValidTo - DateTime.Now).Days;
                if (daysUntilExpiry <= 30)
                {
                    webSecurity.Recommendations.Add("Certificate expires soon - renew immediately");
                }
                else if (daysUntilExpiry <= 90)
                {
                    webSecurity.Recommendations.Add("Certificate expires in 90 days - plan renewal");
                }
            }

            // Security Headers Recommendations
            if (string.IsNullOrEmpty(webResult.HSTS))
            {
                webSecurity.Recommendations.Add("Implement HSTS (HTTP Strict Transport Security) header");
            }
            if (string.IsNullOrEmpty(webResult.ContentSecurityPolicy))
            {
                webSecurity.Recommendations.Add("Implement Content Security Policy (CSP) header");
            }
            if (string.IsNullOrEmpty(webResult.XFrameOptions))
            {
                webSecurity.Recommendations.Add("Implement X-Frame-Options header to prevent clickjacking");
            }
        }

        public void Dispose()
        {
            cveScanner?.Dispose();
        }
    }
}
