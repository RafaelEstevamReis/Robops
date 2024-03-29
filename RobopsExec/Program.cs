﻿using Newtonsoft.Json;
using RafaelEstevam.Simple.Spider.Helper;
using Robops.Lib;
using Robops.Lib.Camara.Leg.API;
using Robops.Lib.Camara.Leg.Gabinete;
using Robops.Spiders.AL.MG;
using Robops.Spiders.Camara.Leg.Cota;
using Robops.Spiders.Senado.Leg.Pessoal;
using Simple.API;
using Simple.Brazilian.Documents;
using Simple.Sqlite;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RobopsExec
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Preparando banco de dados");
            Console.WriteLine("Lendo dados");

            for (int i = 2013; i <= 2022; i++)
            {
                Robops.Spiders.Transparencia.CPGF.DadosCpgf.BaixarAno(i, $"cpgf{i}");
            }


            Console.WriteLine("Fim");
        }

        private static void processaPlanilhaGastosAno()
        {
            var path = "Planilha12003a2022.csv";
            // DATA PGTO;CPF SERVIDOR;CPF/CNPJ FORNECEDOR;NOME FORNECEDOR;VALOR;TIPO;SUBELEMENTO DE DESPESA;CDIC
            FastCsv csv = new FastCsv();
            var dados = csv.ReadDelimiter(File.OpenRead(path))
                           .Skip(1)
                           .Select(l => new
                           {
                               dt = l[0].ToDateTime(),
                               cpf = l[1],
                               forn = l[2],
                               nome = l[3],
                               valor = l[4].ToDecimal(0),
                               tipo = l[5],
                               sub = l[6],
                               cdic = l[7],
                           })
                           .Where(o => o.dt.Year > 2000)
                           .ToArray();

            var agrAno = dados.GroupBy(d => d.dt.Year);

            var dadosAnoFornecedor = agrAno.Select(g => new
            {
                agrAno = g.Key,
                fornecedores = g.GroupBy(o => o.forn)
                                .Select(o => new
                                {
                                    CNPJ = o.Key.Length > 11 ? CNPJ.Mask(o.Key) : CPF.Mask(o.Key),
                                    Nome = o.First().nome,
                                    Qtd = o.Count(),
                                    Total = o.Sum(k => k.valor),
                                })
                                .OrderByDescending(o => o.Total)
                                .ToArray()
            });

            StringBuilder sbCsv = new StringBuilder();
            StringBuilder sbCsvAno = new StringBuilder();
            foreach (var ano in dadosAnoFornecedor)
            {
                sbCsv.AppendLine($"{ano.agrAno};;;{ano.fornecedores.Sum(o => o.Total):C2};{ano.fornecedores.Length}");
                sbCsvAno.AppendLine($"ANO;CNPJ;Fornecedor;Total;Ocorências");
                foreach (var f in ano.fornecedores)
                {
                    sbCsvAno.AppendLine($"{ano.agrAno};\"{f.CNPJ}\";\"{f.Nome}\";\"{f.Total:C2}\";{f.Qtd}");
                    sbCsv.AppendLine($";\"{f.CNPJ}\";\"{f.Nome}\";\"{f.Total:C2}\";{f.Qtd}");
                }
                File.WriteAllText($"agrupadoFornecedor_{ano.agrAno}.csv", sbCsvAno.ToString());
                sbCsvAno.Clear();
            }
            File.WriteAllText("agrupadoAnoFornecedor.csv", sbCsv.ToString());
            sbCsv.Clear();

            var dadosAnoSub = agrAno.Select(g => new
            {
                agrAno = g.Key,
                sub = g.GroupBy(o => o.sub)
                       .Select(o => new
                       {
                           Sub = o.Key,
                           Qtd = o.Count(),
                           Total = o.Sum(k => k.valor),
                       })
                       .OrderByDescending(o => o.Total)
                       .ToArray()
            });
            foreach (var ano in dadosAnoSub)
            {
                sbCsv.AppendLine($"{ano.agrAno};;{ano.sub.Sum(o => o.Total):C2};{ano.sub.Length}");
                foreach (var f in ano.sub)
                {
                    sbCsv.AppendLine($";\"{f.Sub}\";\"{f.Total:C2}\";{f.Qtd}");
                }
            }
            File.WriteAllText("agrupadoAnoSub.csv", sbCsv.ToString());
        }

        private static async Task baixaDocuemntosAsync()
        {
            // baixar notas de fornecedor
            var ci = new ClientInfo("https://api.ops.net.br/deputado/");
            int fornecedor = 28252;
            var recibos = await ci.PostAsync<Robops.Lib.SiteOPS.DeputadoFederal.Agrupamento>("lancamentos",
                    Robops.Lib.SiteOPS.DeputadoFederal.FiltroAgrupamento.AgruparFornecedor(9, fornecedor, 0, 100));

            var client = new WebClient();
            if (!Directory.Exists("recibos")) Directory.CreateDirectory("recibos");
            using var sw = new StreamWriter($"recibos\\recibos_{fornecedor}.txt");
            foreach (var recibo in recibos.Data.data)
            {
                Console.WriteLine("documento/" + recibo.id_cf_despesa);
                // faz request do documento
                var doc = await ci.GetAsync<Robops.Lib.SiteOPS.DeputadoFederal.Documento>("documento/" + recibo.id_cf_despesa);

                sw.WriteLine($"Competência: {doc.Data.competencia} Emissão: {doc.Data.data_emissao} ValorLiq: {doc.Data.valor_liquido} Desc: {doc.Data.descricao_despesa}");
                sw.WriteLine($"[{recibo.numero_documento}] {doc.Data.url_documento}");
                client.DownloadFile(doc.Data.url_documento, $"recibos\\{doc.Data.ano_mes}_{recibo.numero_documento}_{doc.Data.id_documento}.pdf");
            }

        }

        private static void coletaBudgetALRJ()
        {
            Robops.Spiders.Senado.Leg.Combustivel.CatalogaGastosVeiculo.run();
            processaCsvParaTxt();

            for (int i = 2019; i < 2022; i++)
            {
                ColetarVerba.run(i);
            }

            Robops.Spiders.AL.RJ.ColetaBudgets.GeraArquivoBudget("ALRJ.csv");
        }
        private static void processaCsvParaTxt()
        {
            string[] meses = {
                "Janeiro", "Fevereiro", "Março", "Abril",
                "Maio", "Junho", "Julho", "Agosto",
                "Setembro", "Outubro", "Novembro", "Dezembro"
            };
            FastCsv csv = new FastCsv();
            csv.Encoding = Encoding.UTF7;

            var dados = csv.ReadDelimiter(File.OpenRead("teste.csv"))
                           .Skip(1)
                           .Select(l => new
                           {
                               Ano = l[0].ToInt(),
                               Mes = l[1].ToInt(),
                               Parlamentar = l[3],
                               Valor = l[5].Trim().Replace("R$", "R$ "),
                               Cnpj = l[6],
                               Empresa = l[7],
                           })
                           .GroupBy(o => o.Parlamentar)
                           .Select(o => new { Key = o.Key, Dados = o.ToArray() });

            using var file = File.Open("dados.txt", FileMode.Create, FileAccess.Write);
            using var sw = new StreamWriter(file);
            foreach (var v in dados)
            {
                sw.WriteLine($"{v.Key}");
                Console.WriteLine(v.Key);

                var anos = v.Dados.GroupBy(o => o.Ano)
                                  .Select(g => new
                                  {
                                      Ano = g.Key,
                                      notas = g.OrderBy(o => o.Mes)
                                  })
                                  .OrderBy(o => o.Ano);

                foreach (var ano in anos)
                {
                    //sw.WriteLine($"Em {ano.Ano}:");
                    foreach (var nota in ano.notas)
                    {
                        sw.WriteLine($"{meses[nota.Mes - 1]}/{nota.Ano}: Nota no valor de {nota.Valor} pago à empresa {nota.Empresa}, CNPJ {nota.Cnpj}.");
                    }
                    sw.WriteLine();
                }

                sw.WriteLine();
            }

        }
        private static void compararGabineteDoacoes()
        {
            Console.WriteLine("Abrindo bancos");
            var dbGabinete = ConnectionFactory.FromFile("gabinete_camara.db");
            var dbDoacoes = ConnectionFactory.FromFile("doacoes_2020.db");

            using var cnGabinete = dbGabinete.GetConnection();
            using var cnDoacoes = dbDoacoes.GetConnection();

            Console.WriteLine("Selecionando doadores");
            var modelDoacoes = cnGabinete.Query<Robops.Lib.TSE.Contas.ReceitasModel>("SELECT * FROM ReceitasModel WHERE length(DocumentoDoador) = 11 ", null)
                                        .Where(o => o.Ano == 2020)
                                        .OrderBy(o => o.NomeDoadorRFB)
                                        .ToArray();
            Console.WriteLine("Selecionando pessoal");
            var gabinete = cnGabinete.GetAll<PessoalModel>()
                                     .ToArray();
            var hashNomesGabinete = gabinete.Select(o => o.NomeFuncionario.ToUpper())
                                            .ToHashSet();

            var deputados = cnGabinete.GetAll<Robops.Lib.Camara.Leg.Deputado>()
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
            //SqliteDB db = new SqliteDB("senadoresFolha.db");
            var db = ConnectionFactory.FromFile("senadoresFolha.db");
            using var cnn = db.GetConnection();
            cnn.CreateTables()
              //.Add<Robops.Lib.Senado.Leg.Senador>()
              //.Add<Robops.Lib.Senado.Leg.FuncionarioGabinete>()
              .Add<Robops.Lib.Senado.Leg.Folha>()
              .Commit();

            var cat = new CatalogarFuncionariosSenadores();
            cat.Catalogar(ano);

            var dados = cat.Senadores;

            var jaTem = cnn.GetAll<Robops.Lib.Senado.Leg.Folha>()
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
            cnn.BulkInsert(itensFolha);
            // adicoina os faltantes
            var semInformacao = funcionarios
                                    .Where(f => !itensFolha.Any(iF => iF.CodigoFuncionario == f.CodigoFuncionario))
                                    .Select(func => new Robops.Lib.Senado.Leg.Folha()
                                    {
                                        CodigoFuncionario = func.CodigoFuncionario,
                                        Referencia = new DateTime(ano, mes, 1)
                                    })
                                    .ToArray();
            cnn.BulkInsert(semInformacao);
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
