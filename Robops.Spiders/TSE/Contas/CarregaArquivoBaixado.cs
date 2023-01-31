using System;
using RafaelEstevam.Simple.Spider.Helper;
using Robops.Lib;
using Robops.Lib.Transparencia;
using Robops.Lib.TSE.Contas;
using Simple.DatabaseWrapper;
using Simple.Sqlite;

namespace Robops.Spiders.TSE.Contas
{
    public class CarregaArquivoBaixado
    {
        public static void run(ConnectionFactory db)
        {
            using (var cnn = db.GetConnection())
            {
                cnn.CreateTables()
                  .Add<ReceitasModel>()
                  .Commit();
            }
            string path;

            //foreach (var file in Directory.GetFiles(@"N:\Organizando\Dados Crawlers\TSE\", "*.zip"))
            //{
            //    processaArquivo(db, file);
            //}

            processaArquivo2018_2020(db, @"N:\Organizando\Dados Crawlers\TSE\prestacao_de_contas_eleitorais_candidatos_2020.zip");

        }

        private static void processaArquivo(ConnectionFactory db, string file)
        {
            if (file.EndsWith("2020.zip")) processaArquivo2018_2020(db, file);
            else if (file.EndsWith("2018.zip")) processaArquivo2018_2020(db, file);
            else if (file.EndsWith("2016.zip")) processaArquivo2016(db, file);
            else if (file.EndsWith("2014.zip")) processaArquivo2014(db, file);
            else if (file.EndsWith("2012.zip")) processaArquivo2012(db, file);
            else if (file.EndsWith("2010.zip")) processaArquivo2010(db, file);
            else if (file.EndsWith("2008.zip")) processaArquivo2008(db, file);
            else if (file.EndsWith("2006.zip")) processaArquivo2006(db, file);
            else if (file.EndsWith("2004.zip")) processaArquivo2004(db, file);
            else if (file.EndsWith("2002.zip")) processaArquivo2002(db, file);
        }

        private static void processaArquivo2018_2020(ConnectionFactory db, string zipName)
        {
            processaCsvZipado(db, zipName,
                              n => n.StartsWith("receitas_candidatos_2") && n.EndsWith("_BRASIL.csv"),
                              row => new ReceitasModel()
                              {
                                  Ano = row[2].ToInt(),
                                  CodigoEleicao = row[5].ToInt(),
                                  UF = row[12],
                                  NumeroCandidato = row[19].ToInt(),
                                  NomeCandidato = row[20],
                                  DocumentoDoador = row[36],
                                  NomeDoadorRFB = row[38],
                                  Valor = row[56].ToDecimal(0),
                                  FormaPagamento = processaFormaPagamento(row[33]),
                              });
        }
        private static void processaArquivo2016(ConnectionFactory db, string zipName)
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
                                  FormaPagamento = processaFormaPagamento(row[28]),
                              });
        }
        private static void processaArquivo2014(ConnectionFactory db, string zipName)
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
                                  FormaPagamento = processaFormaPagamento(row[25]),
                              });
        }
        private static void processaArquivo2012(ConnectionFactory db, string zipName)
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
                                  FormaPagamento = processaFormaPagamento(row[26]),
                              });
        }
        private static void processaArquivo2010(ConnectionFactory db, string zipName)
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
                                  FormaPagamento = processaFormaPagamento(row[17]),
                              });
        }
        private static void processaArquivo2008(ConnectionFactory db, string zipName)
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
                                  FormaPagamento = processaFormaPagamento(row[18]),
                              });
        }
        private static void processaArquivo2006(ConnectionFactory db, string zipName)
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
                                  FormaPagamento = processaFormaPagamento(row[13]),
                              });
        }
        private static void processaArquivo2004(ConnectionFactory db, string zipName)
        {
            processaCsvZipado(db, zipName,
                              n => n == "ReceitaCandidato.CSV",
                              row => new ReceitasModel()
                              {
                                  Ano = 2004,
                                  CodigoEleicao = 0,
                                  UF = row[4],
                                  NumeroCandidato = row[3].ToInt(),
                                  DocumentoDoador = row[16],
                                  NomeDoadorRFB = row[15],
                                  Valor = row[9].ToDecimal(0),
                                  FormaPagamento = processaFormaPagamento(row[13]),
                              });
        }
        private static void processaArquivo2002(ConnectionFactory db, string zipName)
        {
            // O arquivo original baixado está com o ZIP corrompido
            // mas pode ser aberto e resalvo
            processaCsvZipado(db, zipName,
                              n => n == "ReceitaCandidato.csv",
                              row => new ReceitasModel()
                              {
                                  Ano = 2002,
                                  CodigoEleicao = 0,
                                  UF = row[1],
                                  NumeroCandidato = row[5].ToInt(),
                                  DocumentoDoador = row[7],
                                  NomeDoadorRFB = row[9],
                                  Valor = row[10].ToDecimal(0),
                                  FormaPagamento = processaFormaPagamento(row[11]),
                              });
        }

        private static ReceitasModel.FormasPagamento processaFormaPagamento(string forma)
        {
            // ordenado por frequência
            if (forma == "Estimado") return ReceitasModel.FormasPagamento.Estimado;
            if (forma.ToLower().Contains("dep")) return ReceitasModel.FormasPagamento.Deposito;
            if (forma.ToLower().Contains("transf")) return ReceitasModel.FormasPagamento.Transferencia;
            if (forma == "Em espécie") return ReceitasModel.FormasPagamento.Dinheiro;
            if (forma == "Cheque") return ReceitasModel.FormasPagamento.Cheque;
            if (forma.ToLower().Contains("crédito")) return ReceitasModel.FormasPagamento.CCredito;
            if (forma.ToLower().Contains("boleto")) return ReceitasModel.FormasPagamento.Boleto;
            if (forma.ToLower().Contains("débito")) return ReceitasModel.FormasPagamento.CDebito;
            if (forma.ToLower() == "não informado") return ReceitasModel.FormasPagamento.Outros;
            if (forma.ToLower().Contains("outros t")) return ReceitasModel.FormasPagamento.Outros;
            if (forma.StartsWith("--")) return ReceitasModel.FormasPagamento.Outros;
            if (forma== "Despesa não quitada") return ReceitasModel.FormasPagamento.Outros;
            
            return ReceitasModel.FormasPagamento.Outros;
        }

        private static void processaCsvZipado(ConnectionFactory db, string zipName, Func<string, bool> fileFilter, Func<string[], ReceitasModel> extractionFilter)
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
                using var cnn = db.GetConnection();
                cnn.BulkInsert(data, OnConflict.Replace);
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
