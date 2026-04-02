using Diet_calulator.Services;
using Diet_calulator.ViewModels;

namespace Diet_calulator.Views;

public partial class CardioPage : ContentPage
{
    private readonly CardioViewModel _viewModel;

    public CardioPage()
    {
        InitializeComponent();
        _viewModel = new CardioViewModel();
        BindingContext = _viewModel;
        UpdateActivityUI();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ThemeService.ApplyTheme(this);
        
        // Update speed unit label based on distance unit setting
        var distanceUnit = AppSettings.GetDistanceUnit();
        SpeedUnitLabel.Text = (distanceUnit == "miles") ? "mph" : "km/h";
    }

    private void OnActivityClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        if (button == null) return;

        // Reset all button colors
        WalkingButton.BackgroundColor = Color.Parse("#E0E0E0");
        WalkingButton.TextColor = Color.Parse("#666666");
        RunningButton.BackgroundColor = Color.Parse("#E0E0E0");
        RunningButton.TextColor = Color.Parse("#666666");
        CyclingButton.BackgroundColor = Color.Parse("#E0E0E0");
        CyclingButton.TextColor = Color.Parse("#666666");

        // Set clicked button to active color
        button.BackgroundColor = Color.Parse("#4ECDC4");
        button.TextColor = Color.Parse("White");

        // Update view model
        _viewModel.ActivityType = button.Text;

        // Show/hide relevant sections
        UpdateActivityUI();
    }

    private void UpdateActivityUI()
    {
        switch (_viewModel.ActivityType)
        {
            case "Walking":
                SpeedSection.IsVisible = true;
                InclineSection.IsVisible = true;
                WattsSection.IsVisible = false;
                break;
            case "Running":
                SpeedSection.IsVisible = true;
                InclineSection.IsVisible = true;
                WattsSection.IsVisible = false;
                break;
            case "Cycling":
                SpeedSection.IsVisible = false;
                InclineSection.IsVisible = false;
                WattsSection.IsVisible = true;
                break;
        }
    }

    private async void OnWeightTapped(object sender, TappedEventArgs e)
    {
        var result = await DisplayPromptAsync(
            "Enter Weight",
            $"Current: {_viewModel.Weight:F1} {_viewModel.WeightUnit}",
            "OK",
            "Cancel",
            "Weight",
            keyboard: Keyboard.Numeric);

        if (!string.IsNullOrEmpty(result) && double.TryParse(result, out var weight))
        {
            _viewModel.Weight = weight;
        }
    }

    private async void OnDurationTapped(object sender, TappedEventArgs e)
    {
        var result = await DisplayPromptAsync(
            "Enter Duration",
            $"Current: {_viewModel.Duration} minutes",
            "OK",
            "Cancel",
            "Minutes",
            keyboard: Keyboard.Numeric);

        if (!string.IsNullOrEmpty(result) && int.TryParse(result, out var duration))
        {
            _viewModel.Duration = duration;
        }
    }

    private async void OnSpeedTapped(object sender, TappedEventArgs e)
    {
        var speedUnit = AppSettings.GetDistanceUnit() == "miles" ? "mph" : "km/h";
        var isMiles = AppSettings.GetDistanceUnit() == "miles";
        var currentSpeed = isMiles ? _viewModel.Speed / 1.60934 : _viewModel.Speed;
        
        var result = await DisplayPromptAsync(
            "Enter Speed",
            $"Current: {currentSpeed:F1} {speedUnit}",
            "OK",
            "Cancel",
            speedUnit,
            keyboard: Keyboard.Numeric);

        if (!string.IsNullOrEmpty(result) && double.TryParse(result, out var speed))
        {
            // Konvertera från användarens enhet till km/h för lagring
            var speedKmh = isMiles ? speed * 1.60934 : speed;
            _viewModel.Speed = speedKmh;
        }
    }

    private async void OnInclineTapped(object sender, TappedEventArgs e)
    {
        var result = await DisplayPromptAsync(
            "Enter Incline",
            $"Current: {_viewModel.Incline:F1}",
            "OK",
            "Cancel",
            "%",
            keyboard: Keyboard.Numeric);

        if (!string.IsNullOrEmpty(result) && double.TryParse(result, out var incline))
        {
            _viewModel.Incline = incline;
        }
    }

    private async void OnWattsTapped(object sender, TappedEventArgs e)
    {
        var result = await DisplayPromptAsync(
            "Enter Power",
            $"Current: {_viewModel.Watts:F0} watts",
            "OK",
            "Cancel",
            "watts",
            keyboard: Keyboard.Numeric);

        if (!string.IsNullOrEmpty(result) && double.TryParse(result, out var watts))
        {
            _viewModel.Watts = watts;
        }
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        var name = await DisplayPromptAsync(
            "Save Cardio Plan",
            "Enter a name for this plan:",
            "Save",
            "Cancel",
            "My Cardio Plan");

        if (!string.IsNullOrEmpty(name))
        {
            await _viewModel.SaveCalculationAsync(name);
            await DisplayAlert("Success", "Plan saved successfully!", "OK");
        }
    }

    private async void OnLoadClicked(object sender, EventArgs e)
    {
        var plans = await _viewModel.GetSavedCalculationsAsync();
        if (plans.Count == 0)
        {
            await DisplayAlert("No Plans", "No saved cardio plans found.", "OK");
            return;
        }

        var action = await DisplayActionSheet(
            "Load Cardio Plan",
            "Cancel",
            null,
            plans.Select(p => p.Name).ToArray());

        if (action != null && action != "Cancel")
        {
            var plan = plans.FirstOrDefault(p => p.Name == action);
            if (plan != null)
            {
                await _viewModel.LoadCalculationAsync(plan.Id);
                UpdateActivityUI();
                UpdateButtonStates();
            }
        }
    }

    private void UpdateButtonStates()
    {
        WalkingButton.BackgroundColor = _viewModel.ActivityType == "Walking" ? Color.Parse("#4ECDC4") : Color.Parse("#E0E0E0");
        WalkingButton.TextColor = _viewModel.ActivityType == "Walking" ? Color.Parse("White") : Color.Parse("#666666");
        
        RunningButton.BackgroundColor = _viewModel.ActivityType == "Running" ? Color.Parse("#4ECDC4") : Color.Parse("#E0E0E0");
        RunningButton.TextColor = _viewModel.ActivityType == "Running" ? Color.Parse("White") : Color.Parse("#666666");
        
        CyclingButton.BackgroundColor = _viewModel.ActivityType == "Cycling" ? Color.Parse("#4ECDC4") : Color.Parse("#E0E0E0");
        CyclingButton.TextColor = _viewModel.ActivityType == "Cycling" ? Color.Parse("White") : Color.Parse("#666666");
    }
}
