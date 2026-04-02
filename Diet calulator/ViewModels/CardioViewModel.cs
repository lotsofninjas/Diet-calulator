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
        private double _speed = 5.0;
        private double _incline = 0.0;
        private double _watts = 100;
        
        // Results
        private int _caloriesBurned = 0;

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

        public double Speed
        {
            get => _speed;
            set { SetProperty(ref _speed, value); RecalculateAll(); }
        }

        public double Incline
        {
            get => _incline;
            set { SetProperty(ref _incline, value); RecalculateAll(); }
        }

        public double Watts
        {
            get => _watts;
            set { SetProperty(ref _watts, value); RecalculateAll(); }
        }

        public int CaloriesBurned
        {
            get => _caloriesBurned;
            set { SetProperty(ref _caloriesBurned, value); }
        }

        public string WeightUnit => AppSettings.GetWeightUnit();

        public double SpeedDisplay => AppSettings.GetDistanceUnit() == "miles" 
            ? _speed / 1.60934 
            : _speed;

        public List<string> ActivityTypes => new() { "Walking", "Running", "Cycling" };
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
            // Konvertera vikt till kg om behövs
            double weightKg = AppSettings.ConvertToKg(_weight);

            // Konvertera hastighet från användarens enhet till km/h
            double speedKmh = AppSettings.GetDistanceUnit() == "miles" 
                ? _speed * 1.60934 
                : _speed;

            double calories = ActivityType switch
            {
                "Walking" => _incline > 0 
                    ? CardioCalculator.WalkingKcalIncline(speedKmh, _incline, weightKg, _duration)
                    : CardioCalculator.WalkingKcal(speedKmh, weightKg, _duration),
                "Running" => _incline > 0 
                    ? CardioCalculator.RunningKcalIncline(speedKmh, _incline, weightKg, _duration)
                    : CardioCalculator.RunningKcal(speedKmh, weightKg, _duration),
                "Cycling" => CardioCalculator.CyclingKcal(_watts, weightKg, _duration),
                _ => 0
            };

            CaloriesBurned = (int)Math.Round(calories);
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
                    { "Speed", _speed.ToString() },
                    { "Incline", _incline.ToString() },
                    { "Watts", _watts.ToString() }
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
                _speed = double.TryParse(calculation.Data.GetValueOrDefault("Speed"), out var s) ? s : _speed;
                _incline = double.TryParse(calculation.Data.GetValueOrDefault("Incline"), out var inc) ? inc : _incline;
                _watts = double.TryParse(calculation.Data.GetValueOrDefault("Watts"), out var wt) ? wt : _watts;

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
                    { "Speed", _speed.ToString() },
                    { "Incline", _incline.ToString() },
                    { "Watts", _watts.ToString() }
                }
            };

            await _storageService.SaveCalculationAsync(calculation);
        }

        public async Task<List<SavedCalculation>> GetSavedCalculationsAsync()
        {
            return await _storageService.GetCalculationsAsync("Cardio");
        }

        public async Task DeleteCalculationAsync(String id)
        {
            await _storageService.DeleteCalculationAsync(id);
        }

        public async Task LoadCalculationAsync(String id)
        {
            var calculation = await _storageService.GetCalculationAsync(id);
            if (calculation?.Data != null)
            {
                _activityType = calculation.Data.GetValueOrDefault("ActivityType") ?? _activityType;
                _weight = double.TryParse(calculation.Data.GetValueOrDefault("Weight"), out var w) ? w : _weight;
                _duration = int.TryParse(calculation.Data.GetValueOrDefault("Duration"), out var d) ? d : _duration;
                _speed = double.TryParse(calculation.Data.GetValueOrDefault("Speed"), out var s) ? s : _speed;
                _incline = double.TryParse(calculation.Data.GetValueOrDefault("Incline"), out var inc) ? inc : _incline;
                _watts = double.TryParse(calculation.Data.GetValueOrDefault("Watts"), out var wt) ? wt : _watts;

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

        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
