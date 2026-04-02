using System.ComponentModel;
using System.Runtime.CompilerServices;
using Diet_calulator.Models;
using Diet_calulator.Services;
using Diet_calulator.Constants;

namespace Diet_calulator.ViewModels
{
    public class MacroViewModel : INotifyPropertyChanged
    {
        // Mode selection
        private string _mode = AppConstants.Mode_Maintain;
        private string _macroInputMode = AppConstants.MacroInputMode_Preset;
        private string _eatingPattern = AppConstants.EatingPattern_SameEveryday;
        private string _trainingPattern = AppConstants.TrainingPattern_EveryOtherDay;
        private int _carbsMore = 0;
        
        // User inputs
        private double _weight = 80;
        private int _metabolism = 25;
        private double _proteinPerKg = 2.0;
        private double _fatPerKg = 1.0;
        private int _cardio = 0;
        private int _snacks = 0;
        private int _trainingDaysPerWeek = 4;
        private double _changePerWeek = 0.35;
        
        // Preset selection
        private string _presetProfile = "Balanced";
        
        // Results
        private int _dailyCalories = 0;
        private int _trainingDayCalories = 0;
        private int _restDayCalories = 0;
        
        private int _protein = 0;
        private int _carbs = 0;
        private int _fat = 0;
        
        private int _trainingDayProtein = 0;
        private int _trainingDayCarbs = 0;
        private int _trainingDayFat = 0;
        
        private int _restDayProtein = 0;
        private int _restDayCarbs = 0;
        private int _restDayFat = 0;

        private readonly IStorageService _storageService;

        public event PropertyChangedEventHandler? PropertyChanged;

        #region Properties
        public string Mode
        {
            get => _mode;
            set { SetProperty(ref _mode, value); RecalculateAll(); }
        }

        public string MacroInputMode
        {
            get => _macroInputMode;
            set { SetProperty(ref _macroInputMode, value); RecalculateAll(); }
        }

        public string EatingPattern
        {
            get => _eatingPattern;
            set { SetProperty(ref _eatingPattern, value); RecalculateAll(); }
        }

        public string TrainingPattern
        {
            get => _trainingPattern;
            set { SetProperty(ref _trainingPattern, value); RecalculateAll(); }
        }

        public int CarbsMore
        {
            get => _carbsMore;
            set { SetProperty(ref _carbsMore, value); RecalculateAll(); }
        }

        public double Weight
        {
            get => _weight;
            set { SetProperty(ref _weight, value); RecalculateAll(); }
        }

        public int Metabolism
        {
            get => _metabolism;
            set { SetProperty(ref _metabolism, value); RecalculateAll(); }
        }

        public double ProteinPerKg
        {
            get => _proteinPerKg;
            set { SetProperty(ref _proteinPerKg, value); RecalculateAll(); }
        }

        public double FatPerKg
        {
            get => _fatPerKg;
            set { SetProperty(ref _fatPerKg, value); RecalculateAll(); }
        }

        public int Cardio
        {
            get => _cardio;
            set { SetProperty(ref _cardio, value); RecalculateAll(); }
        }

        public int Snacks
        {
            get => _snacks;
            set { SetProperty(ref _snacks, value); RecalculateAll(); }
        }

        public int TrainingDaysPerWeek
        {
            get => _trainingDaysPerWeek;
            set { SetProperty(ref _trainingDaysPerWeek, value); RecalculateAll(); }
        }

        public double ChangePerWeek
        {
            get => _changePerWeek;
            set { SetProperty(ref _changePerWeek, value); RecalculateAll(); }
        }

        public string PresetProfile
        {
            get => _presetProfile;
            set { SetProperty(ref _presetProfile, value); RecalculateAll(); }
        }

        public int DailyCalories
        {
            get => _dailyCalories;
            set { SetProperty(ref _dailyCalories, value); }
        }

        public int TrainingDayCalories
        {
            get => _trainingDayCalories;
            set { SetProperty(ref _trainingDayCalories, value); }
        }

        public int RestDayCalories
        {
            get => _restDayCalories;
            set { SetProperty(ref _restDayCalories, value); }
        }

        public int Protein
        {
            get => _protein;
            set { SetProperty(ref _protein, value); }
        }

        public int Carbs
        {
            get => _carbs;
            set { SetProperty(ref _carbs, value); }
        }

        public int Fat
        {
            get => _fat;
            set { SetProperty(ref _fat, value); }
        }

        public int TrainingDayProtein
        {
            get => _trainingDayProtein;
            set { SetProperty(ref _trainingDayProtein, value); }
        }

        public int TrainingDayCarbs
        {
            get => _trainingDayCarbs;
            set { SetProperty(ref _trainingDayCarbs, value); }
        }

        public int TrainingDayFat
        {
            get => _trainingDayFat;
            set { SetProperty(ref _trainingDayFat, value); }
        }

        public int RestDayProtein
        {
            get => _restDayProtein;
            set { SetProperty(ref _restDayProtein, value); }
        }

