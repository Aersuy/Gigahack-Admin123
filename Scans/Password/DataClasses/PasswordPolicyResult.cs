using System;
using System.Collections.Generic;

namespace Scans.Password.DataClasses
{
    public class PasswordPolicyResult
    {
        public string Target { get; set; } = string.Empty;
        public DateTime ScanTime { get; set; } = DateTime.Now;
        public bool Success { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public int SecurityScore { get; set; }
        public string SecurityGrade { get; set; } = string.Empty;
        
        // Password Complexity
        public PasswordComplexityPolicy Complexity { get; set; } = new PasswordComplexityPolicy();
        
        // Password History
        public PasswordHistoryPolicy History { get; set; } = new PasswordHistoryPolicy();
        
        // Password Age
        public PasswordAgePolicy Age { get; set; } = new PasswordAgePolicy();
        
        // Account Lockout
        public AccountLockoutPolicy Lockout { get; set; } = new AccountLockoutPolicy();
        
        // Password Change Frequency
        public PasswordChangeFrequencyPolicy ChangeFrequency { get; set; } = new PasswordChangeFrequencyPolicy();
        
        // Overall Recommendations
        public List<string> Recommendations { get; set; } = new List<string>();
        public List<string> Warnings { get; set; } = new List<string>();
        public List<string> Errors { get; set; } = new List<string>();
    }

    public class PasswordComplexityPolicy
    {
        public bool Enabled { get; set; }
        public int MinimumLength { get; set; }
        public bool RequireUppercase { get; set; }
        public bool RequireLowercase { get; set; }
        public bool RequireNumbers { get; set; }
        public bool RequireSpecialCharacters { get; set; }
        public int MinimumCharacterSets { get; set; }
        public bool PreventUsernameInPassword { get; set; }
        public bool PreventCommonPasswords { get; set; }
        public List<string> Warnings { get; set; } = new List<string>();
        public List<string> Errors { get; set; } = new List<string>();
    }

    public class PasswordHistoryPolicy
    {
        public bool Enabled { get; set; }
        public int RememberedPasswords { get; set; }
        public bool Enforced { get; set; }
        public List<string> Warnings { get; set; } = new List<string>();
        public List<string> Errors { get; set; } = new List<string>();
    }

    public class PasswordAgePolicy
    {
        public bool Enabled { get; set; }
        public int MaximumAge { get; set; } // in days
        public int MinimumAge { get; set; } // in days
        public bool Enforced { get; set; }
        public List<string> Warnings { get; set; } = new List<string>();
        public List<string> Errors { get; set; } = new List<string>();
    }

    public class AccountLockoutPolicy
    {
        public bool Enabled { get; set; }
        public int LockoutThreshold { get; set; } // number of failed attempts
        public int LockoutDuration { get; set; } // in minutes
        public int ResetCountAfter { get; set; } // in minutes
        public bool Enforced { get; set; }
        public List<string> Warnings { get; set; } = new List<string>();
        public List<string> Errors { get; set; } = new List<string>();
    }

    public class PasswordChangeFrequencyPolicy
    {
        public bool Enabled { get; set; }
        public int MinimumDaysBetweenChanges { get; set; }
        public bool Enforced { get; set; }
        public List<string> Warnings { get; set; } = new List<string>();
        public List<string> Errors { get; set; } = new List<string>();
    }
}
