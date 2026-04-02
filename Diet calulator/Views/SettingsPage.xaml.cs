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
        
        // Uppdatera Switch med sparad tema-inställning
        ThemeSwitch.IsToggled = ThemeService.CurrentTheme == ThemeService.Theme.Dark;
        
        ThemeService.ApplyTheme(this);
    }

    private void OnThemeToggled(object sender, ToggledEventArgs e)
    {
        ThemeService.CurrentTheme = e.Value ? ThemeService.Theme.Dark : ThemeService.Theme.Light;
        ThemeService.ApplyTheme(this);
        
        // Applicera på andra sidor om de är synliga
        if (Application.Current?.MainPage is Shell shell)
        {
            shell.BackgroundColor = Color.Parse(ThemeService.GetBackground());
            if (shell.FindByName<Label>("TitleLabel") is Label titleLabel)
            {
                titleLabel.TextColor = Color.Parse(ThemeService.GetTextPrimary());
            }
        }
    }

    private void OnWeightUnitToggled(object sender, ToggledEventArgs e)
    {
        _viewModel.WeightUnit = e.Value ? "lbs" : "kg";
    }

    private void OnDistanceUnitToggled(object sender, ToggledEventArgs e)
    {
        _viewModel.DistanceUnit = e.Value ? "miles" : "km";
    }
}
