using Microsoft.Maui.Storage;

namespace Diet_calulator.Services
{
    public static class AppSettings
    {
        private const string WeightUnitKey = "weight_unit";
        private const string DistanceUnitKey = "distance_unit";
        private const string DefaultWeightUnit = "kg";
        private const string DefaultDistanceUnit = "km";

        private static string _weightUnit = DefaultWeightUnit;
        private static string _distanceUnit = DefaultDistanceUnit;
        private static bool _isInitialized = false;

        /// <summary>
        /// Initialize settings - must be called during app startup
        /// </summary>
        public static void Initialize()
        {
            if (_isInitialized)
                return;

            try
            {
                _weightUnit = Preferences.Get(WeightUnitKey, DefaultWeightUnit);
                _distanceUnit = Preferences.Get(DistanceUnitKey, DefaultDistanceUnit);
                _isInitialized = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error initializing AppSettings: {ex.Message}");
                _weightUnit = DefaultWeightUnit;
                _distanceUnit = DefaultDistanceUnit;
                _isInitialized = true;
            }
        }

        /// <summary>
        /// Gets the global weight unit preference (kg or lbs)
        /// </summary>
        public static string GetWeightUnit()
        {
            if (!_isInitialized)
                Initialize();
            return _weightUnit;
        }

        /// <summary>
        /// Sets the global weight unit preference (kg or lbs)
        /// </summary>
        public static void SetWeightUnit(string unit)
        {
            if (!_isInitialized)
                Initialize();
            _weightUnit = unit;
            try
            {
                Preferences.Set(WeightUnitKey, unit);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving weight unit: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets the global distance unit preference (km or miles)
        /// </summary>
        public static string GetDistanceUnit()
        {
            if (!_isInitialized)
                Initialize();
            return _distanceUnit;
        }

        /// <summary>
        /// Sets the global distance unit preference (km or miles)
        /// </summary>
        public static void SetDistanceUnit(string unit)
        {
            if (!_isInitialized)
                Initialize();
            _distanceUnit = unit;
            try
            {
                Preferences.Set(DistanceUnitKey, unit);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving distance unit: {ex.Message}");
            }
        }

        /// <summary>
        /// Checks if using lbs (returns true if lbs, false if kg)
        /// </summary>
        public static bool IsUsingLbs()
        {
            return GetWeightUnit() == "lbs";
        }

        /// <summary>
        /// Checks if using miles (returns true if miles, false if km)
        /// </summary>
        public static bool IsUsingMiles()
        {
            return GetDistanceUnit() == "miles";
        }

        /// <summary>
        /// Converts weight to kg if needed based on current unit setting
        /// </summary>
        public static double ConvertToKg(double weight)
        {
            return IsUsingLbs() ? CardioCalculator.LbsToKg(weight) : weight;
        }

        /// <summary>
        /// Converts weight from kg to current unit setting
        /// </summary>
        public static double ConvertFromKg(double weightKg)
        {
            return IsUsingLbs() ? CardioCalculator.KgToLbs(weightKg) : weightKg;
        }

        /// <summary>
        /// Converts distance to km if needed based on current unit setting
        /// </summary>
        public static double ConvertToKm(double distance)
        {
            return IsUsingMiles() ? distance * 1.60934 : distance;
        }

        /// <summary>
        /// Converts distance from km to current unit setting
        /// </summary>
        public static double ConvertFromKm(double distanceKm)
        {
            return IsUsingMiles() ? distanceKm / 1.60934 : distanceKm;
        }
    }
}
