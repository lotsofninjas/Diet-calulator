using Diet_calulator.Services;
using Diet_calulator.Views;

namespace Diet_calulator
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
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
    }
}
