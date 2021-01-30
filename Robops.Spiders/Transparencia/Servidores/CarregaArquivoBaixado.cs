using System;
using System.IO;
using RafaelEstevam.Simple.Spider.Helper;
using Robops.Lib;
using Robops.Lib.Helpers;
using Robops.Lib.Transparencia;
using Robops.Lib.Transparencia.Servidores;

namespace Robops.Spiders.Transparencia.Servidores
{
    public class CarregaArquivoBaixado
    {
        public static void run(Simple.Sqlite.SqliteDB db)
        {
            db.CreateTables()
              .Add<ServidoresCadastroModel>()
              .Commit();

            Console.WriteLine("Entre com a pasta:");
            string pasta = Console.ReadLine();

            processaPasta(pasta, db);
        }
        private static void processaPasta(string pasta, Simple.Sqlite.SqliteDB db)
        {
            foreach (var arquivo in Directory.GetFiles(pasta, "*.zip"))
            {
                bool militar = arquivo.Contains("_Militares");

                processaArquivo(arquivo, militar, db);
            }
        }

        private static void processaArquivo(string zipName, bool militar, Simple.Sqlite.SqliteDB db)
        {
            int qtd = 0;
            var zip = new LeitorZipTransparencia(zipName);
            zip.ShouldProcessFile = n => n.Contains("_Cadastro");
            zip.IgnoreFirstLine = true;
            zip.InicioLeituraArquivo += (s, a) => Console.WriteLine($"Lendo {a}");

            var zipLines = zip.ReadLines();
            var rows = CSVHelper.DelimiterSplit(zipLines, ';');

            var buffer = new DataBuffer<ServidoresCadastroModel>(10000, data => db.BulkInsert(data));
            foreach (var row in rows)
            {
                var cad = new ServidoresCadastroModel()
                {
                    ServidorCadastroMilitar = militar,

                    ID_ServidorPortal = row[0].ToInt(),
                    Nome = row[1],
                    CPF_6D = row[2],
                    Matricula_3D = row[3],
                    CodigoOrgaoSuperiorLotacao = row[19].ToInt(),
                    CodigoOrgaoLotacao = row[17].ToInt(),
                    DocumentoIngressoServicoPublico = row[36],
                    TipoVinculo = row[27].ToInt(),
                };
                buffer.Add(cad);
                qtd++;

                if (qtd % buffer.Quantity == 0) Console.WriteLine($"Processado: {qtd}");
            }
            buffer.Flush();
        }
    }
}
