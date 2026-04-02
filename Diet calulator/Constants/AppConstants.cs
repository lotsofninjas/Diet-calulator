namespace Diet_calulator.Constants;

/// <summary>
/// Centraliserade konstanter för appen för att undvika magiska strängar
/// </summary>
public static class AppConstants
{
    // Mode constants
    public const string Mode_Maintain = "Maintain";
    public const string Mode_Bulk = "Bulk";
    public const string Mode_Cut = "Cut";

    public static readonly string[] Modes = { Mode_Maintain, Mode_Bulk, Mode_Cut };

    // Macro Input Mode constants
    public const string MacroInputMode_Preset = "Preset";
    public const string MacroInputMode_Custom = "Custom";

    public static readonly string[] MacroInputModes = { MacroInputMode_Preset, MacroInputMode_Custom };

    // Eating Pattern constants
    public const string EatingPattern_SameEveryday = "Same Everyday";
    public const string EatingPattern_TrainingDays = "Training Days";

    public static readonly string[] EatingPatterns = { EatingPattern_SameEveryday, EatingPattern_TrainingDays };

    // Training Pattern constants
    public const string TrainingPattern_EveryOtherDay = "Every Other Day";
    public const string TrainingPattern_2On1Off = "2on1off";
    public const string TrainingPattern_3On1Off = "3on1off";

    public static readonly string[] TrainingPatterns = 
    { 
        TrainingPattern_EveryOtherDay, 
        TrainingPattern_2On1Off, 
        TrainingPattern_3On1Off 
    };

    // Theme constants
    public const string Theme_Light = "Light";
    public const string Theme_Dark = "Dark";

    public static readonly string[] Themes = { Theme_Light, Theme_Dark };

    // Unit constants
    public const string Unit_Kg = "kg";
    public const string Unit_Lbs = "lbs";
    public const string Unit_Km = "km";
    public const string Unit_Miles = "miles";
}
