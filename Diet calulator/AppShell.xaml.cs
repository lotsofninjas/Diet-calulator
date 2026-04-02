using Diet_calulator.Services;
using Diet_calulator.Views;

namespace Diet_calulator
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            ApplyTheme();
        }

        private void ApplyTheme()
        {
            // Applicera på Shell
            this.BackgroundColor = Color.Parse(ThemeService.GetBackground());
            
            // Applicera på TitleLabel
            if (this.FindByName<Label>("TitleLabel") is Label titleLabel)
            {
                titleLabel.TextColor = Color.Parse(ThemeService.GetTextPrimary());
            }
        }

        private async void OnSettingsClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("settings");
        }
    }
}
