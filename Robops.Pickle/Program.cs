using Robops.Pickle.Helpers;
using Robops.Pickle.Interfaces;
using Robops.Pickle.Models;
using Simple.BotUtils.Helpers;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

var cfg = new ConfiguracaoColeta()
{
    Logger = LoggerHelper.Criar(Path.Combine("logs", "Logs.txt")),
};

cfg.Logger.Information("Pickle Start");

// Listar todos os coletores
var types = TypeHelper.GetClassesOfType<IColeta>(Assembly.GetExecutingAssembly());
var coletores = TypeHelper.CreateInstancesFor<IColeta>(types)
                          .ToArray();
cfg.Logger.Information($"{coletores.Length} coletores inicializados");

foreach (var coletor in coletores)
{
    var nomeColetor = coletor.GetType().Name;

    try
    {
        cfg.Logger.Information($"[INIT] {nomeColetor} para {coletor.NomeCasa} ");
        coletor.Setup(cfg);

        cfg.Logger.Information($"[EXEC] {coletor.NomeCasa} ");
        coletor.ColetarDados();
    }
    catch (Exception ex)
    {
        cfg.Logger.Error(ex, $"Falha no coletor {nomeColetor}");
        continue;
    }
}

cfg.Logger.Information("Finalizado");
