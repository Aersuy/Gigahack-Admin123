using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using Scans.InternetExposure.DataClases;

namespace Scans.InternetExposure.ScanLogic
{
    public class EmailAuthScanner
    {
        public EmailAuthResult ScanEmailAuth(string domain)
        {
            var result = new EmailAuthResult
            {
                Domain = domain
            };
            
            // Scan SPF
            result.SPF = ScanSPF(domain);
            
            // Scan DKIM (check common selectors)
            result.DKIM = ScanDKIM(domain);
            
            // Scan DMARC
            result.DMARC = ScanDMARC(domain);
            
            // Calculate security score
            result.SecurityScore = CalculateSecurityScore(result);
            result.SecurityGrade = CalculateSecurityGrade(result.SecurityScore);
            
            return result;
        }
        
        private SPFResult ScanSPF(string domain)
        {
            var spfResult = new SPFResult();
            
            try
            {
                // Query TXT records synchronously
                var txtRecords = GetTxtRecords(domain);
                var spfRecord = txtRecords.FirstOrDefault(record => record.StartsWith("v=spf1"));
                
                if (spfRecord != null)
                {
                    spfResult.RecordExists = true;
                    spfResult.Record = spfRecord;
                    spfResult.IsValid = ValidateSPFRecord(spfRecord, spfResult);
                }
            }
            catch (Exception ex)
            {
                spfResult.Errors.Add($"DNS lookup failed: {ex.Message}");
            }
            
            return spfResult;
        }
        
        private bool ValidateSPFRecord(string record, SPFResult result)
        {
            try
            {
                // Check if record starts with v=spf1
                if (!record.StartsWith("v=spf1"))
                {
                    result.Errors.Add("SPF record must start with 'v=spf1'");
                    return false;
                }
                
                // Parse mechanisms
                var parts = record.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                result.Qualifier = parts[0].Substring(6); // Extract qualifier after v=spf1
                
                for (int i = 1; i < parts.Length; i++)
                {
                    var mechanism = parts[i];
                    
                    if (mechanism.StartsWith("include:"))
                    {
                        result.Includes.Add(mechanism.Substring(8));
                    }
                    else if (mechanism.StartsWith("a") || mechanism.StartsWith("mx") || 
                             mechanism.StartsWith("ip4:") || mechanism.StartsWith("ip6:"))
                    {
                        result.Mechanisms.Add(mechanism);
                    }
                    else if (mechanism == "all")
                    {
                        result.Mechanisms.Add(mechanism);
                    }
                }
                
                // Check for common issues
                if (result.Mechanisms.Count == 0)
                {
                    result.Warnings.Add("No mechanisms found in SPF record");
                }
                
                if (result.Includes.Count > 10)
                {
                    result.Warnings.Add("Too many include mechanisms (may cause DNS lookup issues)");
                }
                
                return result.Errors.Count == 0;
            }
            catch (Exception ex)
            {
                result.Errors.Add($"SPF parsing error: {ex.Message}");
                return false;
            }
        }
        
        private List<DKIMResult> ScanDKIM(string domain)
        {
            var dkimResults = new List<DKIMResult>();
            
            // Common DKIM selectors
            var commonSelectors = new[]
            {
                "default", "mail", "email", "dkim", "selector1", "selector2",
                "k1", "k2", "google", "microsoft", "outlook", "yahoo",
                "s2048", "s1024", "protonmail", "zoho", "amazonses",
                "key1", "key2", "key3", "krs", "krs1", "krs2"
            };
            
            foreach (var selector in commonSelectors)
            {
                var dkimResult = ScanDKIMSelector(domain, selector);
                if (dkimResult.RecordExists)
                {
                    dkimResults.Add(dkimResult);
                }
            }
            
            return dkimResults;
        }
        
