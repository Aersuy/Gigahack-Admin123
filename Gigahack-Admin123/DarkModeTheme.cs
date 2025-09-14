using System;
using System.Drawing;
using System.Windows.Forms;

namespace Gigahack_Admin123
{
    /// <summary>
    /// Windows 11 Dark Mode Theme Implementation
    /// Provides consistent dark mode styling across all forms with improved contrast
    /// </summary>
    public static class DarkModeTheme
    {
        // Refined Windows 11 Dark Mode Color Palette - Modern & Cohesive
        public static readonly Color BackgroundPrimary = Color.FromArgb(20, 20, 20);      // Deep dark background
        public static readonly Color BackgroundSecondary = Color.FromArgb(32, 32, 32);    // Card/panel backgrounds
        public static readonly Color BackgroundTertiary = Color.FromArgb(42, 42, 42);     // Input controls
        public static readonly Color BackgroundAccent = Color.FromArgb(52, 52, 52);       // Hover states
        
        public static readonly Color TextPrimary = Color.FromArgb(220, 220, 220);         // Softer white (much easier on eyes)
        public static readonly Color TextSecondary = Color.FromArgb(180, 180, 180);       // Secondary text
        public static readonly Color TextMuted = Color.FromArgb(140, 140, 140);           // Muted/meta text
        public static readonly Color TextDisabled = Color.FromArgb(100, 100, 100);        // Disabled text
        
        public static readonly Color BorderPrimary = Color.FromArgb(70, 70, 70);          // Subtle borders
        public static readonly Color BorderSecondary = Color.FromArgb(50, 50, 50);        // Very subtle borders
        public static readonly Color BorderAccent = Color.FromArgb(90, 90, 90);           // Focused borders
        
        // Modern Windows 11 Accent Colors - More Vibrant & Cohesive
        public static readonly Color AccentBlue = Color.FromArgb(0, 120, 215);            // Windows 11 signature blue
        public static readonly Color AccentBlueDark = Color.FromArgb(0, 90, 158);         // Pressed state
        public static readonly Color AccentBlueLight = Color.FromArgb(64, 144, 255);      // Hover state
        public static readonly Color AccentBlueBright = Color.FromArgb(96, 165, 255);     // Active/bright state
        
        // Harmonious Status Colors - Better integrated with theme
        public static readonly Color StatusSuccess = Color.FromArgb(16, 185, 129);        // Modern green
        public static readonly Color StatusWarning = Color.FromArgb(245, 158, 11);        // Warm amber
        public static readonly Color StatusError = Color.FromArgb(239, 68, 68);           // Modern red
        public static readonly Color StatusInfo = Color.FromArgb(59, 130, 246);           // Consistent blue
        
        // Refined Button Color Scheme - Modern & Cohesive
        public static readonly Color ButtonPrimary = AccentBlue;
        public static readonly Color ButtonPrimaryHover = AccentBlueBright;
        public static readonly Color ButtonPrimaryPressed = AccentBlueDark;
        public static readonly Color ButtonPrimaryText = Color.FromArgb(240, 240, 240);
        
        public static readonly Color ButtonSecondary = Color.FromArgb(60, 60, 60);
        public static readonly Color ButtonSecondaryHover = Color.FromArgb(75, 75, 75);
        public static readonly Color ButtonSecondaryPressed = Color.FromArgb(45, 45, 45);
        public static readonly Color ButtonSecondaryText = TextPrimary;
        
        public static readonly Color ButtonDanger = StatusError;
        public static readonly Color ButtonDangerHover = Color.FromArgb(248, 113, 113);    // Softer red hover
        public static readonly Color ButtonDangerPressed = Color.FromArgb(220, 38, 38);
        public static readonly Color ButtonDangerText = Color.FromArgb(240, 240, 240);
        
        public static readonly Color ButtonSuccess = StatusSuccess;
        public static readonly Color ButtonSuccessHover = Color.FromArgb(34, 197, 94);
        public static readonly Color ButtonSuccessPressed = Color.FromArgb(21, 128, 61);
        public static readonly Color ButtonSuccessText = Color.FromArgb(240, 240, 240);
        
