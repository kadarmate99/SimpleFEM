using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SimpleFEM.Core.Interfaces;
using SimpleFEM.Core.Models;
using SimpleFEM.Core.Repositories;
using SimpleFEM.Core.Services.GeometryService;
using SimpleFEM.Core.Tools;
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
                    services.AddSingleton<IRepository<Node>, InMemoryRepository<Node>>();
                    services.AddSingleton<IRepository<Line>, InMemoryRepository<Line>>();

                    services.AddSingleton<IGeometryService, GeometryService>();
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