        private DKIMResult ScanDKIMSelector(string domain, string selector)
        {
            var dkimResult = new DKIMResult
            {
                Selector = selector,
                Domain = domain
            };
            
            try
            {
                var dkimDomain = $"{selector}._domainkey.{domain}";
                var txtRecords = GetTxtRecords(dkimDomain);
                var dkimRecord = txtRecords.FirstOrDefault(record => record.StartsWith("v=DKIM1"));
                
                if (dkimRecord != null)
                {
                    dkimResult.RecordExists = true;
                    dkimResult.PublicKey = ExtractDKIMPublicKey(dkimRecord);
                    dkimResult.KeyLength = CalculateKeyLength(dkimResult.PublicKey);
                    dkimResult.KeyType = DetermineKeyType(dkimResult.PublicKey);
                    dkimResult.Algorithm = ExtractDKIMAlgorithm(dkimRecord);
                    dkimResult.HashAlgorithm = ExtractDKIMHashAlgorithm(dkimRecord);
                    dkimResult.ServiceType = ExtractDKIMServiceType(dkimRecord);
                    dkimResult.IsValid = ValidateDKIMRecord(dkimRecord, dkimResult);
                }
            }
            catch (Exception ex)
            {
                dkimResult.Errors.Add($"DKIM lookup failed: {ex.Message}");
            }
            
            return dkimResult;
        }
        
