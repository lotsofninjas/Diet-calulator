using System.ComponentModel;
using System.Runtime.CompilerServices;
using Diet_calulator.Constants;

namespace Diet_calulator.ViewModels;

public class SettingsViewModel : INotifyPropertyChanged
{
    private string _weightUnit = AppConstants.Unit_Kg;
    private string _distanceUnit = AppConstants.Unit_Km;

    public event PropertyChangedEventHandler? PropertyChanged;

    #region Properties
    public string WeightUnit
    {
        get => _weightUnit;
        set { SetProperty(ref _weightUnit, value); }
    }

    public string DistanceUnit
    {
        get => _distanceUnit;
        set { SetProperty(ref _distanceUnit, value); }
    }
    #endregion

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
