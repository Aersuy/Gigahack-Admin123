using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using LLM.DataClasses;

namespace LLM.Logic
{
    public class Communicate
    {
        const string reportPromptBegin = @"
You are a cybersecurity analyst preparing a one-page security report for company leadership.

Instructions:
- Use plain, simple language for non-technical readers.
- Keep it short and easy to read.
- Add a blank line between every section and every bullet point.
- Use this exact structure with clear spacing:

Executive Summary  
<2 short sentences>  

Security Overview  
<3 short sentences>  

Key Findings  
- <Finding 1>  
- <Finding 2>  
- <Finding 3>  

Recommendations  
- <Action 1>  
- <Action 2>  
- <Action 3>  

Conclusion  
<1 short sentence with final advice>  

- Explain technical terms briefly in simple words.
- Do NOT mention generating or writing the report.

";
        public async Task<string> SendMessage(string prompt)
        {
            var http = new HttpClient();
            var url = "http://100.98.187.96:11434/api/generate"; // Home PC Tailscale IP

            var payload = new
            {
                model = "gpt-oss:20b",
                prompt = prompt,
                stream = false
            };

            var json = JsonSerializer.Serialize(payload);
            var resp = await http.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json"));
            var responseBody = await resp.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(responseBody);
            string text = doc.RootElement.GetProperty("response").GetString();

            return text;
        }

        public async Task<string> GenerateReport(OverallReportData data)
        {
            var prompt = $"{reportPromptBegin}\n\n" +
                        $"Target: {data.Target}\n" +
                        $"Overall Score: {data.OverallScore}/100\n" +
                        $"Compliance Level: {data.ComplianceLevel}\n" +
                        $"Scan Date: {data.ScanTime:yyyy-MM-dd HH:mm:ss}\n\n" +
                        $"Security Categories:\n" +
                        $"- Network Security: {data.NetworkSecurityScore}/100\n" +
                        $"- System Security: {data.SystemSecurityScore}/100\n" +
                        $"- Vulnerability Management: {data.VulnerabilityManagementScore}/100\n" +
                        $"- Password Security: {data.PasswordSecurityScore}/100\n" +
                        $"- Web Security: {data.WebSecurityScore}/100\n\n" +
                        $"Key Findings:\n{string.Join("\n", data.KeyFindings)}\n\n" +
                        $"Recommendations:\n{string.Join("\n", data.Recommendations)}\n\n" +
                        $"Please generate a comprehensive security report based on this data.";
           

            return await SendMessage(prompt);
        }
    }

}