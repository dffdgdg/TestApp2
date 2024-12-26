using Microsoft.Extensions.Configuration;
using TestApp.Converters;
using System.Windows;
using System.IO;

namespace TestApp;

public partial class App : Application
{
    public IConfiguration Configuration { get; private set; }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        Configuration = builder.Build();
        var connectionString = Configuration.GetConnectionString("DefaultConnection");
    }
}