        // Special accent colors for different button types
        public static readonly Color ButtonSpecial = Color.FromArgb(139, 92, 246);        // Modern purple
        public static readonly Color ButtonSpecialHover = Color.FromArgb(167, 139, 250);  // Lighter purple
        public static readonly Color ButtonSpecialPressed = Color.FromArgb(124, 58, 237); // Darker purple
        public static readonly Color ButtonSpecialText = Color.FromArgb(240, 240, 240);
        
        // Neutral button for less important actions
        public static readonly Color ButtonNeutral = Color.FromArgb(55, 55, 55);
        public static readonly Color ButtonNeutralHover = Color.FromArgb(68, 68, 68);
        public static readonly Color ButtonNeutralPressed = Color.FromArgb(42, 42, 42);
        public static readonly Color ButtonNeutralText = Color.FromArgb(200, 200, 200);

        /// <summary>
        /// Apply Windows 11 dark mode theme to a form - forcefully override Designer colors
        /// </summary>
        public static void ApplyToForm(Form form)
        {
            // Force form background and text colors
            form.BackColor = BackgroundPrimary;
            form.ForeColor = TextPrimary;
            
            // Apply to all child controls recursively
            ApplyToControlCollection(form.Controls);
            
            // Force a refresh to ensure changes are applied
            form.Invalidate();
            form.Refresh();
        }

        /// <summary>
        /// Apply dark mode theme to a collection of controls
        /// </summary>
        private static void ApplyToControlCollection(Control.ControlCollection controls)
        {
            foreach (Control control in controls)
            {
                ApplyToControl(control);
                
                // Recursively apply to child controls
                if (control.HasChildren)
                {
                    ApplyToControlCollection(control.Controls);
                }
            }
        }

        /// <summary>
        /// Apply dark mode theme to individual control
        /// </summary>
        public static void ApplyToControl(Control control)
        {
            switch (control)
            {
                case Label label:
                    ApplyToLabel(label);
                    break;
                case Button button:
                    ApplyToButton(button);
                    break;
                case TextBox textBox:
                    ApplyToTextBox(textBox);
                    break;
                case ListBox listBox:
                    ApplyToListBox(listBox);
                    break;
                case Panel panel:
                    ApplyToPanel(panel);
                    break;
                case ProgressBar progressBar:
                    ApplyToProgressBar(progressBar);
                    break;
                case RadioButton radioButton:
                    ApplyToRadioButton(radioButton);
                    break;
                default:
                    // Generic control styling
                    control.BackColor = BackgroundSecondary;
                    control.ForeColor = TextPrimary;
                    break;
            }
        }

        /// <summary>
        /// Apply dark mode styling to labels with improved text hierarchy
        /// </summary>
        public static void ApplyToLabel(Label label)
        {
            label.BackColor = Color.Transparent;
            
            // Determine text color based on font style, size, and content
            var labelText = label.Text.ToLower();
            
            if (label.Font.Bold && label.Font.Size >= 16)
            {
                // Main titles and headers
                label.ForeColor = TextPrimary;
            }
            else if (label.Font.Bold && label.Font.Size >= 12)
            {
                // Section headers and important labels
                label.ForeColor = TextPrimary;
            }
            else if (label.Font.Italic || labelText.Contains("category:"))
            {
                // Category labels and metadata
                label.ForeColor = TextMuted;
            }
            else if (labelText.Contains("score") || labelText.Contains("grade") || 
                     labelText.Contains("percentage") || labelText.Contains("status"))
            {
                // Status and score labels - use refined status colors
                if (label.ForeColor == Color.Green || label.ForeColor == Color.Red || 
                    label.ForeColor == Color.Orange || label.ForeColor == Color.DarkRed ||
                    labelText.Contains("good") || labelText.Contains("excellent"))
                {
                    // Apply refined status colors based on content or existing color
                    if (label.ForeColor == Color.Green || labelText.Contains("good") || labelText.Contains("excellent"))
                        label.ForeColor = StatusSuccess;
                    else if (label.ForeColor == Color.Red || label.ForeColor == Color.DarkRed || labelText.Contains("critical") || labelText.Contains("poor"))
                        label.ForeColor = StatusError;
                    else if (label.ForeColor == Color.Orange || labelText.Contains("needs") || labelText.Contains("improvement"))
                        label.ForeColor = StatusWarning;
                    else
                        label.ForeColor = StatusInfo; // Default to info blue for other status
                }
                else
                {
                    label.ForeColor = TextSecondary;
                }
            }
            else if (labelText.Contains("question") && label.Font.Bold)
            {
                // Question numbers
                label.ForeColor = TextPrimary;
            }
            else
            {
                // Regular text labels
                label.ForeColor = TextSecondary;
            }
        }

