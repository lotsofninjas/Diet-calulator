using System.ComponentModel;
using System.Runtime.CompilerServices;
using Diet_calulator.Models;
using Diet_calulator.Services;

namespace Diet_calulator.ViewModels
{
    public class CardioViewModel : INotifyPropertyChanged
    {
        // Activity selection
        private string _activityType = "Walking";
        
        // User inputs
        private double _weight = 75;
        private int _duration = 30;
        private int _frequency = 3;
        private double _speed = 5.0;
        private string _intensity = "Moderate";
        
        // Results
        private int _caloriesPerSession = 0;
        private int _caloriesPerWeek = 0;
        private int _caloriesPerDay = 0;

        private readonly IStorageService _storageService;

        public event PropertyChangedEventHandler? PropertyChanged;

        #region Properties
        public string ActivityType
        {
            get => _activityType;
            set { SetProperty(ref _activityType, value); RecalculateAll(); }
        }

        public double Weight
        {
            get => _weight;
            set { SetProperty(ref _weight, value); RecalculateAll(); }
        }

        public int Duration
        {
            get => _duration;
            set { SetProperty(ref _duration, value); RecalculateAll(); }
        }

        public int Frequency
        {
            get => _frequency;
            set { SetProperty(ref _frequency, value); RecalculateAll(); }
        }

        public double Speed
        {
            get => _speed;
            set { SetProperty(ref _speed, value); RecalculateAll(); }
        }

        public string Intensity
        {
            get => _intensity;
            set { SetProperty(ref _intensity, value); RecalculateAll(); }
        }

        public int CaloriesPerSession
        {
            get => _caloriesPerSession;
            set { SetProperty(ref _caloriesPerSession, value); }
        }

        public int CaloriesPerWeek
        {
            get => _caloriesPerWeek;
            set { SetProperty(ref _caloriesPerWeek, value); }
        }

        public int CaloriesPerDay
        {
            get => _caloriesPerDay;
            set { SetProperty(ref _caloriesPerDay, value); }
        }

        public List<string> ActivityTypes => new() { "Walking", "Running", "Cycling" };
        public List<string> IntensityOptions => new() { "Low", "Moderate", "High", "Very High" };
        #endregion

        public CardioViewModel(IStorageService? storageService = null)
        {
            _storageService = storageService ?? new FileStorageService();
            RecalculateAll();
        }

        private void RecalculateAll()
        {
            CalculateCalories();
        }

        private void CalculateCalories()
        {
            // MET (Metabolic Equivalent) values for different activities
            double met = GetMET();
            
            // Formula: Calories = MET * Weight (kg) * Duration (hours)
            double hours = Duration / 60.0;
            CaloriesPerSession = (int)(met * Weight * hours);
            
            CaloriesPerWeek = CaloriesPerSession * Frequency;
            CaloriesPerDay = CaloriesPerWeek / 7;
        }

        private double GetMET()
        {
            return ActivityType switch
            {
                "Walking" => GetWalkingMET(),
                "Running" => GetRunningMET(),
                "Cycling" => GetCyclingMET(),
                _ => 3.0
            };
        }

        private double GetWalkingMET()
        {
            // Walking MET values based on speed (km/h)
            return Speed switch
            {
                <= 3.0 => 2.8,    // Slow walking
                <= 4.0 => 3.5,    // Normal pace
                <= 5.0 => 4.0,    // Brisk walking
                _ => 5.0          // Very fast walking
            };
        }

        private double GetRunningMET()
        {
            // Running MET values based on speed (km/h)
            return Speed switch
            {
                <= 8.0 => 8.3,    // Slow jogging
                <= 10.0 => 9.8,   // Moderate running
                <= 12.0 => 11.0,  // Fast running
                _ => 13.5         // Very fast running
            };
        }

        private double GetCyclingMET()
        {
            return Intensity switch
            {
                "Low" => 5.8,      // Leisurely cycling
                "Moderate" => 7.5, // Moderate pace
                "High" => 10.0,    // Vigorous cycling
                "Very High" => 14.0, // Very intense
                _ => 7.5
            };
        }

        public async Task SaveAsync()
        {
            var calculation = new SavedCalculation
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Cardio Current",
                Type = "Cardio",
                Data = new Dictionary<string, string>
                {
                    { "ActivityType", _activityType },
                    { "Weight", _weight.ToString() },
                    { "Duration", _duration.ToString() },
                    { "Frequency", _frequency.ToString() },
                    { "Speed", _speed.ToString() },
                    { "Intensity", _intensity }
                }
            };

            await _storageService.SaveCalculationAsync(calculation);
        }

        public async Task LoadAsync()
        {
            var calculations = await _storageService.GetCalculationsAsync("Cardio");
            var calculation = calculations.FirstOrDefault(c => c.Name == "Cardio Current");
            
            if (calculation?.Data != null)
            {
                _activityType = calculation.Data.GetValueOrDefault("ActivityType") ?? _activityType;
                _weight = double.TryParse(calculation.Data.GetValueOrDefault("Weight"), out var w) ? w : _weight;
                _duration = int.TryParse(calculation.Data.GetValueOrDefault("Duration"), out var d) ? d : _duration;
                _frequency = int.TryParse(calculation.Data.GetValueOrDefault("Frequency"), out var f) ? f : _frequency;
                _speed = double.TryParse(calculation.Data.GetValueOrDefault("Speed"), out var s) ? s : _speed;
                _intensity = calculation.Data.GetValueOrDefault("Intensity") ?? _intensity;

                RecalculateAll();
            }
        }

        public async Task SaveCalculationAsync(string name)
        {
            var calculation = new SavedCalculation
            {
                Id = Guid.NewGuid().ToString(),
                Name = name,
                Type = "Cardio",
                Data = new Dictionary<string, string>
                {
                    { "ActivityType", _activityType },
                    { "Weight", _weight.ToString() },
                    { "Duration", _duration.ToString() },
                    { "Frequency", _frequency.ToString() },
                    { "Speed", _speed.ToString() },
                    { "Intensity", _intensity }
                }
            };

            await _storageService.SaveCalculationAsync(calculation);
        }

        public async Task<List<SavedCalculation>> GetSavedCalculationsAsync()
        {
            return await _storageService.GetCalculationsAsync("Cardio");
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
                _activityType = calculation.Data.GetValueOrDefault("ActivityType") ?? _activityType;
                _weight = double.TryParse(calculation.Data.GetValueOrDefault("Weight"), out var w) ? w : _weight;
                _duration = int.TryParse(calculation.Data.GetValueOrDefault("Duration"), out var d) ? d : _duration;
                _frequency = int.TryParse(calculation.Data.GetValueOrDefault("Frequency"), out var f) ? f : _frequency;
                _speed = double.TryParse(calculation.Data.GetValueOrDefault("Speed"), out var s) ? s : _speed;
                _intensity = calculation.Data.GetValueOrDefault("Intensity") ?? _intensity;

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
