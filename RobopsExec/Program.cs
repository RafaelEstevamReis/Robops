using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using RafaelEstevam.Simple.Spider.Helper;
using Robops.Lib.Camara.Leg.API;
using Robops.Lib.Camara.Leg.Gabinete;
using Robops.Spiders.Camara.Leg.Cota;
using Robops.Spiders.Senado.Leg.Pessoal;
using Simple.Sqlite;

namespace RobopsExec
{
    class Program
    {
        static void Main(string[] args)
        {
            SqliteDB.EnabledDatabaseBackup = false; // 5GB é muito, deixa quieto
            Console.WriteLine("Preparando banco de dados");
            //SqliteDB db = new SqliteDB("doacoes_2020.db");
            
            //Console.WriteLine($"BD: {db.DatabaseFileName}");
            Console.WriteLine("Lendo dados");


            //SqliteDB dbGabinete = new SqliteDB("gabinete_camara.db");
            //Robops.Spiders.Camara.Leg.Gabinete.ColetarPessoal.run(dbGabinete);
            //SqliteDB dbDoacoes = new SqliteDB("doacoes_2020.db");
            //Robops.Spiders.TSE.Contas.CarregaArquivoBaixado.run(dbDoacoes);
            compararGabineteDoacoes();

            Console.WriteLine("Fim");
        }

        private static void compararGabineteDoacoes()
        {
            Console.WriteLine("Abrindo bancos");
            SqliteDB dbGabinete = new SqliteDB("gabinete_camara.db");
            SqliteDB dbDoacoes = new SqliteDB("doacoes_2020.db");
            Console.WriteLine("Selecionando doadores");
            var modelDoacoes = dbDoacoes.ExecuteQuery<Robops.Lib.TSE.Contas.ReceitasModel>("SELECT * FROM ReceitasModel WHERE length(DocumentoDoador) = 11 ", null)
                                        .Where(o => o.Ano == 2020)
                                        .OrderBy(o => o.NomeDoadorRFB)
                                        .ToArray();
            Console.WriteLine("Selecionando pessoal");
            var gabinete = dbGabinete.GetAll<PessoalModel>()
                                     .ToArray();
            var hashNomesGabinete = gabinete.Select(o => o.NomeFuncionario.ToUpper())
                                            .ToHashSet();

            var deputados = dbGabinete.GetAll<Robops.Lib.Camara.Leg.Deputado>()
                                      .ToDictionary(d => d.Id);

            Console.WriteLine("Executando busca");
            foreach (var doador in modelDoacoes)
            {
                if (!hashNomesGabinete.Contains(doador.NomeDoadorRFB.ToUpper())) continue;

                var gab = gabinete.First(g => g.NomeFuncionario.ToUpper() == doador.NomeDoadorRFB.ToUpper());
                // ele pode doar para si mesmo
                if (gab.NomeFuncionario.ToUpper() == doador.NomeCandidato.ToUpper()) continue;

                string fileName = "doadores.csv";
                var deputado = deputados[gab.IdDeputado];

                if (deputado.NomeCivil.ToUpper() == doador.NomeCandidato.ToUpper()) fileName = "doadores_pingPong.csv";
                else fileName = "doadores_gramaVizinho.csv";

                string text = $"{doador.NomeDoadorRFB.ToUpper()};{gab.NomeDeputado.ToUpper()};{deputado.PartidoLideranca};{doador.NomeCandidato.ToUpper()};{doador.NumeroCandidato};{gab.InicioExercicio:d} - {gab.FimExercicio:d}\r\n";
                Console.WriteLine(text);
                File.AppendAllText(fileName, text);
            }
        }

        private static void processaDadosFolha(int mes, int ano)
        {
            SqliteDB db = new SqliteDB("senadoresFolha.db");
            db.CreateTables()
              //.Add<Robops.Lib.Senado.Leg.Senador>()
              //.Add<Robops.Lib.Senado.Leg.FuncionarioGabinete>()
              .Add<Robops.Lib.Senado.Leg.Folha>()
              .Commit();

            var cat = new CatalogarFuncionariosSenadores();
            cat.Catalogar(ano);

            var dados = cat.Senadores;

            var jaTem = db.GetAll<Robops.Lib.Senado.Leg.Folha>()
                          .Select(o => $"{o.CodigoFuncionario}-{o.Referencia.Year}-{o.Referencia.Month}")
                          .ToHashSet();

            var funcionarios = dados.SelectMany(s => s.Gabinete)
                                    .Distinct()
                                    .Where(o => !jaTem.Contains($"{o.CodigoFuncionario}-{ano}-{mes}")) // não já foi
                                                                                                       //.Take(500) // lotes ...
                                    .ToArray();
            var codFuncionarios = funcionarios
                                    .Select(f => f.CodigoFuncionario)
                                    .ToArray();

            var itensFolha = CatalogarDadosFolha.CarregarFolhaFuncionarios(codFuncionarios, mes, ano);
            db.BulkInsert(itensFolha);
            // adicoina os faltantes
            var semInformacao = funcionarios
                                    .Where(f => !itensFolha.Any(iF => iF.CodigoFuncionario == f.CodigoFuncionario))
                                    .Select(func => new Robops.Lib.Senado.Leg.Folha()
                                    {
                                        CodigoFuncionario = func.CodigoFuncionario,
                                        Referencia = new DateTime(ano, mes, 1)
                                    })
                                    .ToArray();
            db.BulkInsert(semInformacao);
        }