        /// <summary>
        /// Apply Windows 11 button styling while preserving Designer colors
        /// </summary>
        public static void ApplyToButton(Button button)
        {
            // Store the original Designer colors
            var originalBackColor = button.BackColor;
            
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0; // Clean borderless look
            button.UseVisualStyleBackColor = false;
            
            // Remove any existing mouse event handlers that might override colors
            button.MouseEnter -= null;
            button.MouseLeave -= null;
            
            // Preserve the Designer-set background color
            button.BackColor = originalBackColor;
            
            // Determine appropriate hover colors based on the existing button color
            if (originalBackColor == Color.FromArgb(0, 120, 215))
            {
                // Primary blue buttons
                button.ForeColor = Color.FromArgb(240, 240, 240);
                button.FlatAppearance.MouseOverBackColor = Color.FromArgb(96, 165, 255);
                button.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 100, 180);
                button.Font = new Font(button.Font.FontFamily, button.Font.Size, FontStyle.Bold);
            }
            else if (originalBackColor == Color.FromArgb(196, 43, 28))
            {
                // Windows red buttons (authentic destructive color)
                button.ForeColor = Color.FromArgb(240, 240, 240);
                button.FlatAppearance.MouseOverBackColor = Color.FromArgb(231, 72, 86);
                button.FlatAppearance.MouseDownBackColor = Color.FromArgb(164, 38, 25);
                button.Font = new Font(button.Font.FontFamily, button.Font.Size, FontStyle.Bold);
            }
            else if (originalBackColor == Color.FromArgb(55, 55, 55) || originalBackColor == Color.FromArgb(60, 60, 60))
            {
                // Neutral/secondary buttons (gray) - preserve Designer color
                button.ForeColor = Color.FromArgb(200, 200, 200);
                button.FlatAppearance.MouseOverBackColor = Color.FromArgb(68, 68, 68);
                button.FlatAppearance.MouseDownBackColor = Color.FromArgb(42, 42, 42);
                button.Font = new Font(button.Font.FontFamily, button.Font.Size, FontStyle.Regular);
            }
            else
            {
                // Default styling - preserve Designer color but add hover effects
                button.ForeColor = Color.FromArgb(240, 240, 240);
                // Create a slightly lighter version for hover
                var r = Math.Min(255, originalBackColor.R + 30);
                var g = Math.Min(255, originalBackColor.G + 30);
                var b = Math.Min(255, originalBackColor.B + 30);
                button.FlatAppearance.MouseOverBackColor = Color.FromArgb(r, g, b);
                
                // Create a slightly darker version for pressed
                r = Math.Max(0, originalBackColor.R - 20);
                g = Math.Max(0, originalBackColor.G - 20);
                b = Math.Max(0, originalBackColor.B - 20);
                button.FlatAppearance.MouseDownBackColor = Color.FromArgb(r, g, b);
                
                button.Font = new Font(button.Font.FontFamily, button.Font.Size, FontStyle.Regular);
            }
            
            // Add subtle shadow effect for depth
            button.FlatAppearance.BorderSize = 0;
        }

        /// <summary>
        /// Apply dark mode styling to text boxes with improved contrast
        /// </summary>
        public static void ApplyToTextBox(TextBox textBox)
        {
            textBox.BackColor = BackgroundTertiary;
            textBox.ForeColor = TextPrimary;
            textBox.BorderStyle = BorderStyle.None; // Clean look
            
            // Add padding effect by adjusting font
            if (textBox.Font.Size < 10)
            {
                textBox.Font = new Font(textBox.Font.FontFamily, 10, textBox.Font.Style);
            }
        }

        /// <summary>
        /// Apply dark mode styling to list boxes with improved contrast
        /// </summary>
        public static void ApplyToListBox(ListBox listBox)
        {
            listBox.BackColor = BackgroundTertiary;
            listBox.ForeColor = TextPrimary;
            listBox.BorderStyle = BorderStyle.None; // Clean look
            
            // Improve text readability
            if (listBox.Font.Name == "Consolas" && listBox.Font.Size < 10)
            {
                listBox.Font = new Font("Consolas", 10, listBox.Font.Style);
            }
            else if (listBox.Font.Size < 9)
            {
                listBox.Font = new Font(listBox.Font.FontFamily, 9, listBox.Font.Style);
            }
        }