        public int RestDayCarbs
        {
            get => _restDayCarbs;
            set { SetProperty(ref _restDayCarbs, value); }
        }

        public int RestDayFat
        {
            get => _restDayFat;
            set { SetProperty(ref _restDayFat, value); }
        }

        public List<string> PresetProfiles => new() { "Balanced", "High Protein", "Low Carb" };
        public List<string> ModeOptions => AppConstants.Modes.ToList();
        public List<string> EatingPatternOptions => AppConstants.EatingPatterns.ToList();
        public List<string> TrainingPatternOptions => AppConstants.TrainingPatterns.ToList();
        #endregion

        public MacroViewModel(IStorageService? storageService = null)
        {
            _storageService = storageService ?? new FileStorageService();
        }

        private void RecalculateAll()
        {
            CalculateBaseCalories();
            CalculateMacros();
            CalculateTrainingAndRestDayMacros();
        }

        private void CalculateBaseCalories()
        {
            DailyCalories = (int)(Weight * Metabolism) + Cardio / 7 + Snacks / 7;

            if (Mode == AppConstants.Mode_Bulk)
            {
                DailyCalories += (int)(Weight * ChangePerWeek * 770 / 7);
            }
            else if (Mode == AppConstants.Mode_Cut)
            {
                DailyCalories -= (int)(DailyCalories * ChangePerWeek / 100 / 7);
            }
        }

        private void CalculateMacros()
        {
            if (MacroInputMode == AppConstants.MacroInputMode_Preset)
            {
                CalculatePresetMacros();
            }
            else
            {
                CalculateCustomMacros();
            }
        }

        private void CalculatePresetMacros()
        {
            Protein = (int)(Weight * (PresetProfile == "High Protein" ? 2.5 : 2.0));
            Fat = (int)(Weight * (PresetProfile == "Low Carb" ? 1.5 : 1.0));
            Carbs = (DailyCalories - (Protein * 4) - (Fat * 9)) / 4;
        }

        private void CalculateCustomMacros()
        {
            Protein = (int)(Weight * ProteinPerKg);
            Fat = (int)(Weight * FatPerKg);
            Carbs = (DailyCalories - (Protein * 4) - (Fat * 9)) / 4;
        }

        private void CalculateTrainingAndRestDayMacros()
        {
            if (_eatingPattern == AppConstants.EatingPattern_SameEveryday)
            {
                TrainingDayCalories = DailyCalories;
                RestDayCalories = DailyCalories;
                TrainingDayProtein = Protein;
                TrainingDayCarbs = Carbs;
                TrainingDayFat = Fat;
                RestDayProtein = Protein;
                RestDayCarbs = Carbs;
                RestDayFat = Fat;
            }
            else
            {
                int totalWeeklyCalories = DailyCalories * 7;
                int trainingDayCount = GetTrainingDaysCount();
                int restDayCount = 7 - trainingDayCount;

                TrainingDayCalories = (totalWeeklyCalories + (CarbsMore * trainingDayCount * 4)) / 7;
                RestDayCalories = (totalWeeklyCalories - (CarbsMore * trainingDayCount * 4)) / 7;

                TrainingDayProtein = Protein;
                TrainingDayFat = Fat;
                TrainingDayCarbs = (TrainingDayCalories - (TrainingDayProtein * 4) - (TrainingDayFat * 9)) / 4;

                RestDayProtein = Protein;
                RestDayFat = Fat;
                RestDayCarbs = (RestDayCalories - (RestDayProtein * 4) - (RestDayFat * 9)) / 4;
            }
        }

        private int GetTrainingDaysCount()
        {
            return TrainingPattern switch
            {
                AppConstants.TrainingPattern_EveryOtherDay => 3,
                AppConstants.TrainingPattern_2On1Off => 4,
                AppConstants.TrainingPattern_3On1Off => 5,
                _ => int.TryParse(TrainingPattern, out var days) ? days : 4
            };
        }

        public async Task SaveAsync()
        {
            var calculation = new SavedCalculation
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Current",
                Data = new Dictionary<string, string>
                {
                    { "Mode", _mode },
                    { "Weight", _weight.ToString() },
                    { "Metabolism", _metabolism.ToString() },
                    { "PresetProfile", _presetProfile },
                    { "MacroInputMode", _macroInputMode },
                    { "ProteinPerKg", _proteinPerKg.ToString() },
                    { "FatPerKg", _fatPerKg.ToString() },
                    { "Cardio", _cardio.ToString() },
                    { "EatingPattern", _eatingPattern },
                    { "TrainingPattern", _trainingPattern },
                    { "CarbsMore", _carbsMore.ToString() },
                    { "Snacks", _snacks.ToString() },
                    { "ChangePerWeek", _changePerWeek.ToString() }
                }
            };

            await _storageService.SaveCalculationAsync(calculation);
        }

