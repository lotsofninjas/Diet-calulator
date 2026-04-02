using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

namespace Diet_calulator
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            
            try
            {
                // Initialize settings and theme service after app resources are loaded
                Services.AppSettings.Initialize();
                Services.ThemeService.Initialize();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error initializing services: {ex.Message}");
                // Continue with defaults if initialization fails
            }
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var shell = new AppShell();
            return new Window(shell);
        }
    }
}