        /// <summary>
        /// Apply dark mode styling to panels
        /// </summary>
        public static void ApplyToPanel(Panel panel)
        {
            panel.BackColor = BackgroundSecondary;
            
            // Special handling for panels with borders
            if (panel.BorderStyle == BorderStyle.FixedSingle)
            {
                panel.BorderStyle = BorderStyle.None; // Remove default border
                // Note: Custom border drawing would need to be implemented for colored borders
            }
        }

        /// <summary>
        /// Apply dark mode styling to progress bars with refined colors
        /// </summary>
        public static void ApplyToProgressBar(ProgressBar progressBar)
        {
            progressBar.BackColor = BackgroundSecondary;
            progressBar.ForeColor = AccentBlueBright; // Use brighter accent for better visibility
        }

        /// <summary>
        /// Apply dark mode styling to radio buttons
        /// </summary>
        public static void ApplyToRadioButton(RadioButton radioButton)
        {
            radioButton.BackColor = Color.Transparent;
            radioButton.ForeColor = TextPrimary;
        }

        /// <summary>
        /// Create a rounded rectangle path for custom button styling
        /// </summary>
        public static System.Drawing.Drawing2D.GraphicsPath CreateRoundedRectangle(Rectangle rect, int radius)
        {
            var path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
            path.AddArc(rect.X + rect.Width - radius, rect.Y, radius, radius, 270, 90);
            path.AddArc(rect.X + rect.Width - radius, rect.Y + rect.Height - radius, radius, radius, 0, 90);
            path.AddArc(rect.X, rect.Y + rect.Height - radius, radius, radius, 90, 90);
            path.CloseFigure();
            return path;
        }

        /// <summary>
        /// Apply Windows 11 rounded corners to a button
        /// </summary>
        public static void ApplyRoundedCorners(Button button, int radius = 8)
        {
            var path = CreateRoundedRectangle(new Rectangle(0, 0, button.Width, button.Height), radius);
            button.Region = new Region(path);
        }

        /// <summary>
        /// Apply Windows 11 rounded corners to a control
        /// </summary>
        public static void ApplyRoundedCorners(Control control, int radius = 8)
        {
            var path = CreateRoundedRectangle(new Rectangle(0, 0, control.Width, control.Height), radius);
            control.Region = new Region(path);
        }

        /// <summary>
        /// Forcefully apply dark mode colors to override Designer settings
        /// </summary>
        public static void ForceApplyColors(Form form)
        {
            // Force form colors
            form.BackColor = BackgroundPrimary;
            form.ForeColor = TextPrimary;
            
            // Force all controls recursively
            ForceApplyColorsToControls(form.Controls);
            
            // Force refresh
            form.Invalidate(true);
            form.Update();
        }

        private static void ForceApplyColorsToControls(Control.ControlCollection controls)
        {
            foreach (Control control in controls)
            {
                try
                {
                    switch (control)
                    {
                        case Button button:
                            // Force button colors regardless of Designer settings
                            ApplyToButton(button);
                            break;
                        case Label label:
                            // Force label colors
                            ApplyToLabel(label);
                            break;
                        case TextBox textBox:
                            // Force textbox colors
                            textBox.BackColor = BackgroundTertiary;
                            textBox.ForeColor = TextPrimary;
                            break;
                        case ListBox listBox:
                            // Force listbox colors
                            listBox.BackColor = BackgroundTertiary;
                            listBox.ForeColor = TextPrimary;
                            break;
                        case Panel panel:
                            // Force panel colors
                            panel.BackColor = BackgroundSecondary;
                            break;
                        case ProgressBar progressBar:
                            // Force progress bar colors
                            progressBar.BackColor = BackgroundSecondary;
                            break;
                        default:
                            // Force generic control colors
                            if (!(control is Form)) // Don't override form colors again
                            {
                                control.BackColor = BackgroundSecondary;
                                control.ForeColor = TextPrimary;
                            }
                            break;
                    }
                    
                    // Recursively apply to child controls
                    if (control.HasChildren)
                    {
                        ForceApplyColorsToControls(control.Controls);
                    }
                }
                catch
                {
                    // Skip controls that can't be modified
                    continue;
                }
            }
        }
    }
}