        // DKIM Public Key Extraction
        private string ExtractDKIMPublicKey(string dkimRecord)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dkimRecord))
                    return "";
                
                // Look for p= parameter in DKIM record
                var match = Regex.Match(dkimRecord, @"p=([A-Za-z0-9+/=]+)");
                if (match.Success)
                {
                    return match.Groups[1].Value;
                }
                
                // Alternative pattern for different DKIM record formats
                var altMatch = Regex.Match(dkimRecord, @"p\s*=\s*([A-Za-z0-9+/=]+)");
                if (altMatch.Success)
                {
                    return altMatch.Groups[1].Value;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error extracting DKIM public key: {ex.Message}");
            }
            
            return "";
        }
        
        // DKIM Key Length Calculation
        private int CalculateKeyLength(string publicKey)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(publicKey))
                    return 0;
                
                // Decode base64 public key
                var keyBytes = Convert.FromBase64String(publicKey);
                
                // For RSA keys, we need to parse the ASN.1 structure to get the actual key length
                // For ED25519, it's typically 256 bits (32 bytes)
                
                if (keyBytes.Length >= 32)
                {
                    // Try to parse as ASN.1 DER encoded RSA key
                    try
                    {
                        // Look for RSA public key structure
                        // RSA public key in ASN.1 DER format typically starts with 0x30
                        if (keyBytes[0] == 0x30)
                        {
                            // This is a simplified approach - in a real implementation,
                            // you'd parse the ASN.1 structure to find the modulus length
                            
                            // For now, estimate based on total key size
                            // RSA keys are typically 1024, 2048, 3072, or 4096 bits
                            if (keyBytes.Length <= 200)
                                return 1024;
                            else if (keyBytes.Length <= 300)
                                return 2048;
                            else if (keyBytes.Length <= 450)
                                return 3072;
                            else
                                return 4096;
                        }
                        else if (keyBytes.Length == 32)
                        {
                            // ED25519 key
                            return 256;
                        }
                        else if (keyBytes.Length == 64)
                        {
                            // ED448 key
                            return 448;
                        }
                    }
                    catch
                    {
                        // If ASN.1 parsing fails, fall back to size estimation
                    }
                    
                    // Fallback: estimate based on key size
                    return keyBytes.Length * 8;
                }
                
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error calculating key length: {ex.Message}");
                return 0;
            }
        }
        
        // DKIM Key Type Determination
        private string DetermineKeyType(string publicKey)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(publicKey))
                    return "Unknown";
                
                // Decode base64 to examine the key structure
                var keyBytes = Convert.FromBase64String(publicKey);
                
                // Check for RSA key indicators
                if (keyBytes.Length > 100 && keyBytes[0] == 0x30)
                {
                    // ASN.1 DER encoded RSA key
                    return "RSA";
                }
                
                // Check for ED25519 key indicators
                if (keyBytes.Length == 32)
                {
                    return "ED25519";
                }
                
                // Check for other common key types
                if (keyBytes.Length == 64)
                {
                    return "ED448";
                }
                
                // Try to parse as ASN.1 to determine type
                try
                {
                    var keyInfo = new AsnEncodedData(keyBytes);
                    if (keyInfo.Oid != null)
                    {
                        return keyInfo.Oid.FriendlyName ?? "Unknown";
                    }
                }
                catch
                {
                    // Not ASN.1 encoded, might be raw key
                }
                
                return "Unknown";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error determining key type: {ex.Message}");
                return "Unknown";
            }
        }
        
        // DKIM Algorithm Extraction
        private string ExtractDKIMAlgorithm(string dkimRecord)
        {
            try
            {
                var match = Regex.Match(dkimRecord, @"k=([a-zA-Z0-9-]+)");
                return match.Success ? match.Groups[1].Value : "";
            }
            catch
            {
                return "";
            }
        }
        
        // DKIM Hash Algorithm Extraction
        private string ExtractDKIMHashAlgorithm(string dkimRecord)
        {
            try
            {
                var match = Regex.Match(dkimRecord, @"h=([a-zA-Z0-9-]+)");
                return match.Success ? match.Groups[1].Value : "";
            }
            catch
            {
                return "";
            }
        }
        
        // DKIM Service Type Extraction
        private string ExtractDKIMServiceType(string dkimRecord)
        {
            try
            {
                var match = Regex.Match(dkimRecord, @"s=([a-zA-Z0-9-]+)");
                return match.Success ? match.Groups[1].Value : "";
            }
            catch
            {
                return "";
            }
        }
        
        // DKIM Record Validation
        private bool ValidateDKIMRecord(string dkimRecord, DKIMResult result)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dkimRecord))
                {
                    result.Errors.Add("DKIM record is empty");
                    return false;
                }
                
                // Check if record starts with v=DKIM1
                if (!dkimRecord.StartsWith("v=DKIM1"))
                {
                    result.Errors.Add("DKIM record must start with 'v=DKIM1'");
                    return false;
                }
                
                // Parse DKIM tags
                var tags = dkimRecord.Split(';', StringSplitOptions.RemoveEmptyEntries);
                var tagDict = new Dictionary<string, string>();
                
                foreach (var tag in tags)
                {
                    var parts = tag.Split('=', 2);
                    if (parts.Length == 2)
                    {
                        tagDict[parts[0].Trim()] = parts[1].Trim();
                    }
                }
                
                // Check required fields
                if (!tagDict.ContainsKey("k"))
                {
                    result.Errors.Add("DKIM record missing key type (k=)");
                }
                else
                {
                    var keyType = tagDict["k"];
                    if (!new[] { "rsa", "ed25519" }.Contains(keyType.ToLower()))
                    {
                        result.Warnings.Add($"Unsupported key type: {keyType}");
                    }
                }
                
                if (!tagDict.ContainsKey("p"))
                {
                    result.Errors.Add("DKIM record missing public key (p=)");
                }
                else
                {
                    var publicKey = tagDict["p"];
                    if (string.IsNullOrWhiteSpace(publicKey))
                    {
                        result.Errors.Add("DKIM public key is empty");
                    }
                    else
                    {
                        // Validate base64 encoding
                        try
                        {
                            Convert.FromBase64String(publicKey);
                        }
                        catch
                        {
                            result.Errors.Add("DKIM public key is not valid base64");
                        }
                    }
                }
                
                // Check optional but recommended fields
                if (!tagDict.ContainsKey("h"))
                {
                    result.Warnings.Add("DKIM record missing hash algorithm (h=)");
                }
                else
                {
                    var hashAlg = tagDict["h"];
                    if (!new[] { "sha1", "sha256" }.Contains(hashAlg.ToLower()))
                    {
                        result.Warnings.Add($"Unsupported hash algorithm: {hashAlg}");
                    }
                }
                
                if (!tagDict.ContainsKey("s"))
                {
                    result.Warnings.Add("DKIM record missing service type (s=)");
                }
                
                // Check for common issues
                if (tagDict.ContainsKey("p") && tagDict["p"] == "")
                {
                    result.Warnings.Add("DKIM public key is empty (key revoked)");
                }
                
                // Validate key length
                if (result.KeyLength > 0)
                {
                    if (result.KeyType == "RSA" && result.KeyLength < 1024)
                    {
                        result.Warnings.Add($"RSA key length ({result.KeyLength} bits) is below recommended minimum (1024 bits)");
                    }
                    else if (result.KeyType == "RSA" && result.KeyLength < 2048)
                    {
                        result.Warnings.Add($"RSA key length ({result.KeyLength} bits) is below recommended minimum (2048 bits)");
                    }
                }
                
                return result.Errors.Count == 0;
            }
            catch (Exception ex)
            {
                result.Errors.Add($"DKIM validation error: {ex.Message}");
                return false;
            }
        }
        
        private DMARCResult ScanDMARC(string domain)
        {
            var dmarcResult = new DMARCResult();
            
            try
            {
                var dmarcDomain = $"_dmarc.{domain}";
                var txtRecords = GetTxtRecords(dmarcDomain);
                var dmarcRecord = txtRecords.FirstOrDefault(record => record.StartsWith("v=DMARC1"));
                
                if (dmarcRecord != null)
                {
                    dmarcResult.RecordExists = true;
                    dmarcResult.Record = dmarcRecord;
                    dmarcResult.IsValid = ValidateDMARCRecord(dmarcRecord, dmarcResult);
                }
            }
            catch (Exception ex)
            {
                dmarcResult.Errors.Add($"DMARC lookup failed: {ex.Message}");
            }
            
            return dmarcResult;
        }
        
        private bool ValidateDMARCRecord(string record, DMARCResult result)
        {
            try
            {
                if (!record.StartsWith("v=DMARC1"))
                {
                    result.Errors.Add("DMARC record must start with 'v=DMARC1'");
                    return false;
                }
                
                // Parse DMARC tags
                var tags = record.Split(';', StringSplitOptions.RemoveEmptyEntries);
                
                foreach (var tag in tags)
                {
                    var parts = tag.Split('=', 2);
                    if (parts.Length != 2) continue;
                    
                    var key = parts[0].Trim();
                    var value = parts[1].Trim();
                    
                    switch (key)
                    {
                        case "p":
                            result.Policy = value;
                            break;
                        case "sp":
                            result.SubdomainPolicy = value;
                            break;
                        case "pct":
                            if (int.TryParse(value, out int percentage))
                                result.Percentage = percentage;
                            break;
                        case "rua":
                            result.RUA = value;
                            break;
                        case "ruf":
                            result.RUF = value;
                            break;
                        case "adkim":
                        case "aspf":
                            result.Alignment = value;
                            break;
                    }
                }
                
                // Validate policy
                if (string.IsNullOrEmpty(result.Policy))
                {
                    result.Errors.Add("DMARC policy (p=) is required");
                }
                else if (!new[] { "none", "quarantine", "reject" }.Contains(result.Policy))
                {
                    result.Errors.Add("Invalid DMARC policy. Must be 'none', 'quarantine', or 'reject'");
                }
                
                // Check for warnings
                if (result.Policy == "none")
                {
                    result.Warnings.Add("Policy is set to 'none' - no action will be taken on failed messages");
                }
                
                if (result.Percentage < 100)
                {
                    result.Warnings.Add($"Only {result.Percentage}% of messages will be subject to DMARC policy");
                }
                
                return result.Errors.Count == 0;
            }
            catch (Exception ex)
            {
                result.Errors.Add($"DMARC parsing error: {ex.Message}");
                return false;
            }
        }
        
        // DNS TXT Record Lookup Methods
        private List<string> GetTxtRecords(string domain)
        {
            var txtRecords = new List<string>();
            
            try
            {
                Console.WriteLine($"Attempting DNS TXT lookup for: {domain}");
                
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    // Try PowerShell first on Windows (most reliable)
                    txtRecords = GetTxtRecordsPowerShell(domain);
                    
                    // If PowerShell fails, try nslookup
                    if (txtRecords.Count == 0)
                    {
                        Console.WriteLine("PowerShell failed, trying nslookup...");
                        txtRecords = GetTxtRecordsNslookup(domain);
                    }
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || 
                         RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    // Try dig on Unix systems
                    txtRecords = GetTxtRecordsDig(domain);
                }
                
                // If all external methods fail, try .NET DNS (limited functionality)
                if (txtRecords.Count == 0)
                {
                    Console.WriteLine("External DNS tools failed, trying .NET DNS...");
                    txtRecords = GetTxtRecordsDotNet(domain);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DNS lookup failed for {domain}: {ex.Message}");
            }
            
            return txtRecords;
        }
        
        private List<string> GetTxtRecordsDotNet(string domain)
        {
            var txtRecords = new List<string>();
            
            try
            {
                // Use .NET's built-in DNS resolution
                var hostEntry = Dns.GetHostEntry(domain);
                
                // Note: .NET's Dns.GetHostEntry doesn't directly support TXT records
                // This is a fallback that might not work for all cases
                // We'll rely on external commands for TXT record lookup
                
                Console.WriteLine($"DNS resolution successful for {domain}, but TXT records require external tools");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DNS resolution failed for {domain}: {ex.Message}");
            }
            
            return txtRecords;
        }
        
        private List<string> GetTxtRecordsNslookup(string domain)
        {
            var txtRecords = new List<string>();
            
            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "nslookup",
                        Arguments = $"-type=TXT {domain}",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    }
                };
                
                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();
                
                Console.WriteLine($"nslookup output for {domain}:");
                Console.WriteLine(output);
                Console.WriteLine($"nslookup error for {domain}:");
                Console.WriteLine(error);
                
                if (process.ExitCode != 0)
                {
                    Console.WriteLine($"nslookup error: {error}");
                    return txtRecords;
                }
                
                // Parse nslookup output - improved parsing
                var lines = output.Split('\n');
                bool inTxtSection = false;
                string currentTxtRecord = "";
                
                foreach (var line in lines)
                {
                    var trimmedLine = line.Trim();
                    
                    // Look for TXT record indicators
                    if (trimmedLine.Contains("text =") || trimmedLine.Contains("text="))
                    {
                        inTxtSection = true;
                        var startIndex = trimmedLine.IndexOf("text =") >= 0 ? 
                            trimmedLine.IndexOf("text =") + 6 : 
                            trimmedLine.IndexOf("text=") + 5;
                        var txtValue = trimmedLine.Substring(startIndex).Trim();
                        
                        if (!string.IsNullOrEmpty(txtValue))
                        {
                            currentTxtRecord = txtValue;
                            txtRecords.Add(txtValue);
                        }
                    }
                    else if (inTxtSection && (trimmedLine.StartsWith("\"") || trimmedLine.StartsWith(" ")))
                    {
                        // Multi-line TXT record continuation
                        var txtValue = trimmedLine.Trim().Trim('"');
                        if (!string.IsNullOrEmpty(txtValue))
                        {
                            if (string.IsNullOrEmpty(currentTxtRecord))
                            {
                                currentTxtRecord = txtValue;
                                txtRecords.Add(txtValue);
                            }
                            else
                            {
                                // Append to current record
                                currentTxtRecord += txtValue;
                                txtRecords[txtRecords.Count - 1] = currentTxtRecord;
                            }
                        }
                    }
                    else if (inTxtSection && !trimmedLine.StartsWith(" ") && !string.IsNullOrEmpty(trimmedLine) && !trimmedLine.StartsWith("Name:"))
                    {
                        // End of TXT records section
                        currentTxtRecord = "";
                        inTxtSection = false;
                    }
                }
                
                Console.WriteLine($"Found {txtRecords.Count} TXT records for {domain}");
                foreach (var record in txtRecords)
                {
                    Console.WriteLine($"TXT Record: {record}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"nslookup DNS TXT lookup failed for {domain}: {ex.Message}");
            }
            
            return txtRecords;
        }
        
        private List<string> GetTxtRecordsDig(string domain)
        {
            var txtRecords = new List<string>();
            
            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "dig",
                        Arguments = $"+short TXT {domain}",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    }
                };
                
                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();
                
                Console.WriteLine($"dig output for {domain}:");
                Console.WriteLine(output);
                Console.WriteLine($"dig error for {domain}:");
                Console.WriteLine(error);
                
                if (process.ExitCode != 0)
                {
                    Console.WriteLine($"dig error: {error}");
                    return txtRecords;
                }
                
                // Parse dig output
                var lines = output.Split('\n');
                foreach (var line in lines)
                {
                    var trimmedLine = line.Trim();
                    if (!string.IsNullOrWhiteSpace(trimmedLine))
                    {
                        // Remove quotes from dig output
                        var txtValue = trimmedLine.Trim('"');
                        txtRecords.Add(txtValue);
                    }
                }
                
                Console.WriteLine($"Found {txtRecords.Count} TXT records for {domain}");
                foreach (var record in txtRecords)
                {
                    Console.WriteLine($"TXT Record: {record}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"dig DNS TXT lookup failed for {domain}: {ex.Message}");
            }
            
            return txtRecords;
        }
        
        private List<string> GetTxtRecordsPowerShell(string domain)
        {
            var txtRecords = new List<string>();
            
            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "powershell",
                        Arguments = $"-Command \"Resolve-DnsName -Name '{domain}' -Type TXT | Select-Object -ExpandProperty Strings\"",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    }
                };
                
                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();
                
                Console.WriteLine($"PowerShell output for {domain}:");
                Console.WriteLine(output);
                Console.WriteLine($"PowerShell error for {domain}:");
                Console.WriteLine(error);
                
                if (process.ExitCode != 0)
                {
                    Console.WriteLine($"PowerShell error: {error}");
                    return txtRecords;
                }
                
                // Parse PowerShell output
                var lines = output.Split('\n');
                foreach (var line in lines)
                {
                    var trimmedLine = line.Trim();
                    if (!string.IsNullOrWhiteSpace(trimmedLine))
                    {
                        txtRecords.Add(trimmedLine);
                    }
                }
                
                Console.WriteLine($"Found {txtRecords.Count} TXT records for {domain}");
                foreach (var record in txtRecords)
                {
                    Console.WriteLine($"TXT Record: {record}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"PowerShell DNS TXT lookup failed for {domain}: {ex.Message}");
            }
            
            return txtRecords;
        }
        
        private int CalculateSecurityScore(EmailAuthResult result)
        {
            int score = 0;
            
            // SPF scoring
            if (result.SPF.RecordExists && result.SPF.IsValid)
                score += 30;
            else if (result.SPF.RecordExists)
                score += 15;
            
            // DKIM scoring
            if (result.DKIM.Any(d => d.RecordExists && d.IsValid))
                score += 30;
            else if (result.DKIM.Any(d => d.RecordExists))
                score += 15;
            
            // DMARC scoring
            if (result.DMARC.RecordExists && result.DMARC.IsValid)
            {
                score += 40;
                if (result.DMARC.Policy == "reject")
                    score += 10;
                else if (result.DMARC.Policy == "quarantine")
                    score += 5;
            }
            else if (result.DMARC.RecordExists)
                score += 20;
            
            return Math.Min(score, 100);
        }
        
        private string CalculateSecurityGrade(int score)
        {
            return score switch
            {
                >= 90 => "A",
                >= 80 => "B",
                >= 70 => "C",
                >= 60 => "D",
                _ => "F"
            };
        }
    }
}
