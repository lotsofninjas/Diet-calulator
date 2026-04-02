using Microsoft.Extensions.Logging;
using Diet_calulator.Converters;
using Diet_calulator.Views;

namespace Diet_calulator
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Register routes
            Routing.RegisterRoute("settings", typeof(SettingsPage));
            Routing.RegisterRoute("CardioPage", typeof(CardioPage));

            // Register converters
            Microsoft.Maui.Controls.ResourceDictionary resources = new();
            resources.Add("EqualityConverter", new EqualityConverter());
            Microsoft.Maui.Controls.Application.Current?.Resources.MergedDictionaries.Add(resources);

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
