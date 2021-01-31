using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using RafaelEstevam.Simple.Spider.Helper;
using Robops.Lib;
using Robops.Lib.Helpers;
using Robops.Lib.Transparencia;
using Robops.Lib.Transparencia.AuxilioEmergencial;

namespace Robops.Spiders.Transparencia.AuxilioEmergencial
{
    public class CarregaArquivoBaixado
    {
        static HashSet<string> uids;
        static int qtd = 0;
        public static void run(Simple.Sqlite.SqliteDB db)
        {
            db.CreateTables()
              .Add<AuxilioModel>()
              .Commit();

            Console.WriteLine("Entre com a pasta:");
            string pasta = Console.ReadLine();

            uids = new HashSet<string>();


            processaPasta(pasta, db);
        }
        private static void processaPasta(string pasta, Simple.Sqlite.SqliteDB db)
        {
            foreach (var arquivo in Directory.GetFiles(pasta, "*.zip"))
            {
                processaArquivo(arquivo,  db);
            }
        }

        private static void processaArquivo(string zipName, Simple.Sqlite.SqliteDB db)
        {
            var zip = new LeitorZipTransparencia(zipName);
            //zip.ShouldProcessFile = n => n.Contains("_Cadastro");
            zip.IgnoreFirstLine = true;
            zip.InicioLeituraArquivo += (s, a) => Console.WriteLine($"Lendo {a}");

            var zipLines = zip.ReadLines();
            var rows = CSVHelper.DelimiterSplit(zipLines, ';');

            var buffer = new DataBuffer<AuxilioModel>(20000, data => db.BulkInsert(data, addReplace: true));
            foreach (var row in rows)
            {
                var cad = new AuxilioModel()
                {
                    UF = row[1],
                    Municipio = row[2].ToInt(0),
                    NIS = row[4],
                    CPF_6D = row[5],
                    Nome = row[6],
                    NIS_Responsavel = row[7],
                    CPF_6D_Responsavel = row[8],
                    Nome_Responsavel = row[9],
                    Enquadramento = row[10],
                };

                if (cad.NIS == "00000000000") cad.NIS = null;
                if (cad.NIS_Responsavel == "-2") cad.NIS_Responsavel = null;
                if (cad.Nome_Responsavel == "Não se aplica") cad.Nome_Responsavel = null;

                if (cad.NIS_Responsavel == cad.NIS) cad.NIS_Responsavel = null;
                if (cad.CPF_6D_Responsavel == cad.NIS) cad.CPF_6D_Responsavel = null;
                if (cad.Nome_Responsavel == cad.Nome) cad.Nome_Responsavel = null;

                //if (uids.Contains(cad.Key())) continue;
                //uids.Add(cad.Key());

                buffer.Add(cad);
                qtd++;

                if (qtd % buffer.Quantity == 0) Console.WriteLine($"{DateTime.Now.ToLongTimeString()} Processado: {qtd:N0} {cad.Nome}");
            }
            buffer.Flush();
        }
    }
}
