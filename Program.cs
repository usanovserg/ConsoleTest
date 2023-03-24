



// See https://aka.ms/new-console-template for more information
using ControllerExChanges.Controller;
using ControllerExChanges.Entity;
using ControllerExChanges.Enums;
using ControllerExChanges.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Compact;


public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");

        IServiceCollection services = new ServiceCollection()
        .AddSingleton<ILogger, Serilog.Core.Logger>();

        IServiceProvider serviceProvider = services.BuildServiceProvider();

        ILogger log = BuildLogger();

        Controller controller = Controller.GetController(log);

        IConnector? connector = controller.CreateConnector(ExchangeType.LiveFutures);

        if (connector == null) return;

        ConnectParametrs parametrs = connector.ConnectParametrs;

        parametrs.ExchangeType = ExchangeType.LiveFutures;
        parametrs.Login = "serg-225@mail.ru";
        parametrs.Password = ",/?*xeRa";
        parametrs.Path = "111";

        ConnectStatus state = connector.Connect().Result;

        Console.WriteLine("ConnectStatus = " + state);

        Console.ReadLine();
    }


    private static ILogger BuildLogger()
    {
        if (!Directory.Exists(@"Log"))
        {
            Directory.CreateDirectory(@"Log");
        }

        ILogger log = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .WriteTo.File(new CompactJsonFormatter(),
                                    @"Log\" + DateTime.Now.ToShortDateString() + "_file.log", LevelAlias.Minimum, 104857600L)
                    .CreateLogger();

        return log;
    }
}

















