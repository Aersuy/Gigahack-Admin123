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
You are a cybersecurity analyst preparing a professional, easy-to-understand security report for company leadership.
- Do NOT mention generating or writing the report.
- Use clear, simple language; anyone without technical knowledge should understand it.
- Organize the report into sections: Executive Summary, Security Overview, Key Findings, Recommendations, and Conclusion.
- Explain the significance and potential impact of each finding.
- Use short paragraphs and bullet points for readability.
- Maintain a professional and neutral tone.
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
            string prompFinal = reportPromptBegin + prompt;

            return await SendMessage(prompFinal);
        }
    }

}
