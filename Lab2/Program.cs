using Lab2;
using Lab2.model;
using Lab2.repository;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

class Program

{
    public static void Main(string[] args)

    {
        CreateHostBuilder(args).Build().Run();
    }


    public static IHostBuilder CreateHostBuilder(string[] args)

    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>

            {
                services.AddHostedService<Princess>();
                services.AddHostedService<SimulatorService>();
                services.AddSingleton<Simulator>();
                services.AddScoped<IContenderGenerator, ContenderGenerator>();
                services.AddScoped<Friend>();
                services.AddScoped<Hall>();
            });
    }
}