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
using SimpleFEM.Data.Repositories;
using SimpleFEM.UI.ViewModels;
using System.Windows;

namespace SimpleFEM
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IHost _host;
        // fixed file path for testing
        private const string DEFAULT_DB_PATH = "C:\\Users\\MateKadar\\Downloads\\SimpleFEMtest.db";

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
                    services.AddDbContext<DataContext>(options =>
                    options.UseSqlite($"Data Source={DEFAULT_DB_PATH}"),
                    ServiceLifetime.Singleton);

                    services.AddSingleton<IRepository<Node>, EfRepository<Node>>();
                    services.AddSingleton<IRepository<Line>, EfRepository<Line>>();

                    services.AddSingleton<IGeometryService, GeometryService>();
                    services.AddSingleton<CommandManager>();

                    services.AddTransient<IDrawingTool, NodeTool>();
                    services.AddTransient<IDrawingTool, LineTool>();

                    services.AddSingleton<MainWindow>();
                    services.AddSingleton<MainViewModel>();
                })
                .Build();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            await _host.StartAsync();

            var context = _host.Services.GetRequiredService<DataContext>();
            await context.Database.MigrateAsync();

            var mainWindow = _host.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();

            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            using (_host)
            {
                await _host.StopAsync();
            }
            base.OnExit(e);
        }
    }
}
