using RafaelEstevam.Simple.Spider.Helper;
using Robops.Lib;
using Robops.Lib.Transparencia;
using Robops.Lib.Transparencia.AuxilioEmergencial;
using Simple.DatabaseWrapper;
using Simple.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Robops.Spiders.Transparencia.AuxilioEmergencial
{
    public class CarregaArquivoBaixado
    {
        static HashSet<long> uids;
        static int qtd = 0;
        public static void run(ConnectionFactory db)
        {
            using (var cnn = db.GetConnection())
            {
                cnn.CreateTables()
                  .Add<AuxilioModel>()
                  .Commit();
            }

            string pasta = "";
            while (!Directory.Exists(pasta))
            {
                Console.WriteLine("Entre com a pasta:");
                pasta = Console.ReadLine();
            }

            {
                using var cnn = db.GetConnection();
                // recovery from where it stopped
                var allNis = cnn.Query<string>("SELECT NIS FROM AuxilioModel WHERE NIS IS NOT NULL", null)
                               .Select(nis => long.Parse(nis));
                uids = new HashSet<long>(allNis);
            }

            processaPasta(pasta, db);
        }
        private static void processaPasta(string pasta, ConnectionFactory db)
        {
            foreach (var arquivo in Directory.GetFiles(pasta, "*.zip"))
            {
                if (arquivo.Contains("202004")) continue; // already done
                if (arquivo.Contains("202005")) continue; // already done
                if (arquivo.Contains("202006")) continue; // already done
                if (arquivo.Contains("202007")) continue; // already done

                processaArquivo(arquivo,  db);
            }
        }

        private static void processaArquivo(string zipName, ConnectionFactory db)
        {
            var zip = new LeitorZipTransparencia(zipName);
            //zip.ShouldProcessFile = n => n.Contains("_Cadastro");
            zip.IgnoreFirstLine = true;
            zip.InicioLeituraArquivo += (s, a) => Console.WriteLine($"Lendo {a}");

            var zipLines = zip.ReadLines();
            var rows = CSVHelper.DelimiterSplit(zipLines, ';');

            var buffer = new DataBuffer<AuxilioModel>(20000, data =>
            {
                using var cnn = db.GetConnection();
                cnn.BulkInsert(data, OnConflict.Replace);
                Console.WriteLine($"# Data Write ");
            });
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

                qtd++;
                if (qtd % buffer.Quantity == 0) Console.WriteLine($"{DateTime.Now.ToLongTimeString()} Processado: {qtd:N0} {cad.Nome}");

                if (uids.Contains(cad.Key())) continue;
                uids.Add(cad.Key());

                buffer.Add(cad);

            }
            buffer.Flush();
        }
    }
}
