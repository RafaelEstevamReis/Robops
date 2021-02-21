using System;
using System.Collections.Generic;
using System.Linq;
using RafaelEstevam.Simple.Spider;
using Robops.Lib;
using Robops.Lib.Camara.Leg;
using Robops.Lib.Camara.Leg.Gabinete;
using Simple.Sqlite;

namespace Robops.Spiders.Camara.Leg.Gabinete
{
    public class ColetarPessoal
    {
        static SqliteDB db;
        public static void run(SqliteDB database)
        {
            db = database;
            db.CreateTables()
              .Add<PessoalModel>()
              .Add<Deputado>()
              .Commit();

            var init = InitializationParams.Default002()
                .SetConfig(c => c.Disable_AutoAnchorsLinks()
                                 .Set_DownloadDelay(0));
                    
            var spider = new SimpleSpider("camara_gabinete", new Uri("https://www.camara.leg.br/"), init);

            spider.FetchCompleted += Spider_FetchCompleted;
            spider.AddPage(new Uri("https://www.camara.leg.br/deputados/quem-sao"), spider.BaseUri);
            spider.Execute();

        }

        private static void Spider_FetchCompleted(object Sender, FetchCompleteEventArgs args)
        {
            var spider = (SimpleSpider)Sender;
            var hObj = args.GetHObject();
            if (args.Link.EndsWith("/quem-sao"))
            {
                var lista = hObj["select > #parametro-nome > option"].GetAttributeValues("value");
                spider.AddPages(lista.Where(i => i.Length > 0)
                                     .Select(c => new Uri($"https://www.camara.leg.br/deputados/{c}")),
                                args.Link);
                for (int ano = 2021; ano >= 2020; ano--)
                {
                    spider.AddPages(lista.Where(i => i.Length > 0)
                                         .Select(c => new Uri($"https://www.camara.leg.br/deputados/{c}/pessoal-gabinete?ano={ano}")),
                                    args.Link);
                }
            }
            else if (args.Link.Contains("/pessoal-gabinete?"))
            {
                string nomeDeputado = hObj["h1"].Trim();
                int ano = args.Link.Uri.ToString().Split('=')[1].ToInt();
                int deputado = args.Link.Uri.ToString().Split('/')[4].ToInt();
                var rows = hObj["tr"];
                var pessoas = new List<PessoalModel>();
                foreach (var row in rows)
                {
                    var cols = row["td"].TrimAll();
                    if (cols.Length == 0) continue;

                    DateTime inicio = new DateTime(1900, 01, 01);
                    DateTime fim = new DateTime(1900, 01, 01);

                    var datas = cols[3].Split(' ');
                    if (cols[3].Contains("Desde"))
                    {
                        inicio = datas[1].ToDateTime();
                        fim = DateTime.Now;
                    }
                    else if (cols[3].Contains("De "))
                    {
                        inicio = datas[1].ToDateTime();
                        fim = datas[3].ToDateTime();
                    }

                    var pessoa = new PessoalModel()
                    {
                        Ano = ano,
                        IdDeputado = deputado,
                        NomeDeputado = nomeDeputado,
                        NomeFuncionario = cols[0],
                        GrupoFuncional = cols[1],
                        Cargo = cols[2],
                        InicioExercicio = inicio,
                        FimExercicio = fim,
                        UrlRemuneracao = "",
                    };
                    pessoas.Add(pessoa);
                }
                db.BulkInsert(pessoas, addReplace: true);
            }
            else
            {
                //Página do deputado
                int id = args.Link.Uri.ToString().Split('/')[4].ToInt();
                Deputado dep = new Deputado()
                {
                    Id = id,
                    Nome = hObj["h2 > #nomedeputado"].Trim(),
                    PartidoLideranca = hObj["span > .foto-deputado__partido-estado"].Trim(),
                };
                foreach (var li in hObj["ul > .informacoes-deputado > li"])
                {
                    var hdr = li["span"].Trim();

                    switch (hdr)
                    {
                        case "Nome Civil:":
                            dep.NomeCivil = li.Children("#text").Trim();
                            break;
                        case "Telefone:":
                            dep.Telefone = li.Children("#text").Trim();
                            break;
                        case "E-mail:":
                            dep.EMail = li["a"].Trim();
                            break;
                        case "Endereço:":
                            dep.Endereco = li.Children("#text").Trim();
                            break;
                        case "Data de Nascimento:":
                            dep.DataNascimento = li.Children("#text").Trim().ToDateTime();
                            break;
                        case "Naturalidade:":
                            dep.Naturalidade = li.Children("#text").Trim();
                            dep.Naturalidade_UF = dep.Naturalidade.Split('-')[1].Trim();
                            break;

                        default:
                            break;
                    }
                }

                db.InsertOrReplace(dep);
            }
        }
    }
}
