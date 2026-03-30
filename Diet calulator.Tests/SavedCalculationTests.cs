using Diet_calulator.Models;
using Xunit;

namespace Diet_calulator.Tests
{
    public class SavedCalculationTests
    {
        [Fact]
        public void SavedCalculation_CreatesWithUniqueId()
        {
            // Arrange & Act
            var calc1 = new SavedCalculation("Test 1", "Metabolism");
            var calc2 = new SavedCalculation("Test 2", "Macro");

            // Assert
            Assert.NotEqual(calc1.Id, calc2.Id);
            Assert.False(string.IsNullOrEmpty(calc1.Id));
            Assert.False(string.IsNullOrEmpty(calc2.Id));
        }

        [Fact]
        public void SavedCalculation_StoresNameCorrectly()
        {
            // Arrange & Act
            var calc = new SavedCalculation("My Calculation", "Metabolism");

            // Assert
            Assert.Equal("My Calculation", calc.Name);
        }

        [Fact]
        public void SavedCalculation_StoresTypeCorrectly()
        {
            // Arrange & Act
            var calc = new SavedCalculation("Test", "Macro");

            // Assert
            Assert.Equal("Macro", calc.Type);
        }

        [Fact]
        public void SavedCalculation_SetsSavedDateToNow()
        {
            // Arrange
            var before = DateTime.Now;

            // Act
            var calc = new SavedCalculation("Test", "Metabolism");

            // Assert
            var after = DateTime.Now;
            Assert.True(calc.SavedDate >= before && calc.SavedDate <= after);
        }

        [Fact]
        public void SavedCalculation_DataDictionaryIsInitialized()
        {
            // Arrange & Act
            var calc = new SavedCalculation("Test", "Metabolism");

            // Assert
            Assert.NotNull(calc.Data);
            Assert.IsType<Dictionary<string, string>>(calc.Data);
        }

        [Fact]
        public void SavedCalculation_CanStoreData()
        {
            // Arrange
            var calc = new SavedCalculation("Test", "Metabolism");

            // Act
            calc.Data["StartWeight"] = "80";
            calc.Data["EndWeight"] = "75";

            // Assert
            Assert.Equal("80", calc.Data["StartWeight"]);
            Assert.Equal("75", calc.Data["EndWeight"]);
        }

        [Fact]
        public void SavedCalculation_ToStringReturnsNameAndDate()
        {
            // Arrange
            var calc = new SavedCalculation("Test Calc", "Metabolism");

            // Act
            var result = calc.ToString();

            // Assert
            Assert.Contains("Test Calc", result);
            Assert.Contains(calc.SavedDate.ToString("yyyy-MM-dd"), result);
        }

        [Fact]
        public void SavedCalculation_MetabolismType()
        {
            // Arrange & Act
            var calc = new SavedCalculation("Metabolism Test", "Metabolism");

            // Assert
            Assert.Equal("Metabolism", calc.Type);
        }

        [Fact]
        public void SavedCalculation_MacroType()
        {
            // Arrange & Act
            var calc = new SavedCalculation("Macro Test", "Macro");

            // Assert
            Assert.Equal("Macro", calc.Type);
        }

        [Fact]
        public void SavedCalculation_ParameterlessConstructor()
        {
            // Arrange & Act
            var calc = new SavedCalculation();

            // Assert
            Assert.NotNull(calc.Id);
            Assert.Null(calc.Name);
            Assert.Null(calc.Type);
        }
    }
}
