using Robops.Pickle.Interfaces;
using Robops.Pickle.Models;
using Serilog;
using Serilog.Events;
using Simple.BotUtils.Helpers;
using System.IO;
using System.Linq;
using System.Reflection;

var cfg = new ConfiguracaoColeta()
{
    Logger = new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File(Path.Combine("logs", "Logs.txt"), rollingInterval: RollingInterval.Day)
            .CreateLogger(),
};

cfg.Logger.Information("Pickle Start");

// Listar todos os coletores
var types = TypeHelper.GetClassesOfType<IColeta>(Assembly.GetExecutingAssembly());
var coletores = TypeHelper.CreateInstancesFor<IColeta>(types)
                          .ToArray();
cfg.Logger.Information($"{coletores.Length} coletores inicializados");

foreach (var coletor in coletores)
{
    cfg.Logger.Information($"[INIT] {coletor.GetType().Name} para {coletor.NomeCasa} ");
    coletor.Setup(cfg);
    coletor.ColetarDados();
}

cfg.Logger.Information("Finalizado");
