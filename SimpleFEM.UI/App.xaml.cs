using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SimpleFEM.Core.Commands;
using SimpleFEM.Core.Interfaces;
using SimpleFEM.Core.Models;
using SimpleFEM.Core.Services.GeometryService;
using SimpleFEM.Core.Tools;
using SimpleFEM.Data;
using SimpleFEM.Data.Factories;
using SimpleFEM.Data.Repositories.EfCore;
using SimpleFEM.Data.Services;
using SimpleFEM.UI.ViewModels;
using SimpleFEM.UI.Views;
using System.Windows;

namespace SimpleFEM
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IHost _host;

        public App()
        {
            _host = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: false);
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddSingleton<IDbContextFactory<DataContext>, ModelFileDbContextFactory>();

                    services.AddSingleton<IModelFileService, SqliteModelFileService>();

                    services.AddSingleton<IRepository<Node>, NodeEfRepository>();
                    services.AddSingleton<IRepository<Line>, LineEfRepository>();

                    services.AddSingleton<IGeometryService, GeometryService>();
                    services.AddSingleton<CommandManager>();

                    services.AddTransient<IDrawingTool, NodeTool>();
                    services.AddTransient<IDrawingTool, LineTool>();

                    services.AddSingleton<MainWindow>();
                    services.AddSingleton<MainViewModel>();

                    services.AddSingleton<MainWindow>();
                    services.AddSingleton<MainViewModel>();

                    services.AddTransient<LaunchWindow>();
                    services.AddTransient<LaunchViewModel>();
                })
                .Build();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            await _host.StartAsync();

            // Initial window. User Opens or Creates new file and opens it. Uses IModelFileService.
            var launchScreen = _host.Services.GetRequiredService<LaunchWindow>();

            // Sends event if a file is opened --> Open main view 
            var modelFileService = _host.Services.GetRequiredService<IModelFileService>();
            modelFileService.ModelFileChanged += (s, args) =>
            {
                var mainWindow = _host.Services.GetRequiredService<MainWindow>();
                mainWindow.Show();
                launchScreen.Close();
            };

            launchScreen.Show();
            
            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            // Ensure any open model file is closed before exit
            var fileService = _host.Services.GetRequiredService<IModelFileService>();
            fileService.CloseCurrentModelFile();

            using (_host)
            {
                await _host.StopAsync();
            }
            base.OnExit(e);
        }
    }
}