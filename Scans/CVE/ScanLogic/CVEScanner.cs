using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Scans.CVE.DataClasses;
using Scans.InternetExposure.DataClases;

namespace Scans.CVE.ScanLogic
{
    public class CVEScanner
    {
        private readonly HttpClient _httpClient;
        private readonly string _nvdApiBaseUrl = "https://services.nvd.nist.gov/rest/json/cves/2.0";
        private readonly string _cveDetailsApiBaseUrl = "https://cve.mitre.org/cgi-bin/cvename.cgi";
        private readonly string _cveSearchApiBaseUrl = "https://cve.mitre.org/cgi-bin/cvename.cgi";

        public CVEScanner()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Gigahack-Admin123/1.0");
        }

        public async Task<CVESearchResult> SearchCVEsForService(string serviceName, string version = "")
        {
            var result = new CVESearchResult
            {
                SearchQuery = $"{serviceName} {version}".Trim(),
                SearchTime = DateTime.Now
            };

            try
            {
                // Search using multiple APIs
                var allCVEs = new List<CVEResult>();
                
                // Try NVD API first
                var nvdResults = await SearchNVDAPI(serviceName, version);
                allCVEs.AddRange(nvdResults);
                
                // Try CVE Search API as fallback
                var cveSearchResults = await SearchCVESearchAPI(serviceName, version);
                var existingIds = nvdResults.Select(c => c.CVEId).ToHashSet();
                allCVEs.AddRange(cveSearchResults.Where(c => !existingIds.Contains(c.CVEId)));

                result.CVEs = allCVEs.OrderByDescending(c => c.CVSSScore ?? 0).ToList();
                result.TotalResults = allCVEs.Count;
                result.Success = true;
                
                Console.WriteLine($"Total CVEs found: {result.TotalResults}");
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        public async Task<CVESearchResult> SearchCVEsForPortScanResults(List<PortScanResult> portResults)
        {
            // Flatten all port scan items from all results
            var openPorts = portResults.SelectMany(r => r.Ports).Where(p => p.open).ToList();
            
            // Build search query from discovered services
            var services = openPorts.Where(p => !string.IsNullOrEmpty(p.service))
                                  .Select(p => p.service)
                                  .Distinct()
                                  .ToList();
            
            var result = new CVESearchResult
            {
                SearchQuery = services.Any() ? string.Join(", ", services) : "No services found",
                SearchTime = DateTime.Now
            };

            try
            {
                var allCVEs = new List<CVEResult>();

                foreach (var port in openPorts)
                {
                    if (!string.IsNullOrEmpty(port.service))
                    {
                        var serviceCVEs = await SearchCVEsForService(port.service, ExtractVersionFromBanner(port.banner));
                        allCVEs.AddRange(serviceCVEs.CVEs);
                    }
                }

                // Remove duplicates based on CVE ID
                result.CVEs = allCVEs
                    .GroupBy(c => c.CVEId)
                    .Select(g => g.First())
                    .OrderByDescending(c => c.CVSSScore ?? 0)
                    .ToList();

                result.TotalResults = result.CVEs.Count;
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        private async Task<List<CVEResult>> SearchNVDAPI(string serviceName, string version)
        {
            var results = new List<CVEResult>();

            try
            {
                // Build search query for NVD API
                var searchTerms = new List<string> { serviceName };
                if (!string.IsNullOrEmpty(version))
                {
                    searchTerms.Add(version);
                }

                var keywordQuery = string.Join(" ", searchTerms);
                var encodedQuery = Uri.EscapeDataString(keywordQuery);
                
                // Search with keyword parameter - NVD API 2.0 format
                var url = $"{_nvdApiBaseUrl}?keywordSearch={encodedQuery}&resultsPerPage=50&startIndex=0";
                
                Console.WriteLine($"Searching NVD API with URL: {url}");
                Console.WriteLine($"Search terms: {keywordQuery}");
                
                var response = await _httpClient.GetStringAsync(url);
                Console.WriteLine($"NVD API Response length: {response.Length}");
                
                var nvdResponse = JsonConvert.DeserializeObject<CVEDatabaseResponse>(response);

                if (nvdResponse?.vulnerabilities != null)
                {
                    Console.WriteLine($"Found {nvdResponse.vulnerabilities.Count} results from NVD API");
                    foreach (var vulnerability in nvdResponse.vulnerabilities)
                    {
                        var cve = ConvertNVDItemToCVEResult(vulnerability.cve);
                        if (cve != null)
                        {
                            results.Add(cve);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("No results found in NVD API response");
                    Console.WriteLine($"Response: {response.Substring(0, Math.Min(500, response.Length))}...");
                }
            }
            catch (Exception ex)
            {
                // Log error but continue with other sources
                Console.WriteLine($"NVD API Error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }

            return results;
        }

        private async Task<List<CVEResult>> SearchCVESearchAPI(string serviceName, string version)
        {
            var results = new List<CVEResult>();

            try
            {
                // For now, just return empty results and focus on NVD API
                Console.WriteLine("CVE Search API temporarily disabled - focusing on NVD API");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CVE Search API Error: {ex.Message}");
            }

            return results;
        }
        
        private string GetSeverityFromScore(double score)
        {
            if (score >= 9.0) return "Critical";
            if (score >= 7.0) return "High";
            if (score >= 4.0) return "Medium";
            if (score >= 0.1) return "Low";
            return "None";
        }

        private CVEResult? ConvertNVDItemToCVEResult(CVEDatabaseItem item)
        {
            try
            {
                var cve = new CVEResult
                {
                    CVEId = item.id,
                    PublishedDate = item.published,
                    LastModifiedDate = item.lastModified,
                    Source = "NVD"
                };

                // Extract description
                if (item.descriptions?.Lang?.Any() == true)
                {
                    var englishDesc = item.descriptions.Lang.FirstOrDefault(d => d.Lang == "en");
                    if (englishDesc != null)
                    {
                        cve.Description = englishDesc.Value;
                    }
                }

                // Extract CVSS score and severity
                if (item.metrics?.CvssMetricV31?.Any() == true)
                {
                    var cvss = item.metrics.CvssMetricV31.First();
                    cve.CVSSScore = cvss.CvssData.BaseScore;
                    cve.Severity = cvss.CvssData.BaseSeverity;
                    cve.CVSSVector = cvss.CvssData.VectorString;
                }
                else if (item.metrics?.CvssMetricV30?.Any() == true)
                {
                    var cvss = item.metrics.CvssMetricV30.First();
                    cve.CVSSScore = cvss.CvssData.BaseScore;
                    cve.Severity = cvss.CvssData.BaseSeverity;
                    cve.CVSSVector = cvss.CvssData.VectorString;
                }
                else if (item.metrics?.CvssMetricV2?.Any() == true)
                {
                    var cvss = item.metrics.CvssMetricV2.First();
                    cve.CVSSScore = cvss.CvssData.BaseScore;
                    cve.Severity = cvss.CvssData.BaseSeverity;
                    cve.CVSSVector = cvss.CvssData.VectorString;
                }

                // Extract CWE information
                if (item.weaknesses?.Description?.Any() == true)
                {
                    cve.CWE = item.weaknesses.Description
                        .Where(w => w.Type == "Primary" && !string.IsNullOrEmpty(w.Value))
                        .Select(w => w.Value)
                        .ToList();
                }

                // Extract references
                if (item.references?.ReferenceData?.Any() == true)
                {
                    cve.References = item.references.ReferenceData
                        .Select(r => r.Url)
                        .ToList();
                }

                return cve;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error converting NVD item: {ex.Message}");
                return null;
            }
        }

        private string ExtractVersionFromBanner(string banner)
        {
            if (string.IsNullOrEmpty(banner))
                return string.Empty;

            // Simple version extraction patterns
            var versionPatterns = new[]
            {
                @"(\d+\.\d+\.\d+)",  // x.y.z
                @"(\d+\.\d+)",       // x.y
                @"version\s+(\d+\.\d+\.\d+)",  // version x.y.z
                @"v(\d+\.\d+\.\d+)"  // vx.y.z
            };

            foreach (var pattern in versionPatterns)
            {
                var match = System.Text.RegularExpressions.Regex.Match(banner, pattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    return match.Groups[1].Value;
                }
            }

            return string.Empty;
        }

        public async Task TestAPIs()
        {
            Console.WriteLine("Testing CVE APIs...");
            
            // Test with a known vulnerable service
            var testService = "apache";
            var testVersion = "2.4.41";
            
            Console.WriteLine($"Testing with service: {testService}, version: {testVersion}");
            
            var nvdResults = await SearchNVDAPI(testService, testVersion);
            Console.WriteLine($"NVD API returned {nvdResults.Count} results");
            
            var cveSearchResults = await SearchCVESearchAPI(testService, testVersion);
            Console.WriteLine($"CVE Search API returned {cveSearchResults.Count} results");
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}
