namespace Diet_calulator.Services
{
    public static class CardioCalculator
    {
        public static double RunningKcal(double speedKmh, double weightKg, double minutes)
        {
            double v = speedKmh * 1000.0 / 60.0; // m/min
            double vo2 = 0.2 * v + 3.5;

            double kcalPerMin = (vo2 * weightKg) / 200.0;
            return kcalPerMin * minutes;
        }

        public static double RunningKcalIncline(double speedKmh, double inclinePercent, double weightKg, double minutes)
        {
            double v = speedKmh * 1000.0 / 60.0;
            double G = inclinePercent / 100.0;

            double vo2 = 0.2 * v + 0.9 * v * G + 3.5;

            double kcalPerMin = (vo2 * weightKg) / 200.0;
            return kcalPerMin * minutes;
        }

        public static double WalkingKcal(double speedKmh, double weightKg, double minutes)
        {
            double v = speedKmh * 1000.0 / 60.0;
            double vo2 = 0.1 * v + 3.5;

            double kcalPerMin = (vo2 * weightKg) / 200.0;
            return kcalPerMin * minutes;
        }

        public static double WalkingKcalIncline(double speedKmh, double inclinePercent, double weightKg, double minutes)
        {
            double v = speedKmh * 1000.0 / 60.0;
            double G = inclinePercent / 100.0;

            double vo2 = 0.1 * v + 1.8 * v * G + 3.5;

            double kcalPerMin = (vo2 * weightKg) / 200.0;
            return kcalPerMin * minutes;
        }

        public static double CyclingKcal(double watts, double weightKg, double minutes)
        {
            double vo2 = (1.8 * watts / weightKg) + 7.0;

            double kcalPerMin = (vo2 * weightKg) / 200.0;
            return kcalPerMin * minutes;
        }

        /// <summary>
        /// Converts pounds to kilograms
        /// </summary>
        public static double LbsToKg(double lbs)
        {
            return lbs / 2.20462;
        }

        /// <summary>
        /// Converts kilograms to pounds
        /// </summary>
        public static double KgToLbs(double kg)
        {
            return kg * 2.20462;
        }
    }
}
