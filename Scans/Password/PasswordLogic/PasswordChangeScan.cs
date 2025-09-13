using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Security.Principal;
using System.Management;
using System.Runtime.Versioning;
using Scans.Password.DataClasses;

namespace Scans.Password.PasswordLogic
{
    [SupportedOSPlatform("windows")]
    public class PasswordChangeScan
    {
        public async Task<PasswordPolicyResult> ScanPasswordPolicies(string domainController = null)
        {
            var result = new PasswordPolicyResult
            {
                Target = domainController ?? "Local Machine",
                ScanTime = DateTime.Now
            };

            try
            {
                // Try to scan domain policies if domain controller is provided
                if (!string.IsNullOrEmpty(domainController))
                {
                  //  await ScanDomainPasswordPolicies(result, domainController);
                }
                else
                {
                    // Scan local machine policies
                    ScanLocalPasswordPolicies(result);
                }

                // Calculate security score
                CalculateSecurityScore(result);
                
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = ex.Message;
                result.Errors.Add($"Scan failed: {ex.Message}");
            }

            return result;
        }
        /*
         * No domain controller
        private async Task ScanDomainPasswordPolicies(PasswordPolicyResult result, string domainController)
        {
            try
            {
                result.Target = domainController;
                
               
                await Task.Delay(1000); // Simulate async operation
                
            
                
                result.Warnings.Add("Domain policy scanning requires Active Directory access - using simulated results");
                
                // Simulated domain policy results
                result.Complexity.Enabled = true;
                result.Complexity.MinimumLength = 8;
                result.Complexity.RequireUppercase = true;
                result.Complexity.RequireLowercase = true;
                result.Complexity.RequireNumbers = true;
                result.Complexity.RequireSpecialCharacters = true;
                result.Complexity.MinimumCharacterSets = 3;
                
                result.History.Enabled = true;
                result.History.RememberedPasswords = 12;
                result.History.Enforced = true;
                
                result.Age.Enabled = true;
                result.Age.MaximumAge = 90; // 90 days
                result.Age.MinimumAge = 1; // 1 day
                result.Age.Enforced = true;
                
                result.Lockout.Enabled = true;
                result.Lockout.LockoutThreshold = 5;
                result.Lockout.LockoutDuration = 30; // 30 minutes
                result.Lockout.ResetCountAfter = 30;
                result.Lockout.Enforced = true;
                
                result.ChangeFrequency.Enabled = true;
                result.ChangeFrequency.MinimumDaysBetweenChanges = 1;
                result.ChangeFrequency.Enforced = true;
            }
            catch (Exception ex)
            {
                result.Errors.Add($"Domain policy scan failed: {ex.Message}");
            }
        }
        */
        private void ScanLocalPasswordPolicies(PasswordPolicyResult result)
        {
            try
            {
                // Check if running with elevated privileges
                bool isElevated = IsRunningAsAdministrator();
                if (!isElevated)
                {
                    result.Warnings.Add("Running without elevated privileges - some policy settings may not be accessible");
                }

                // Scan password complexity policies
                ScanPasswordComplexityPolicies(result);
                
                // Scan password history policies
                ScanPasswordHistoryPolicies(result);
                
                // Scan password age policies
                ScanPasswordAgePolicies(result);
                
                // Scan account lockout policies
                ScanAccountLockoutPolicies(result);
                
                // Scan password change frequency policies
                ScanPasswordChangeFrequencyPolicies(result);
                
                // Additional security checks
                PerformAdditionalSecurityChecks(result);
            }
            catch (Exception ex)
            {
                result.Errors.Add($"Local policy scan failed: {ex.Message}");
            }
        }

