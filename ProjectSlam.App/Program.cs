using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjectSlam.App.Features.Core;
using ProjectSlam.App.Features.DataManager;
using ProjectSlam.Data;
using ProjectSlam.Data.Interfaces;
using ProjectSlam.Data.Repositories;
using ProjectSlam.Data.Services;
using System.IO;
using System.Windows;

namespace ProjectSlam.App;

public class Program
{
    [STAThread]
    private static void Main(string[] args)
    {
        try
        {

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            using (var serviceProvider = serviceCollection.BuildServiceProvider())
            {
                // Start the WPF application
                var app = new App();

                try
                {
                    // Initialize database synchronously
                    var dbConfig = serviceProvider.GetRequiredService<DbConfig>();
                    Task.Run(async () => await dbConfig.InitializeDatabaseAsync())
                        .GetAwaiter()
                        .GetResult();

                    var photoStoragePath = serviceProvider.GetRequiredService<IConfiguration>()
                        .GetSection("PhotoStoragePath").Value;

                    if (!string.IsNullOrEmpty(photoStoragePath))
                    {
                        Directory.CreateDirectory(photoStoragePath);
                    }

                    var view = serviceProvider.GetRequiredService<MainWindow>();
                    view.Show();
                    app.Run();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to initialize database: {ex.Message}", "Startup Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Application failed to start: {ex.Message}", "Fatal Error",
            MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        // Configure database
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("AppSettings.json", optional: true, reloadOnChange: true)
            .Build();

        services.AddSingleton<IConfiguration>(configuration);
        services.AddSingleton<DbConfig>();

        // Register Views
        services.AddSingleton<MainWindow>();
        services.AddTransient<DataManager>();

        // Register ViewModels
        services.AddTransient<MainWindowViewModel>();
        services.AddTransient<DataManagerViewModel>();

        // Register Services
        services.AddSingleton<IFileDialog, FileDialogService>();
        services.AddSingleton<IValidationService, ValidationService>();
        services.AddSingleton<IPhotoService>(sp =>
        {
            var photoPath = sp.GetRequiredService<IConfiguration>()
                .GetSection("PhotoStoragePath").Value ??
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SOMS", "Photos");
            return new PhotoService(photoPath);
        });

        // Register repositories
        services.AddScoped<IGlobalMovesetRepository, GlobalMovesetRepository>();
    }
}