        public async Task LoadAsync()
        {
            var calculations = await _storageService.GetCalculationsAsync("Current");
            var calculation = calculations.FirstOrDefault();
            
            if (calculation?.Data != null)
            {
                _mode = calculation.Data.GetValueOrDefault("Mode") ?? _mode;
                _weight = double.TryParse(calculation.Data.GetValueOrDefault("Weight"), out var w) ? w : _weight;
                _metabolism = int.TryParse(calculation.Data.GetValueOrDefault("Metabolism"), out var m) ? m : _metabolism;
                _presetProfile = calculation.Data.GetValueOrDefault("PresetProfile") ?? _presetProfile;
                _macroInputMode = calculation.Data.GetValueOrDefault("MacroInputMode") ?? _macroInputMode;
                _proteinPerKg = double.TryParse(calculation.Data.GetValueOrDefault("ProteinPerKg"), out var p) ? p : _proteinPerKg;
                _fatPerKg = double.TryParse(calculation.Data.GetValueOrDefault("FatPerKg"), out var f) ? f : _fatPerKg;
                _cardio = int.TryParse(calculation.Data.GetValueOrDefault("Cardio"), out var c) ? c : _cardio;
                _eatingPattern = calculation.Data.GetValueOrDefault("EatingPattern") ?? _eatingPattern;
                _trainingPattern = calculation.Data.GetValueOrDefault("TrainingPattern") ?? _trainingPattern;
                _carbsMore = int.TryParse(calculation.Data.GetValueOrDefault("CarbsMore"), out var cm) ? cm : _carbsMore;
                _snacks = int.TryParse(calculation.Data.GetValueOrDefault("Snacks"), out var s) ? s : _snacks;
                _changePerWeek = double.TryParse(calculation.Data.GetValueOrDefault("ChangePerWeek"), out var ch) ? ch : _changePerWeek;

                RecalculateAll();
            }
        }

        public async Task SaveCalculationAsync(string name)
        {
            var calculation = new SavedCalculation
            {
                Id = Guid.NewGuid().ToString(),
                Name = name,
                Data = new Dictionary<string, string>
                {
                    { "Mode", _mode },
                    { "Weight", _weight.ToString() },
                    { "Metabolism", _metabolism.ToString() },
                    { "PresetProfile", _presetProfile },
                    { "MacroInputMode", _macroInputMode },
                    { "ProteinPerKg", _proteinPerKg.ToString() },
                    { "FatPerKg", _fatPerKg.ToString() },
                    { "Cardio", _cardio.ToString() },
                    { "EatingPattern", _eatingPattern },
                    { "TrainingPattern", _trainingPattern },
                    { "CarbsMore", _carbsMore.ToString() },
                    { "Snacks", _snacks.ToString() },
                    { "ChangePerWeek", _changePerWeek.ToString() }
                }
            };

            await _storageService.SaveCalculationAsync(calculation);
        }

        public async Task<List<SavedCalculation>> GetSavedCalculationsAsync()
        {
            return await _storageService.GetCalculationsAsync("Current");
        }

        public async Task DeleteCalculationAsync(string id)
        {
            await _storageService.DeleteCalculationAsync(id);
        }

        public async Task LoadCalculationAsync(string id)
        {
            var calculation = await _storageService.GetCalculationAsync(id);
            if (calculation?.Data != null)
            {
                _mode = calculation.Data.GetValueOrDefault("Mode") ?? _mode;
                _weight = double.TryParse(calculation.Data.GetValueOrDefault("Weight"), out var w) ? w : _weight;
                _metabolism = int.TryParse(calculation.Data.GetValueOrDefault("Metabolism"), out var m) ? m : _metabolism;
                _presetProfile = calculation.Data.GetValueOrDefault("PresetProfile") ?? _presetProfile;
                _macroInputMode = calculation.Data.GetValueOrDefault("MacroInputMode") ?? _macroInputMode;
                _proteinPerKg = double.TryParse(calculation.Data.GetValueOrDefault("ProteinPerKg"), out var p) ? p : _proteinPerKg;
                _fatPerKg = double.TryParse(calculation.Data.GetValueOrDefault("FatPerKg"), out var f) ? f : _fatPerKg;
                _cardio = int.TryParse(calculation.Data.GetValueOrDefault("Cardio"), out var c) ? c : _cardio;
                _eatingPattern = calculation.Data.GetValueOrDefault("EatingPattern") ?? _eatingPattern;
                _trainingPattern = calculation.Data.GetValueOrDefault("TrainingPattern") ?? _trainingPattern;
                _carbsMore = int.TryParse(calculation.Data.GetValueOrDefault("CarbsMore"), out var cm) ? cm : _carbsMore;
                _snacks = int.TryParse(calculation.Data.GetValueOrDefault("Snacks"), out var s) ? s : _snacks;
                _changePerWeek = double.TryParse(calculation.Data.GetValueOrDefault("ChangePerWeek"), out var ch) ? ch : _changePerWeek;

                RecalculateAll();
            }
        }

        protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
