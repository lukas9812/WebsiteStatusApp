using WebsiteStatus;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Microsoft.Extensions.Hosting;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.File(@"C:\temp\WorkerService\LogFile.txt")
    .CreateLogger();

try
{
    Log.Information("Starting up the service.");
    IHost host = Host.CreateDefaultBuilder(args)
        .UseWindowsService()
        .ConfigureServices(services =>
        {
            services.AddHostedService<Worker>();
        })
        .UseSerilog()
        .Build();

    await host.RunAsync();
    return;
}
catch (Exception e)
{
    Log.Fatal(e, "There was a problem with start a service");
    return;
}
finally
{
    Log.CloseAndFlush();
}

//For running this service write this command to the PowerShell launched as administrator.
//sc.exe create WebsiteStatus binpath= c\temp\WorkerService\WebsiteStatus.exe start= auto
//Open Services in Windows and hit run + check @C:\temp\WorkerService\LogFile.txt.
//For delete use command : sc.exe delete WebsiteStatus