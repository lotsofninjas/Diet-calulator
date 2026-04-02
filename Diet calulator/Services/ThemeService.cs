using System.Collections.Generic;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace Diet_calulator.Services;

public static class ThemeService
{
    private const string ThemePreferenceKey = "app_theme";

    public enum Theme
    {
        Light,
        Dark
    }

    private static Theme _currentTheme = Theme.Dark;
    private static bool _isInitialized = false;
    private static bool _initializationAttempted = false;

    public static Theme CurrentTheme 
    { 
        get => _currentTheme;
        set 
        { 
            _currentTheme = value;
            SaveThemePreference();
        }
    }

    static ThemeService()
    {
        // Don't load preference during static initialization
        // Initialize() will be called from App.xaml.cs instead
    }

    public static void Initialize()
    {
        if (_isInitialized || _initializationAttempted)
            return;

        _initializationAttempted = true;

        try
        {
            LoadThemePreference();
            _isInitialized = true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error initializing theme: {ex.Message}");
            _currentTheme = Theme.Dark;
            _isInitialized = true;
        }
    }

    private static void LoadThemePreference()
    {
        try
        {
            var savedTheme = Preferences.Default.Get(ThemePreferenceKey, "Dark");
            _currentTheme = savedTheme == "Light" ? Theme.Light : Theme.Dark;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading theme preference: {ex.Message}");
            _currentTheme = Theme.Dark;
        }
    }

    private static void SaveThemePreference()
    {
        try
        {
            Preferences.Default.Set(ThemePreferenceKey, CurrentTheme == Theme.Light ? "Light" : "Dark");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error saving theme preference: {ex.Message}");
        }
    }

    // Light color palette
    public static class LightColors
    {
        public const string Background = "#F5F5F5";
        public const string Surface = "#FFFFFF";
        public const string SurfaceVariant = "#E0E0E0";
        
        public const string TextPrimary = "#000000";
        public const string TextSecondary = "#333333";
        public const string TextTertiary = "#666666";
        
        public const string Primary = "#4ECDC4";
        
        public const string Error = "#ff6b6b";
    }

    // Dark color palette
    public static class DarkColors
    {
        public const string Background = "#121212";
        public const string Surface = "#1e1e1e";
        public const string SurfaceVariant = "#2a2a2a";
        
        public const string TextPrimary = "#ffffff";
        public const string TextSecondary = "#e0e0e0";
        public const string TextTertiary = "#a0a0a0";
        
        public const string Primary = "#4ECDC4";
        
        public const string Error = "#ff6b6b";
    }

    public static string GetBackground() => CurrentTheme == Theme.Light 
        ? LightColors.Background 
        : DarkColors.Background;

    public static string GetSurface() => CurrentTheme == Theme.Light 
        ? LightColors.Surface 
        : DarkColors.Surface;

    public static string GetSurfaceVariant() => CurrentTheme == Theme.Light 
        ? LightColors.SurfaceVariant 
        : DarkColors.SurfaceVariant;

    public static string GetTextPrimary() => CurrentTheme == Theme.Light 
        ? LightColors.TextPrimary 
        : DarkColors.TextPrimary;

    public static string GetTextSecondary() => CurrentTheme == Theme.Light 
        ? LightColors.TextSecondary 
        : DarkColors.TextSecondary;

    public static string GetTextTertiary() => CurrentTheme == Theme.Light 
        ? LightColors.TextTertiary 
        : DarkColors.TextTertiary;

    public static string GetPrimary() => CurrentTheme == Theme.Light 
        ? LightColors.Primary 
        : DarkColors.Primary;

    public static void ApplyTheme(ContentPage page)
    {
        page.BackgroundColor = Color.Parse(GetBackground());
        
        if (page.Content is VisualElement content)
        {
            ApplyThemeRecursive(content);
        }
    }

    private static void ApplyThemeRecursive(VisualElement view)
    {
        // Behandla bastyper
        if (view is Label label)
        {
            label.TextColor = Color.Parse(GetTextPrimary());
        }
        else if (view is Button button)
        {
            button.BackgroundColor = Color.Parse(GetPrimary());
            button.TextColor = Color.Parse(CurrentTheme == Theme.Light ? "#FFFFFF" : "#FFFFFF");
        }
        else if (view is Frame frame)
        {
            frame.BackgroundColor = Color.Parse(GetSurface());
            frame.BorderColor = Color.Parse(GetPrimary());
        }
        else if (view is Switch switchCtrl)
        {
            switchCtrl.ThumbColor = Color.Parse(GetPrimary());
            switchCtrl.OnColor = Color.Parse(GetPrimary());
            switchCtrl.OffColor = Color.Parse(GetSurfaceVariant());
        }
        else if (view is Entry entry)
        {
            entry.TextColor = Color.Parse(GetTextPrimary());
            entry.BackgroundColor = Color.Parse(GetSurfaceVariant());
            entry.PlaceholderColor = Color.Parse(GetTextTertiary());
        }
        else if (view is ScrollView scrollView)
        {
            scrollView.BackgroundColor = Color.Parse(GetBackground());
        }
        else if (view is VerticalStackLayout || view is HorizontalStackLayout || view is Grid)
        {
            view.BackgroundColor = Color.FromArgb("#00000000"); // Transparent
        }

        // Rekursivt för layout-containers
        if (view is Layout layout)
        {
            foreach (var child in layout.Children)
            {
                if (child is VisualElement childElement)
                {
                    ApplyThemeRecursive(childElement);
                }
            }
        }
    }
}
