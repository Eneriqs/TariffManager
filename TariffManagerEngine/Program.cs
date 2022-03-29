// See https://aka.ms/new-console-template for more information
using Serilog;
using TariffManagerLib.Services;


var log = new LoggerConfiguration()
              .ReadFrom.AppSettings()
              .CreateLogger();
Log.Logger = log;

ProcessManager processManager = new ProcessManager();
processManager.Process();




//Console.Read();