        [SupportedOSPlatform("windows")]
        private bool IsRunningAsAdministrator()
        {
            try
            {
                var identity = WindowsIdentity.GetCurrent();
                var principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch
            {
                return false;
            }
        }

        [SupportedOSPlatform("windows")]
        private void ScanPasswordComplexityPolicies(PasswordPolicyResult result)
        {
            try
            {
                // Read from Local Security Policy registry
                using (var key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\Netlogon\Parameters"))
                {
                    if (key != null)
                    {
                        // Password complexity requirements
                        var passwordComplexity = key.GetValue("RequireStrongKey") as int?;
                        result.Complexity.Enabled = passwordComplexity == 1;
                    }
                }

                // Read from Security Policy registry
                using (var key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Lsa"))
                {
                    if (key != null)
                    {
                        // Password complexity
                        var passwordComplexity = key.GetValue("PasswordComplexity") as int?;
                        if (passwordComplexity.HasValue)
                        {
                            result.Complexity.Enabled = passwordComplexity.Value == 1;
                        }

                        // Minimum password length
                        var minPasswordLength = key.GetValue("MinPasswordLength") as int?;
                        if (minPasswordLength.HasValue)
                        {
                            result.Complexity.MinimumLength = minPasswordLength.Value;
                        }
                        else
                        {
                            result.Complexity.MinimumLength = 0;
                        }
                    }
                }

                // Try to get more detailed complexity requirements from Group Policy
                try
                {
                    using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Policies\Microsoft\Windows\System"))
                    {
                        if (key != null)
                        {
                            var passwordComplexity = key.GetValue("PasswordComplexity") as int?;
                            if (passwordComplexity.HasValue)
                            {
                                result.Complexity.Enabled = passwordComplexity.Value == 1;
                            }

                            var minPasswordLength = key.GetValue("MinPasswordLength") as int?;
                            if (minPasswordLength.HasValue)
                            {
                                result.Complexity.MinimumLength = minPasswordLength.Value;
                            }
                        }
                    }
                }
                catch
                {
                    // Group Policy registry may not exist
                }

                // Set default complexity requirements based on Windows version and settings
                if (result.Complexity.Enabled)
                {
                    // Windows default complexity requirements
                    result.Complexity.RequireUppercase = true;
                    result.Complexity.RequireLowercase = true;
                    result.Complexity.RequireNumbers = true;
                    result.Complexity.RequireSpecialCharacters = true;
                    result.Complexity.MinimumCharacterSets = 3;
                    result.Complexity.PreventUsernameInPassword = true;
                }

                // Validate minimum length
                if (result.Complexity.MinimumLength < 8)
                {
                    result.Complexity.Warnings.Add($"Minimum password length is {result.Complexity.MinimumLength} - recommend at least 8 characters");
                }
            }
            catch (Exception ex)
            {
                result.Complexity.Errors.Add($"Failed to read password complexity policies: {ex.Message}");
            }
        }

        [SupportedOSPlatform("windows")]
        private void ScanPasswordHistoryPolicies(PasswordPolicyResult result)
        {
            try
            {
                using (var key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Lsa"))
                {
                    if (key != null)
                    {
                        var passwordHistory = key.GetValue("PasswordHistoryLength") as int?;
                        if (passwordHistory.HasValue)
                        {
                            result.History.RememberedPasswords = passwordHistory.Value;
                            result.History.Enabled = passwordHistory.Value > 0;
                            result.History.Enforced = passwordHistory.Value > 0;
                        }
                    }
                }

                // Check Group Policy settings
                try
                {
                    using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Policies\Microsoft\Windows\System"))
                    {
                        if (key != null)
                        {
                            var passwordHistory = key.GetValue("PasswordHistoryLength") as int?;
                            if (passwordHistory.HasValue)
                            {
                                result.History.RememberedPasswords = passwordHistory.Value;
                                result.History.Enabled = passwordHistory.Value > 0;
                                result.History.Enforced = passwordHistory.Value > 0;
                            }
                        }
                    }
                }
                catch
                {
                    // Group Policy registry may not exist
                }

                if (result.History.RememberedPasswords < 12)
                {
                    result.History.Warnings.Add($"Password history is {result.History.RememberedPasswords} - recommend at least 12");
                }
            }
            catch (Exception ex)
            {
                result.History.Errors.Add($"Failed to read password history policies: {ex.Message}");
            }
        }

        [SupportedOSPlatform("windows")]
        private void ScanPasswordAgePolicies(PasswordPolicyResult result)
        {
            try
            {
                using (var key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Lsa"))
                {
                    if (key != null)
                    {
                        var maxPasswordAge = key.GetValue("MaxPasswordAge") as int?;
                        var minPasswordAge = key.GetValue("MinPasswordAge") as int?;

                        if (maxPasswordAge.HasValue)
                        {
                            result.Age.MaximumAge = maxPasswordAge.Value / 86400; // Convert seconds to days
                            result.Age.Enabled = maxPasswordAge.Value > 0;
                            result.Age.Enforced = maxPasswordAge.Value > 0;
                        }

                        if (minPasswordAge.HasValue)
                        {
                            result.Age.MinimumAge = minPasswordAge.Value / 86400; // Convert seconds to days
                        }
                    }
                }

                // Check Group Policy settings
                try
                {
                    using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Policies\Microsoft\Windows\System"))
                    {
                        if (key != null)
                        {
                            var maxPasswordAge = key.GetValue("MaxPasswordAge") as int?;
                            var minPasswordAge = key.GetValue("MinPasswordAge") as int?;

                            if (maxPasswordAge.HasValue)
                            {
                                result.Age.MaximumAge = maxPasswordAge.Value / 86400;
                                result.Age.Enabled = maxPasswordAge.Value > 0;
                                result.Age.Enforced = maxPasswordAge.Value > 0;
                            }

                            if (minPasswordAge.HasValue)
                            {
                                result.Age.MinimumAge = minPasswordAge.Value / 86400;
                            }
                        }
                    }
                }
                catch
                {
                    // Group Policy registry may not exist
                }

                if (result.Age.MaximumAge > 90)
                {
                    result.Age.Warnings.Add($"Maximum password age is {result.Age.MaximumAge} days - recommend 90 days or less");
                }
            }
            catch (Exception ex)
            {
                result.Age.Errors.Add($"Failed to read password age policies: {ex.Message}");
            }
        }

        [SupportedOSPlatform("windows")]
        private void ScanAccountLockoutPolicies(PasswordPolicyResult result)
        {
            try
            {
                using (var key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Lsa"))
                {
                    if (key != null)
                    {
                        var lockoutThreshold = key.GetValue("LockoutThreshold") as int?;
                        var lockoutDuration = key.GetValue("LockoutDuration") as int?;
                        var resetCountAfter = key.GetValue("ResetLockoutCount") as int?;

                        if (lockoutThreshold.HasValue)
                        {
                            result.Lockout.LockoutThreshold = lockoutThreshold.Value;
                            result.Lockout.Enabled = lockoutThreshold.Value > 0;
                            result.Lockout.Enforced = lockoutThreshold.Value > 0;
                        }

                        if (lockoutDuration.HasValue)
                        {
                            result.Lockout.LockoutDuration = lockoutDuration.Value / 60; // Convert seconds to minutes
                        }

                        if (resetCountAfter.HasValue)
                        {
                            result.Lockout.ResetCountAfter = resetCountAfter.Value / 60; // Convert seconds to minutes
                        }
                    }
                }

                // Check Group Policy settings
                try
                {
                    using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Policies\Microsoft\Windows\System"))
                    {
                        if (key != null)
                        {
                            var lockoutThreshold = key.GetValue("LockoutThreshold") as int?;
                            var lockoutDuration = key.GetValue("LockoutDuration") as int?;
                            var resetCountAfter = key.GetValue("ResetLockoutCount") as int?;

                            if (lockoutThreshold.HasValue)
                            {
                                result.Lockout.LockoutThreshold = lockoutThreshold.Value;
                                result.Lockout.Enabled = lockoutThreshold.Value > 0;
                                result.Lockout.Enforced = lockoutThreshold.Value > 0;
                            }

                            if (lockoutDuration.HasValue)
                            {
                                result.Lockout.LockoutDuration = lockoutDuration.Value / 60;
                            }

                            if (resetCountAfter.HasValue)
                            {
                                result.Lockout.ResetCountAfter = resetCountAfter.Value / 60;
                            }
                        }
                    }
                }
                catch
                {
                    // Group Policy registry may not exist
                }

                if (result.Lockout.LockoutThreshold > 5)
                {
                    result.Lockout.Warnings.Add($"Lockout threshold is {result.Lockout.LockoutThreshold} - recommend 5 or fewer attempts");
                }
            }
            catch (Exception ex)
            {
                result.Lockout.Errors.Add($"Failed to read account lockout policies: {ex.Message}");
            }
        }

        [SupportedOSPlatform("windows")]
        private void ScanPasswordChangeFrequencyPolicies(PasswordPolicyResult result)
        {
            try
            {
                using (var key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Lsa"))
                {
                    if (key != null)
                    {
                        var minPasswordAge = key.GetValue("MinPasswordAge") as int?;
                        if (minPasswordAge.HasValue)
                        {
                            result.ChangeFrequency.MinimumDaysBetweenChanges = minPasswordAge.Value / 86400; // Convert seconds to days
                            result.ChangeFrequency.Enabled = minPasswordAge.Value > 0;
                            result.ChangeFrequency.Enforced = minPasswordAge.Value > 0;
                        }
                    }
                }

                // Check Group Policy settings
                try
                {
                    using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Policies\Microsoft\Windows\System"))
                    {
                        if (key != null)
                        {
                            var minPasswordAge = key.GetValue("MinPasswordAge") as int?;
                            if (minPasswordAge.HasValue)
                            {
                                result.ChangeFrequency.MinimumDaysBetweenChanges = minPasswordAge.Value / 86400;
                                result.ChangeFrequency.Enabled = minPasswordAge.Value > 0;
                                result.ChangeFrequency.Enforced = minPasswordAge.Value > 0;
                            }
                        }
                    }
                }
                catch
                {
                    // Group Policy registry may not exist
                }
            }
            catch (Exception ex)
            {
                result.ChangeFrequency.Errors.Add($"Failed to read password change frequency policies: {ex.Message}");
            }
        }

        private void PerformAdditionalSecurityChecks(PasswordPolicyResult result)
        {
            try
            {
                // Check for additional security features
                CheckWindowsSecurityFeatures(result);
                
                // Check for common security misconfigurations
                CheckSecurityMisconfigurations(result);
            }
            catch (Exception ex)
            {
                result.Warnings.Add($"Additional security checks failed: {ex.Message}");
            }
        }

        [SupportedOSPlatform("windows")]
        private void CheckWindowsSecurityFeatures(PasswordPolicyResult result)
        {
            try
            {
                // Check if Windows Defender is enabled
                using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Policies\Microsoft\Windows Defender"))
                {
                    if (key != null)
                    {
                        var disableAntiSpyware = key.GetValue("DisableAntiSpyware") as int?;
                        if (disableAntiSpyware == 1)
                        {
                            result.Warnings.Add("Windows Defender appears to be disabled");
                        }
                    }
                }

                // Check UAC settings
                using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System"))
                {
                    if (key != null)
                    {
                        var enableLUA = key.GetValue("EnableLUA") as int?;
                        if (enableLUA == 0)
                        {
                            result.Warnings.Add("User Account Control (UAC) is disabled");
                        }
                    }
                }
            }
            catch
            {
                // Registry access may fail
            }
        }

        private void CheckSecurityMisconfigurations(PasswordPolicyResult result)
        {
            try
            {
                // Check for weak password policies
                if (!result.Complexity.Enabled)
                {
                    result.Errors.Add("Password complexity is not enabled - this is a critical security risk");
                }

                if (!result.History.Enabled)
                {
                    result.Warnings.Add("Password history is not enabled - users can reuse passwords");
                }

                if (!result.Age.Enabled)
                {
                    result.Warnings.Add("Password expiration is not enabled - passwords never expire");
                }

                if (!result.Lockout.Enabled)
                {
                    result.Warnings.Add("Account lockout is not enabled - vulnerable to brute force attacks");
                }

                // Check for default or weak settings
                if (result.Complexity.MinimumLength < 8)
                {
                    result.Errors.Add($"Minimum password length is too short: {result.Complexity.MinimumLength} characters");
                }

                if (result.Age.MaximumAge > 365)
                {
                    result.Warnings.Add($"Maximum password age is too long: {result.Age.MaximumAge} days");
                }

                if (result.Lockout.LockoutThreshold > 10)
                {
                    result.Warnings.Add($"Account lockout threshold is too high: {result.Lockout.LockoutThreshold} attempts");
                }
            }
            catch (Exception ex)
            {
                result.Warnings.Add($"Security misconfiguration check failed: {ex.Message}");
            }
        }

        private void CalculateSecurityScore(PasswordPolicyResult result)
        {
            int score = 0;
            int maxScore = 100;

            // Password Complexity (40 points)
            if (result.Complexity.Enabled)
            {
                score += 10; // Base score for having complexity enabled
                
                if (result.Complexity.MinimumLength >= 12) score += 10;
                else if (result.Complexity.MinimumLength >= 8) score += 7;
                else if (result.Complexity.MinimumLength >= 6) score += 4;
                
                if (result.Complexity.RequireUppercase) score += 5;
                if (result.Complexity.RequireLowercase) score += 5;
                if (result.Complexity.RequireNumbers) score += 5;
                if (result.Complexity.RequireSpecialCharacters) score += 5;
            }

            // Password History (20 points)
            if (result.History.Enabled && result.History.Enforced)
            {
                score += 10; // Base score for having history enabled
                if (result.History.RememberedPasswords >= 12) score += 10;
                else if (result.History.RememberedPasswords >= 6) score += 7;
                else if (result.History.RememberedPasswords >= 3) score += 4;
            }

            // Password Age (20 points)
            if (result.Age.Enabled && result.Age.Enforced)
            {
                score += 10; // Base score for having age policy enabled
                if (result.Age.MaximumAge <= 30) score += 10;
                else if (result.Age.MaximumAge <= 60) score += 7;
                else if (result.Age.MaximumAge <= 90) score += 4;
            }

            // Account Lockout (20 points)
            if (result.Lockout.Enabled && result.Lockout.Enforced)
            {
                score += 10; // Base score for having lockout enabled
                if (result.Lockout.LockoutThreshold <= 3) score += 10;
                else if (result.Lockout.LockoutThreshold <= 5) score += 7;
                else if (result.Lockout.LockoutThreshold <= 10) score += 4;
            }

            result.SecurityScore = Math.Min(score, maxScore);

            // Determine security grade
            if (result.SecurityScore >= 90) result.SecurityGrade = "A";
            else if (result.SecurityScore >= 80) result.SecurityGrade = "B";
            else if (result.SecurityScore >= 70) result.SecurityGrade = "C";
            else if (result.SecurityScore >= 60) result.SecurityGrade = "D";
            else result.SecurityGrade = "F";

            // Generate recommendations based on score
            GenerateRecommendations(result);
        }

        private void GenerateRecommendations(PasswordPolicyResult result)
        {
            // Password Complexity recommendations
            if (!result.Complexity.Enabled)
            {
                result.Recommendations.Add("Enable password complexity requirements");
            }
            else
            {
                if (result.Complexity.MinimumLength < 8)
                {
                    result.Recommendations.Add($"Increase minimum password length to at least 8 characters (currently {result.Complexity.MinimumLength})");
                }
                
                if (!result.Complexity.RequireUppercase)
                    result.Recommendations.Add("Require uppercase letters in passwords");
                
                if (!result.Complexity.RequireLowercase)
                    result.Recommendations.Add("Require lowercase letters in passwords");
                
                if (!result.Complexity.RequireNumbers)
                    result.Recommendations.Add("Require numbers in passwords");
                
                if (!result.Complexity.RequireSpecialCharacters)
                    result.Recommendations.Add("Require special characters in passwords");
            }

            // Password History recommendations
            if (!result.History.Enabled)
            {
                result.Recommendations.Add("Enable password history to prevent password reuse");
            }
            else if (result.History.RememberedPasswords < 12)
            {
                result.Recommendations.Add($"Increase password history to at least 12 passwords (currently {result.History.RememberedPasswords})");
            }

            // Password Age recommendations
            if (!result.Age.Enabled)
            {
                result.Recommendations.Add("Enable password expiration policy");
            }
            else if (result.Age.MaximumAge > 90)
            {
                result.Recommendations.Add($"Reduce maximum password age to 90 days or less (currently {result.Age.MaximumAge} days)");
            }

            // Account Lockout recommendations
            if (!result.Lockout.Enabled)
            {
                result.Recommendations.Add("Enable account lockout policy to prevent brute force attacks");
            }
            else if (result.Lockout.LockoutThreshold > 5)
            {
                result.Recommendations.Add($"Reduce lockout threshold to 5 or fewer attempts (currently {result.Lockout.LockoutThreshold})");
            }

            // General security recommendations
            if (result.SecurityScore < 70)
            {
                result.Recommendations.Add("Consider implementing multi-factor authentication (MFA)");
                result.Recommendations.Add("Regular security awareness training for users");
            }
        }
    }
}