        private static void comparaListasNomes(string[] Lista1, string[] Lista2)
        {
            var l1 = Lista1.Where(l => !string.IsNullOrWhiteSpace(l))
                           .OrderBy(o => o);
            var l2 = Lista2.Where(l => !string.IsNullOrWhiteSpace(l))
                           .OrderBy(o => o);

            // caso tenha CamelCase, pode ser que sejam localizados nomes grudados

            // Passe 1, acha primeiro nome
            var primeirosNomes = Lista1.Select(l => l.Split(' ')[0].ToLower())
                                       .Intersect(
                                            Lista2.Select(l => l.Split(' ')[0].ToLower())
                                       )
                                       .ToArray();

            primeirosNomes = primeirosNomes;
        }
        private static void cruzaDeputadosCandidatos()
        {
            // Coletado com coletarDeputadosApi()
            var deputados = Directory
                .GetFiles(@"B:\Temp\api\deputados\", "*.json")
                .Select(f => JsonConvert.DeserializeObject<Deputado>(File.ReadAllText(f)))
                .ToDictionary(o => o.dados.cpf);

            // http://agencia.tse.jus.br/estatistica/sead/odsele/consulta_cand/consulta_cand_2020.zip -> Brasil.csv
            var arquivo = @"B:\Temp\consulta_cand_2020_BRASIL.csv";
            using var fs = File.OpenRead(arquivo);
            var lines = CSVHelper.FileSplit(new StreamReader(fs, Encoding.UTF7))
                                 .Skip(1);

            Console.WriteLine("Lendo candidatos");

            foreach (var line in lines)
            {
                // [20] CPF
                var cpf = line[20];

                if (deputados.ContainsKey(cpf))
                {
                    var deputado = deputados[cpf];
                    Console.WriteLine($"{deputado.dados.id:000000} {deputado.dados.cpf} {deputado.dados.nomeCivil} -> {line[12]}/{line[14]} [{line[28]}]");

                    string url = $"https://www.camara.leg.br/cota-parlamentar/sumarizado?nuDeputadoId={deputado.dados.id}&dataInicio=8/2020&dataFim=10/2020&despesa=&nomeHospede=&nomePassageiro=&nomeFornecedor=&cnpjFornecedor=&numDocumento=&sguf=&filtroNivel1=1&filtroNivel2=2&filtroNivel3=3";
                    var document = FetchHelper.FetchResourceDocument(new Uri(url), enableCaching: true);
                }
            }
        }

        private static void coletarDeputadosApi()
        {
            // https://dadosabertos.camara.leg.br/arquivos/deputados/csv/deputados.csv
            var arquivo = @"B:\Temp\deputados.csv";
            using var fs = File.OpenRead(arquivo);
            using var sr = new StreamReader(fs);
            var lines = CSVHelper.FileSplit(sr)
                                 .Skip(1);
            foreach (var l in lines)
            {
                var idLegislaturaFinal = l[3];

                if (idLegislaturaFinal != "56") continue;

                var uri = new Uri(l[0]);
                var txt = FetchHelper.FetchResourceText(uri, enableCaching: true);
                File.WriteAllText($@"B:\Temp\api\deputados\{uri.Segments[^1]}.json", txt);
            }
        }

        static void coletarTodos()
        {
            var spider = new SpiderCotaParlamentar(2020, 8);
            spider.Executar();

            var despesas = spider.ListaDespesas.ToArray();
            Console.WriteLine($"Coletado: {despesas.Sum(d => d.ValorDespesa):C2} em {despesas.Length} despesas");
            var deputados = spider.ListaDeputados.ToArray();
            Console.WriteLine($" em {deputados.Length} deputados");
        }
    }
}
