using System;
using System.IO;
using System.Linq;
using RafaelEstevam.Simple.Spider.Helper;
using Robops.Lib;
using Robops.Lib.Helpers;
using Robops.Lib.Transparencia;
using Robops.Lib.TSE.Contas;
using Simple.Sqlite;

namespace Robops.Spiders.TSE.Contas
{
    public class CarregaArquivoBaixado
    {
        public static void run(SqliteDB db)
        {
            db.CreateTables()
              .Add<ReceitasModel>()
              .Commit();

            foreach (var file in Directory.GetFiles(@"N:\Organizando\Dados Crawlers\TSE\", "*.zip"))
            {
                processaArquivo(db, file);
            }
            //string path = @"N:\Organizando\Dados Crawlers\TSE\prestacao_de_contas_eleitorais_candidatos_2020.zip";
            //processaArquivo2018_2020(db, path);
            //string path = @"N:\Organizando\Dados Crawlers\TSE\prestacao_de_contas_eleitorais_candidatos_2018.zip";
            //baixarArquivo_modelo2018_2020(path, db);
            //string path = @"N:\Organizando\Dados Crawlers\TSE\prestacao_contas_final_2016.zip";
            //processaArquivo2016(db, path);
            //string path = @"N:\Organizando\Dados Crawlers\TSE\prestacao_final_2014.zip";
            //processaArquivo2014(db, path);
            //string path = @"N:\Organizando\Dados Crawlers\TSE\prestacao_contas_final_2012.zip";
            //processaArquivo2012(db, path);
            //string path = @"N:\Organizando\Dados Crawlers\TSE\prestacao_contas_2010.zip";
            //processaArquivo2010(db, path);
            //string path = @"N:\Organizando\Dados Crawlers\TSE\prestacao_contas_2008.zip";
            //processaArquivo2008(db, path);
            //string path = @"N:\Organizando\Dados Crawlers\TSE\prestacao_contas_2006.zip";
            //processaArquivo2006(db, path);

            //string path = @"N:\Organizando\Dados Crawlers\TSE\prestacao_contas_2004.zip";
            //processaArquivo2004(db, path);

        }

        private static void processaArquivo(SqliteDB db, string file)
        {
            if (file.EndsWith("2020.zip")) processaArquivo2018_2020(db, file);
            else if (file.EndsWith("2018.zip")) processaArquivo2018_2020(db, file);
            else if (file.EndsWith("2016.zip")) processaArquivo2016(db, file);
            else if (file.EndsWith("2014.zip")) processaArquivo2014(db, file);
            else if (file.EndsWith("2012.zip")) processaArquivo2012(db, file);
            else if (file.EndsWith("2010.zip")) processaArquivo2010(db, file);
            else if (file.EndsWith("2008.zip")) processaArquivo2008(db, file);
            else if (file.EndsWith("2006.zip")) processaArquivo2006(db, file);
        }

        private static void processaArquivo2018_2020(SqliteDB db, string zipName)
        {
            processaCsvZipado(db, zipName,
                              n => n.StartsWith("receitas_candidatos_2") && n.EndsWith("_BRASIL.csv"),
                              row => new ReceitasModel()
                              {
                                  Ano = row[2].ToInt(),
                                  CodigoEleicao = row[5].ToInt(),
                                  UF = row[12],
                                  NumeroCandidato = row[19].ToInt(),
                                  DocumentoDoador = row[36],
                                  NomeDoadorRFB = row[38],
                                  Valor = row[56].ToDecimal(0),
                                  FormaPagamento = ReceitasModel.FormasPagamento.Outros,
                              });
        }
        private static void processaArquivo2016(SqliteDB db, string zipName)
        {
            processaCsvZipado(db, zipName,
                              n => n.StartsWith("receitas_candidatos_") && n.EndsWith("_brasil.txt"), // !! TXT
                              row => new ReceitasModel()
                              {
                                  Ano = 2016,
                                  CodigoEleicao = row[0].ToInt(),
                                  UF = row[5],
                                  NumeroCandidato = row[9].ToInt(),
                                  DocumentoDoador = row[16],
                                  NomeDoadorRFB = row[18],
                                  Valor = row[25].ToDecimal(0),
                                  FormaPagamento = ReceitasModel.FormasPagamento.Outros,
                              });
        }
        private static void processaArquivo2014(SqliteDB db, string zipName)
        {
            processaCsvZipado(db, zipName,
                              n => n.StartsWith("receitas_candidatos_") && n.EndsWith("_brasil.txt"), // !! TXT
                              row => new ReceitasModel()
                              {
                                  Ano = 2014,
                                  CodigoEleicao = row[0].ToInt(),
                                  UF = row[5],
                                  NumeroCandidato = row[7].ToInt(),
                                  DocumentoDoador = row[13],
                                  NomeDoadorRFB = row[15],
                                  Valor = row[22].ToDecimal(0),
                                  FormaPagamento = ReceitasModel.FormasPagamento.Outros,
                              });
        }
        private static void processaArquivo2012(SqliteDB db, string zipName)
        {
            processaCsvZipado(db, zipName,
                              n => n.StartsWith("receitas_candidatos_") && n.EndsWith("_brasil.txt"), // !! TXT
                              row => new ReceitasModel()
                              {
                                  Ano = 2012,
                                  CodigoEleicao = row[0].ToInt(),
                                  UF = row[4],
                                  NumeroCandidato = row[8].ToInt(),
                                  DocumentoDoador = row[14],
                                  NomeDoadorRFB = row[16],
                                  Valor = row[23].ToDecimal(0),
                                  FormaPagamento = ReceitasModel.FormasPagamento.Outros,
                              });
        }
        private static void processaArquivo2010(SqliteDB db, string zipName)
        {
            processaCsvZipado(db, zipName,
                              n => n == "ReceitasCandidatos.txt",
                              row => new ReceitasModel()
                              {
                                  Ano = 2010,
                                  CodigoEleicao = 0,
                                  UF = row[2],
                                  NumeroCandidato = row[4].ToInt(),
                                  DocumentoDoador = row[11],
                                  NomeDoadorRFB = row[12],
                                  Valor = row[14].ToDecimal(0),
                                  FormaPagamento = ReceitasModel.FormasPagamento.Outros,
                              });
        }
        private static void processaArquivo2008(SqliteDB db, string zipName)
        {
            processaCsvZipado(db, zipName,
                              n => n.StartsWith("receitas_candidatos_") && n.EndsWith("_brasil.csv"),
                              row => new ReceitasModel()
                              {
                                  Ano = 2008,
                                  CodigoEleicao = 0,
                                  UF = row[6],
                                  NumeroCandidato = row[5].ToInt(),
                                  DocumentoDoador = row[21],
                                  NomeDoadorRFB = row[20],
                                  Valor = row[14].ToDecimal(0),
                                  FormaPagamento = ReceitasModel.FormasPagamento.Outros,
                              });
        }
        private static void processaArquivo2006(SqliteDB db, string zipName)
        {
            // O arquivo original baixado está com o ZIP corrompido
            // mas pode ser aberto e resalvo
            processaCsvZipado(db, zipName,
                              n => n == "ReceitaCandidato.csv",
                              row => new ReceitasModel()
                              {
                                  Ano = 2006,
                                  CodigoEleicao = 0,
                                  UF = row[5],
                                  NumeroCandidato = row[4].ToInt(),
                                  DocumentoDoador = row[16],
                                  NomeDoadorRFB = row[15],
                                  Valor = row[9].ToDecimal(0),
                                  FormaPagamento = ReceitasModel.FormasPagamento.Outros,
                              });
        }
        //private static void processaArquivo2004(SqliteDB db, string zipName)
        //{
        //    processaCsvZipado(db, zipName,
        //                      n => n == "ReceitaCandidato.CSV",
        //                      row => new ReceitasModel()
        //                      {
        //                          Ano = 2004,
        //                          CodigoEleicao = 0,
        //                          UF = row[5],
        //                          NumeroCandidato = row[4].ToInt(),
        //                          DocumentoDoador = row[16],
        //                          NomeDoadorRFB = row[15],
        //                          Valor = row[9].ToDecimal(0),
        //                          FormaPagamento = ReceitasModel.FormasPagamento.Outros,
        //                      });
        //}

        private static void processaCsvZipado(SqliteDB db, string zipName, Func<string, bool> fileFilter, Func<string[], ReceitasModel> extractionFilter)
        {
            var zip = new LeitorZipTransparencia(zipName);
            // Receitas, Brasil, agnóstico ao ano
            zip.ShouldProcessFile = fileFilter;
            zip.InicioLeituraArquivo += (s, e) => Console.WriteLine(e);
            zip.IgnoreFirstLine = true;
            var zipLines = zip.ReadLines();
            var rows = CSVHelper.DelimiterSplit(zipLines, ';');

            var buffer = new DataBuffer<ReceitasModel>(30000, data =>
            {
                db.BulkInsert(data, addReplace: true);
            });
            int qtd = 0;
            foreach (var row in rows)
            {                
                var cad = extractionFilter(row);
                buffer.Add(cad);

                qtd++;
                if (qtd % buffer.Quantity == 0) Console.WriteLine($"{DateTime.Now.ToLongTimeString()} Processado: {qtd:N0} {cad.NomeDoadorRFB}");
            }
            buffer.Flush();
        }
    }
}
