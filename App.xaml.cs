using Microsoft.Extensions.Configuration;
using TestApp.Converters;
using System.Windows;

namespace TestApp;

public partial class App : Application
{
    public IConfiguration Configuration { get; }

    public App()
    {

    }
}