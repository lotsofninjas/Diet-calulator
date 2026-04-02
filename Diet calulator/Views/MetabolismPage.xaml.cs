using Diet_calulator.ViewModels;
using Diet_calulator.Models;
using Diet_calulator.Services;

namespace Diet_calulator.Views
{
    public partial class MetabolismPage : ContentPage
    {
        private MetabolismViewModel _viewModel;
        private Action<string>? _currentInputHandler;
        private string? _currentInputType;

        public MetabolismPage()
        {
            InitializeComponent();
            _viewModel = new MetabolismViewModel();
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
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ThemeService.ApplyTheme(this);
        }

        private async void OnInfoClicked(object sender, EventArgs e)
        {
            await DisplayAlert(
                "How to Use - Metabolism Calculator",
                "1. Enter your STARTING weight at the beginning of the measurement period\n\n" +
                "2. Enter your ENDING weight after the measurement period. Note: More weeks gives more accurate results\n\n" +
                "3. Select the number of WEEKS (2, 3, or 4)\n\n" +
                "4. Enter your AVERAGE daily calorie intake. Note: The more precisely you measure your intake, the more accurate the calculation\n\n" +
                "5. (Optional) Enter total CARDIO calories burned during the period. Including cardio makes the calculation more independent of cardio activity. Leave empty if not tracking cardio\n\n" +
                "6. Your weight unit can be changed in Settings\n\n" +
                "The calculator will compute your actual metabolism based on real weight changes.",
                "Got it");
        }

        private void OnStartWeightTapped(object sender, EventArgs e)
        {
            _currentInputType = "StartWeight";
            HiddenEntry.Text = string.Empty;
            _currentInputHandler = (value) =>
            {
                if (!string.IsNullOrEmpty(value) && double.TryParse(value, out var weight))
                {
                    _viewModel.StartWeight = weight;
                }
            };
            MainThread.BeginInvokeOnMainThread(() =>
            {
                HiddenEntry.Focus();
            });
        }

        private void OnEndWeightTapped(object sender, EventArgs e)
        {
            _currentInputType = "EndWeight";
            HiddenEntry.Text = string.Empty;
            _currentInputHandler = (value) =>
            {
                if (!string.IsNullOrEmpty(value) && double.TryParse(value, out var weight))
                {
                    _viewModel.EndWeight = weight;
                }
            };
            MainThread.BeginInvokeOnMainThread(() =>
            {
                HiddenEntry.Focus();
            });
        }

        private void OnKcalTapped(object sender, EventArgs e)
        {
            _currentInputType = "Kcal";
            HiddenEntry.Text = string.Empty;
            _currentInputHandler = (value) =>
            {
                if (!string.IsNullOrEmpty(value) && int.TryParse(value, out var kcal))
                {
                    _viewModel.KcalPerDay = kcal;
                }
            };
            MainThread.BeginInvokeOnMainThread(() =>
            {
                HiddenEntry.Focus();
            });
        }

        private async void OnWeeksTapped(object sender, EventArgs e)
        {
            var result = await DisplayActionSheet(
                "Select Weeks",
                "Cancel",
                null,
                "2", "3", "4");

            if (result != null && result != "Cancel" && int.TryParse(result, out var weeks))
            {
                _viewModel.Weeks = weeks;
            }
        }

        private void OnCardioTapped(object sender, EventArgs e)
        {
            _currentInputType = "Cardio";
            HiddenEntry.Text = string.Empty;
            _currentInputHandler = (value) =>
            {
                if (!string.IsNullOrEmpty(value) && int.TryParse(value, out var cardio))
                {
                    _viewModel.CardioKcal = cardio;
                }
                else if (string.IsNullOrEmpty(value))
                {
                    _viewModel.CardioKcal = 0;
                }
            };
            MainThread.BeginInvokeOnMainThread(() =>
            {
                HiddenEntry.Focus();
            });
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            var name = await DisplayPromptAsync(
                "Save Calculation",
                "Enter a name for this calculation:",
                "Save",
                "Cancel",
                "e.g., Week 1 Measurement");

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

                // Delete button - pinned to right
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

                // Tap gesture on label stack to load (not on delete button)
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
