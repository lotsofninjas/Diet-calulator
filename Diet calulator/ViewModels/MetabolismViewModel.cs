using System.ComponentModel;
using System.Runtime.CompilerServices;
using Diet_calulator.Models;
using Diet_calulator.Services;

namespace Diet_calulator.ViewModels
{
    public class MetabolismViewModel : INotifyPropertyChanged
    {
        private double _startWeight = 80;
        private double _endWeight = 75;
        private int _weeks = 2;
        private int _kcalPerDay = 2000;
        private int _cardioKcal = 0;
        private int _metabolism = 0;
        private string _errorMessage = "";
        private string _weightUnit = "kg";
        private readonly IStorageService _storageService;

        public event PropertyChangedEventHandler? PropertyChanged;

        public double StartWeight
        {
            get => _startWeight;
            set { SetProperty(ref _startWeight, value); CalculateMetabolism(); }
        }

        public double EndWeight
        {
            get => _endWeight;
            set { SetProperty(ref _endWeight, value); CalculateMetabolism(); }
        }

        public int Weeks
        {
            get => _weeks;
            set { SetProperty(ref _weeks, value); CalculateMetabolism(); }
        }

        public int KcalPerDay
        {
            get => _kcalPerDay;
            set { SetProperty(ref _kcalPerDay, value); CalculateMetabolism(); }
        }

        public int CardioKcal
        {
            get => _cardioKcal;
            set { SetProperty(ref _cardioKcal, value); CalculateMetabolism(); }
        }

        public int Metabolism
        {
            get => _metabolism;
            set => SetProperty(ref _metabolism, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public string WeightUnit
        {
            get => _weightUnit;
            set { SetProperty(ref _weightUnit, value); CalculateMetabolism(); }
        }

        public List<int> WeekOptions => new() { 2, 3, 4 };
        public List<string> WeightUnitOptions => new() { "kg", "lbs" };

        public MetabolismViewModel()
        {
            _storageService = new FileStorageService();
        }

        private void CalculateMetabolism()
        {
            ErrorMessage = "";

            // Validate input values
            if (_startWeight <= 0 || _endWeight <= 0 || _weeks <= 0 || _kcalPerDay <= 0)
            {
                Metabolism = 0;
                return;
            }

            try
            {
                double totalDays = _weeks * 7.0;
                double averageWeight = (_startWeight + _endWeight) / 2.0;
                double totalKcalInput;
                double metabolism;

                // Constants for weight change (per unit in current weight unit)
                double kcalPerUnitFatLoss;
                double kcalPerUnitMuscleGain;

                if (_weightUnit == "kg")
                {
                    kcalPerUnitFatLoss = 7700.0;    // 1 kg fat = 7700 kcal
                    kcalPerUnitMuscleGain = 5150.0; // 1 kg muscle = 5150 kcal
                }
                else // lbs
                {
                    kcalPerUnitFatLoss = 7700.0 / 2.20462;    // 1 lbs fat ? 3492 kcal
                    kcalPerUnitMuscleGain = 5150.0 / 2.20462; // 1 lbs muscle ? 2336 kcal
                }

                if (_startWeight > _endWeight)
                {
                    // Weight loss
                    double weightChange = _startWeight - _endWeight;
                    // Calculate kcal deficit from weight loss
                    double kcalDeficit = weightChange * kcalPerUnitFatLoss;
                    // Cardio reduces the deficit (adds to actual expenditure)
                    double cardioTotal = _cardioKcal * _weeks;
                    // Actual total calories consumed
                    double kcalEaten = _kcalPerDay * totalDays;
                    // Total energy expended = eaten + deficit + cardio
                    double totalExpended = kcalEaten + kcalDeficit + cardioTotal;
                    
                    metabolism = totalExpended / totalDays / averageWeight;
                }
                else
                {
                    // Weight gain
                    double weightChange = _endWeight - _startWeight;
                    // Calculate kcal surplus needed for weight gain
                    double kcalSurplus = weightChange * kcalPerUnitMuscleGain;
                    double cardioTotal = _cardioKcal * _weeks;
                    // Total calories consumed
                    double kcalEaten = _kcalPerDay * totalDays;
                    // Total energy expended = eaten - surplus + cardio
                    double totalExpended = kcalEaten - kcalSurplus + cardioTotal;
                    
                    metabolism = totalExpended / totalDays / averageWeight;
                }

                Metabolism = (int)Math.Round(metabolism);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error during calculation: {ex.Message}";
                Metabolism = 0;
            }
        }

        public async Task SaveCalculationAsync(string name)
        {
            try
            {
                var calculation = new SavedCalculation(name, "Metabolism")
                {
                    Data = new Dictionary<string, string>
                    {
                        { "StartWeight", _startWeight.ToString() },
                        { "EndWeight", _endWeight.ToString() },
                        { "Weeks", _weeks.ToString() },
                        { "KcalPerDay", _kcalPerDay.ToString() },
                        { "CardioKcal", _cardioKcal.ToString() },
                        { "Metabolism", _metabolism.ToString() },
                        { "WeightUnit", _weightUnit }
                    }
                };

                await _storageService.SaveCalculationAsync(calculation);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error saving calculation: {ex.Message}";
            }
        }

        public async Task LoadCalculationAsync(string id)
        {
            try
            {
                var calculation = await _storageService.GetCalculationAsync(id);
                if (calculation != null)
                {
                    StartWeight = double.Parse(calculation.Data["StartWeight"]);
                    EndWeight = double.Parse(calculation.Data["EndWeight"]);
                    Weeks = int.Parse(calculation.Data["Weeks"]);
                    KcalPerDay = int.Parse(calculation.Data["KcalPerDay"]);
                    CardioKcal = int.Parse(calculation.Data["CardioKcal"]);
                    WeightUnit = calculation.Data["WeightUnit"];
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading calculation: {ex.Message}";
            }
        }

        public async Task<List<SavedCalculation>> GetSavedCalculationsAsync()
        {
            return await _storageService.GetCalculationsAsync("Metabolism");
        }

        public async Task DeleteCalculationAsync(string id)
        {
            await _storageService.DeleteCalculationAsync(id);
        }

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string? propertyName = null)
        {
            if (Equals(storage, value))
                return false;

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
