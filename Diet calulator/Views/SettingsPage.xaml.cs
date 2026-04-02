using Diet_calulator.ViewModels;
using Diet_calulator.Services;

namespace Diet_calulator.Views;

public partial class SettingsPage : ContentPage
{
    private readonly SettingsViewModel _viewModel;

    public SettingsPage()
    {
        InitializeComponent();
        _viewModel = new SettingsViewModel();
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        
        // Uppdatera weight unit switch från globala inställningar
        WeightUnitSwitch.IsToggled = AppSettings.IsUsingLbs();
        UpdateWeightUnitLabels();

        // Uppdatera distance unit switch från globala inställningar
        DistanceUnitSwitch.IsToggled = AppSettings.IsUsingMiles();
        UpdateDistanceUnitLabels();
        
        ThemeService.ApplyTheme(this);
    }

    private void OnWeightUnitToggled(object sender, ToggledEventArgs e)
    {
        string unit = e.Value ? "lbs" : "kg";
        _viewModel.WeightUnit = unit;
        AppSettings.SetWeightUnit(unit);
        UpdateWeightUnitLabels();
    }

    private void OnDistanceUnitToggled(object sender, ToggledEventArgs e)
    {
        string unit = e.Value ? "miles" : "km";
        _viewModel.DistanceUnit = unit;
        AppSettings.SetDistanceUnit(unit);
        UpdateDistanceUnitLabels();
    }

    private void UpdateWeightUnitLabels()
    {
        bool isLbs = AppSettings.IsUsingLbs();
        KgLabel.TextColor = isLbs ? Color.Parse("#CCCCCC") : Color.Parse("Black");
        LbsLabel.TextColor = isLbs ? Color.Parse("Black") : Color.Parse("#CCCCCC");
        KgLabel.FontAttributes = isLbs ? FontAttributes.None : FontAttributes.Bold;
        LbsLabel.FontAttributes = isLbs ? FontAttributes.Bold : FontAttributes.None;
    }

    private void UpdateDistanceUnitLabels()
    {
        bool isMiles = AppSettings.IsUsingMiles();
        KmLabel.TextColor = isMiles ? Color.Parse("#CCCCCC") : Color.Parse("Black");
        MilesLabel.TextColor = isMiles ? Color.Parse("Black") : Color.Parse("#CCCCCC");
        KmLabel.FontAttributes = isMiles ? FontAttributes.None : FontAttributes.Bold;
        MilesLabel.FontAttributes = isMiles ? FontAttributes.Bold : FontAttributes.None;
    }
}
