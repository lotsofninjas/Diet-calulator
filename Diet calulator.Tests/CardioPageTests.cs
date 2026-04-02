using Diet_calulator.Services;
using Diet_calulator.ViewModels;
using Xunit;

namespace Diet_calulator.Tests
{
    public class CardioPageTests
    {
        [Fact]
        public void AppSettings_GetDistanceUnit_ReturnsKm_ByDefault()
        {
            // Arrange & Act
            var distanceUnit = AppSettings.GetDistanceUnit();

            // Assert
            Assert.NotNull(distanceUnit);
            Assert.True(distanceUnit == "km" || distanceUnit == "Miles", 
                $"Distance unit should be 'km' or 'Miles', but got '{distanceUnit}'");
        }

        [Fact]
        public void AppSettings_GetDistanceUnit_ReturnsMiles_WhenSet()
        {
            // Arrange
            Preferences.Default.Set("distance_unit", "Miles");

            // Act
            var distanceUnit = AppSettings.GetDistanceUnit();

            // Assert
            Assert.Equal("Miles", distanceUnit);
        }

        [Fact]
        public void AppSettings_GetDistanceUnit_ReturnsKm_WhenSet()
        {
            // Arrange
            Preferences.Default.Set("distance_unit", "km");

            // Act
            var distanceUnit = AppSettings.GetDistanceUnit();

            // Assert
            Assert.Equal("km", distanceUnit);
        }

        [Fact]
        public void CardioViewModel_SpeedDisplay_ShowsMph_WhenDistanceUnitIsMiles()
        {
            // Arrange
            Preferences.Default.Set("distance_unit", "Miles");
            var viewModel = new CardioViewModel();
            viewModel.Speed = 8.05; // 5 mph converted to km/h

            // Act
            var speedDisplay = viewModel.SpeedDisplay;

            // Assert
            Assert.True(speedDisplay >= 4.9 && speedDisplay <= 5.1, 
                $"SpeedDisplay should be ~5.0 mph, but got {speedDisplay}");
        }

        [Fact]
        public void CardioViewModel_SpeedDisplay_ShowsKmh_WhenDistanceUnitIsKm()
        {
            // Arrange
            Preferences.Default.Set("distance_unit", "km");
            var viewModel = new CardioViewModel();
            viewModel.Speed = 5.0; // 5 km/h

            // Act
            var speedDisplay = viewModel.SpeedDisplay;

            // Assert
            Assert.Equal(5.0, speedDisplay);
        }

        [Fact]
        public void CardioViewModel_SpeedDisplay_ConvertsCorrectly_KmhToMph()
        {
            // Arrange
            Preferences.Default.Set("distance_unit", "Miles");
            var viewModel = new CardioViewModel();
            viewModel.Speed = 16.09; // 10 mph in km/h

            // Act
            var speedDisplay = viewModel.SpeedDisplay;

            // Assert
            Assert.True(speedDisplay >= 9.9 && speedDisplay <= 10.1, 
                $"SpeedDisplay should convert 16.09 km/h to ~10.0 mph, but got {speedDisplay}");
        }

        [Fact]
        public void CardioViewModel_SpeedDisplay_ConvertsCorrectly_MphToKmh()
        {
            // Arrange
            Preferences.Default.Set("distance_unit", "Miles");
            var viewModel = new CardioViewModel();
            viewModel.Speed = 10.0; // Stored in km/h

            // Act
            var speedDisplay = viewModel.SpeedDisplay;

            // Assert
            var expectedMph = 10.0 / 1.60934; // ~6.21 mph
            Assert.True(speedDisplay >= 6.2 && speedDisplay <= 6.3, 
                $"SpeedDisplay should convert 10.0 km/h to ~6.21 mph, but got {speedDisplay}");
        }
    }
}
