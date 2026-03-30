using Diet_calulator.ViewModels;
using Diet_calulator.Models;

namespace Diet_calulator.Views
{
    public partial class MacroPage : ContentPage
    {
        private MacroViewModel _viewModel;
        private Action<string>? _currentInputHandler;
        private string? _currentInputType;

        public MacroPage()
        {
            InitializeComponent();
            _viewModel = new MacroViewModel();
            BindingContext = _viewModel;
            
            HiddenEntry.TextChanged += (s, e) => 
            {
                if (!string.IsNullOrEmpty(e.NewTextValue))
                {
                    _currentInputHandler?.Invoke(e.NewTextValue);
                }
            };
            
            HiddenEntry.Unfocused += (s, e) =>
            {
                HiddenEntry.Text = string.Empty;
            };

            // Subscribe to property changes to update UI visibility
            _viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "Mode" || e.PropertyName == "MacroInputMode" || e.PropertyName == "EatingPattern")
                {
                    UpdateUIVisibility();
                }
            };

            UpdateUIVisibility();
        }

        private void UpdateUIVisibility()
        {
            // Show custom macros only if MacroInputMode is "Custom"
            CustomMacroSection.IsVisible = _viewModel.MacroInputMode == "Custom";
            PresetSection.IsVisible = _viewModel.MacroInputMode == "Preset";

            // Show snacks and change per week only for Bulk/Cut
            SnacksSection.IsVisible = _viewModel.Mode == "Bulk";
            ChangePerWeekSection.IsVisible = _viewModel.Mode != "Maintain";

            // Update change per week picker options based on mode
            if (_viewModel.Mode == "Bulk")
            {
                ChangePerWeekPicker.ItemsSource = _viewModel.BulkChangeOptions;
            }
            else if (_viewModel.Mode == "Cut")
            {
                ChangePerWeekPicker.ItemsSource = _viewModel.CutChangeOptions;
            }

            // Show training/rest day sections only if eating pattern is "TrainingDays"
            TrainingDaySection.IsVisible = _viewModel.EatingPattern == "Training Days";
            RestDaySection.IsVisible = _viewModel.EatingPattern == "Training Days";
        }

        private async void OnInfoClicked(object sender, EventArgs e)
        {
            await DisplayAlert(
                "How to Use - Macro Calculator",
                "1. Select your GOAL:\n" +
                "   • Bulk: Gain weight with surplus calories\n" +
                "   • Maintain: Keep current weight\n" +
                "   • Cut: Lose weight with deficit calories\n\n" +
                "2. Choose INPUT MODE:\n" +
                "   • Preset: Use predefined macro distributions\n" +
                "   • Custom: Set your own protein/fat ratios\n\n" +
                "3. Enter your WEIGHT and METABOLISM\n\n" +
                "4. Choose EATING PATTERN:\n" +
                "   • Same Everyday: Same macros every day\n" +
                "   • Training Days: More calories on training days\n\n" +
                "The calculator will compute your daily macros!",
                "Got it");
        }

        private void OnWeightTapped(object sender, EventArgs e)
        {
            _currentInputType = "Weight";
            HiddenEntry.Text = string.Empty;
            _currentInputHandler = (value) =>
            {
                if (!string.IsNullOrEmpty(value) && double.TryParse(value, out var weight))
                {
                    _viewModel.Weight = weight;
                }
            };
            MainThread.BeginInvokeOnMainThread(() => HiddenEntry.Focus());
        }

        private void OnMetabolismTapped(object sender, EventArgs e)
        {
            _currentInputType = "Metabolism";
            HiddenEntry.Text = string.Empty;
            _currentInputHandler = (value) =>
            {
                if (!string.IsNullOrEmpty(value) && double.TryParse(value, out var metabolism))
                {
                    _viewModel.Metabolism = metabolism;
                }
            };
            MainThread.BeginInvokeOnMainThread(() => HiddenEntry.Focus());
        }

        private void OnProteinPerKgTapped(object sender, EventArgs e)
        {
            _currentInputType = "ProteinPerKg";
            HiddenEntry.Text = string.Empty;
            _currentInputHandler = (value) =>
            {
                if (!string.IsNullOrEmpty(value) && double.TryParse(value, out var protein))
                {
                    _viewModel.ProteinPerKg = protein;
                }
            };
            MainThread.BeginInvokeOnMainThread(() => HiddenEntry.Focus());
        }

        private void OnFatPerKgTapped(object sender, EventArgs e)
        {
            _currentInputType = "FatPerKg";
            HiddenEntry.Text = string.Empty;
            _currentInputHandler = (value) =>
            {
                if (!string.IsNullOrEmpty(value) && double.TryParse(value, out var fat))
                {
                    _viewModel.FatPerKg = fat;
                }
            };
            MainThread.BeginInvokeOnMainThread(() => HiddenEntry.Focus());
        }

        private void OnCardioTapped(object sender, EventArgs e)
        {
            _currentInputType = "Cardio";
            HiddenEntry.Text = string.Empty;
            _currentInputHandler = (value) =>
            {
                if (!string.IsNullOrEmpty(value) && int.TryParse(value, out var cardio))
                {
                    _viewModel.Cardio = cardio;
                }
                else if (string.IsNullOrEmpty(value))
                {
                    _viewModel.Cardio = 0;
                }
            };
            MainThread.BeginInvokeOnMainThread(() => HiddenEntry.Focus());
        }

        private void OnSnacksTapped(object sender, EventArgs e)
        {
            _currentInputType = "Snacks";
            HiddenEntry.Text = string.Empty;
            _currentInputHandler = (value) =>
            {
                if (!string.IsNullOrEmpty(value) && int.TryParse(value, out var snacks))
                {
                    _viewModel.Snacks = snacks;
                }
                else if (string.IsNullOrEmpty(value))
                {
                    _viewModel.Snacks = 0;
                }
            };
            MainThread.BeginInvokeOnMainThread(() => HiddenEntry.Focus());
        }

        private void OnChangePerWeekTapped(object sender, EventArgs e)
        {
            _currentInputType = "ChangePerWeek";
            HiddenEntry.Text = string.Empty;
            _currentInputHandler = (value) =>
            {
                if (!string.IsNullOrEmpty(value))
                {
                    // Remove unit (kg or %)
                    string cleanValue = value.Replace("kg", "").Replace("%", "").Trim();
                    if (double.TryParse(cleanValue, out var change))
                    {
                        _viewModel.ChangePerWeek = change;
                    }
                }
            };
            MainThread.BeginInvokeOnMainThread(() => HiddenEntry.Focus());
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            var name = await DisplayPromptAsync(
                "Save Calculation",
                "Enter a name for this calculation:",
                "Save",
                "Cancel",
                "e.g., My Bulk Plan");

            if (!string.IsNullOrWhiteSpace(name))
            {
                await _viewModel.SaveCalculationAsync(name);
                await DisplayAlert("Success", "Calculation saved successfully", "OK");
            }
        }

        private async void OnLoadClicked(object sender, EventArgs e)
        {
            try
            {
                var calculations = await _viewModel.GetSavedCalculationsAsync();

                if (calculations.Count == 0)
                {
                    await DisplayAlert("No Calculations", "You haven't saved any calculations yet", "OK");
                    return;
                }

                await ShowLoadDialog(calculations);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to load calculations: {ex.Message}", "OK");
            }
        }

        private async Task ShowLoadDialog(List<SavedCalculation> calculations)
        {
            var content = new VerticalStackLayout { Padding = 20, Spacing = 10 };

            foreach (var calc in calculations)
            {
                var itemLayout = new Grid
                {
                    Padding = 10,
                    BackgroundColor = Colors.White,
                    ColumnDefinitions = new ColumnDefinitionCollection
                    {
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                        new ColumnDefinition { Width = GridLength.Auto }
                    },
                    RowDefinitions = new RowDefinitionCollection
                    {
                        new RowDefinition { Height = GridLength.Auto }
                    }
                };

                var labelStack = new VerticalStackLayout
                {
                    Spacing = 2,
                    Padding = 0
                };

                labelStack.Add(new Label
                {
                    Text = calc.Name,
                    FontSize = 14,
                    FontAttributes = FontAttributes.Bold
                });

                labelStack.Add(new Label
                {
                    Text = calc.SavedDate.ToString("d"),
                    FontSize = 12,
                    TextColor = Colors.Gray
                });

                itemLayout.Add(labelStack, 0, 0);

                var deleteBtn = new Button
                {
                    Text = "X",
                    BackgroundColor = Colors.Red,
                    TextColor = Colors.White,
                    FontSize = 16,
                    FontAttributes = FontAttributes.Bold,
                    Padding = new Thickness(0),
                    WidthRequest = 40,
                    HeightRequest = 40,
                    VerticalOptions = LayoutOptions.Center
                };

                var calcId = calc.Id;
                var calcName = calc.Name;
                deleteBtn.Clicked += async (s, e) =>
                {
                    var confirm = await DisplayAlert(
                        "Delete?",
                        $"Are you sure you want to delete '{calcName}'?",
                        "Yes",
                        "No");

                    if (confirm)
                    {
                        try
                        {
                            await _viewModel.DeleteCalculationAsync(calcId);
                            await DisplayAlert("Deleted", "Calculation deleted successfully", "OK");
                            await Navigation.PopModalAsync();
                            var updated = await _viewModel.GetSavedCalculationsAsync();
                            if (updated.Count > 0)
                            {
                                await ShowLoadDialog(updated);
                            }
                        }
                        catch (Exception ex)
                        {
                            await DisplayAlert("Error", $"Failed to delete: {ex.Message}", "OK");
                        }
                    }
                };

                itemLayout.Add(deleteBtn, 1, 0);

                var tapGesture = new TapGestureRecognizer();
                var calculation = calc;
                tapGesture.Tapped += async (s, e) =>
                {
                    try
                    {
                        await _viewModel.LoadCalculationAsync(calculation.Id);
                        await Navigation.PopModalAsync();
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("Error", $"Failed to load: {ex.Message}", "OK");
                    }
                };

                labelStack.GestureRecognizers.Add(tapGesture);

                content.Add(itemLayout);
            }

            var scrollView = new ScrollView { Content = content };

            var page = new ContentPage
            {
                Title = "Load Calculation",
                Content = scrollView,
                BackgroundColor = Color.Parse("#F5F5F5")
            };

            await Navigation.PushModalAsync(page);
        }
    }
}
