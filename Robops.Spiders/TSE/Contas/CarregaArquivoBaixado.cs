using System;
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

            string path = @"N:\Organizando\Dados Crawlers\TSE\prestacao_contas_final_2016.zip";
            baixarArquivo_modelo2016(path, db);

            //string path = @"N:\Organizando\Dados Crawlers\TSE\prestacao_de_contas_eleitorais_candidatos_2018.zip";
            //string path = @"N:\Organizando\Dados Crawlers\TSE\prestacao_de_contas_eleitorais_candidatos_2020.zip";
            //baixarArquivo_modelo2018_2020(path, db);
        }

        private static void baixarArquivo_modelo2018_2020(string zipName, SqliteDB db)
        {
            var zip = new LeitorZipTransparencia(zipName);
            // Receitas, Brasil, agnóstico ao ano
            zip.ShouldProcessFile = n => (n.StartsWith("receitas_candidatos_2") && n.EndsWith("_BRASIL.csv"));
            zip.InicioLeituraArquivo += (s,e) => Console.WriteLine(e);
            zip.IgnoreFirstLine = true;
            var zipLines = zip.ReadLines();
            var rows = CSVHelper.DelimiterSplit(zipLines, ';');

            var buffer = new DataBuffer<ReceitasModel>(20000, data =>
            {
                db.BulkInsert(data, addReplace: true);
                Console.WriteLine($"# Data Write ");
            });
            int qtd = 0;
            foreach (var row in rows)
            {
                var cad = new ReceitasModel()
                {
                    Ano = row[2].ToInt(),
                    CodigoEleicao = row[5].ToInt(),
                    UF = row[12],
                    NumeroCandidato = row[19].ToInt(),
                    DocumentoDoador = row[36],
                    NomeDoadorRFB = row[38],
                    Valor = row[56].ToDecimal(0),
                    FormaPagamento = ReceitasModel.FormasPagamento.Outros,
                };
                buffer.Add(cad);

                qtd++;
                if (qtd % buffer.Quantity == 0) Console.WriteLine($"{DateTime.Now.ToLongTimeString()} Processado: {qtd:N0} {cad.NomeDoadorRFB}");
            }
            buffer.Flush();
        }

        private static void baixarArquivo_modelo2016(string zipName, SqliteDB db)
        {
            var zip = new LeitorZipTransparencia(zipName);
            // Receitas, Brasil, agnóstico ao ano
            zip.ShouldProcessFile = n => (n.StartsWith("receitas_candidatos_") && n.EndsWith("_brasil.txt")); // !! TXT
            zip.InicioLeituraArquivo += (s, e) => Console.WriteLine(e);
            zip.IgnoreFirstLine = true;
            var zipLines = zip.ReadLines();
            var rows = CSVHelper.DelimiterSplit(zipLines, ';');

            var buffer = new DataBuffer<ReceitasModel>(20000, data =>
            {
                db.BulkInsert(data, addReplace: true);
                Console.WriteLine($"# Data Write ");
            });
            int qtd = 0;
            foreach (var row in rows)
            {
                var cad = new ReceitasModel()
                {
                    Ano = 2016,//row[2].ToInt(),
                    CodigoEleicao = row[0].ToInt(),
                    UF = row[5],
                    NumeroCandidato = row[9].ToInt(),
                    DocumentoDoador = row[16],
                    NomeDoadorRFB = row[18],
                    Valor = row[26].ToDecimal(0),
                    FormaPagamento = ReceitasModel.FormasPagamento.Outros,
                };
                buffer.Add(cad);

                qtd++;
                if (qtd % buffer.Quantity == 0) Console.WriteLine($"{DateTime.Now.ToLongTimeString()} Processado: {qtd:N0} {cad.NomeDoadorRFB}");
            }
            buffer.Flush();
        }

    }
}
