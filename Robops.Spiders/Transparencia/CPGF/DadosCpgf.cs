using OpenQA.Selenium.DevTools.V85.Network;
using Robops.Lib.Transparencia;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Robops.Spiders.Transparencia.CPGF
{
    public class DadosCpgf
    {
        public static void BaixarAno(int ano, string diretorio)
        {
            if (!Directory.Exists(diretorio)) Directory.CreateDirectory(diretorio);

            List<string> lines = new List<string>();
            for (int mes = 1; mes <= 12; mes++)
            {
                if (ano == 2022 && mes == 12) continue; // Não tem 

                string file = Path.Combine(diretorio, $"cpgf_{ano}{mes:00}.zip");

                BaixaArquivo(ano, mes, file);
                lines.AddRange(carregaLinhas(file, incluirHeader: mes == 1));
            }
            string pathCsvAno = Path.Combine(diretorio, $"arquivoAno_{ano}.csv");
            File.WriteAllLines(pathCsvAno, lines.ToArray());
        }
        public static void BaixaArquivo(int ano, int mes, string destinationFile)
        {
            if (File.Exists(destinationFile))
            {
                System.Console.WriteLine("Skipping " + destinationFile);
                return;
            }

            string url = $"https://portaldatransparencia.gov.br/download-de-dados/cpgf/{ano}{mes:00}";
            WebClient wc = new WebClient();
            wc.Headers[HttpRequestHeader.UserAgent] = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/109.0.0.0 Safari/537.36";
            System.Console.WriteLine($"Baixando {ano}{mes:00}...");
            wc.DownloadFile(url, destinationFile);
        }
        private static IEnumerable<string> carregaLinhas(string file, bool incluirHeader)
        {
            var ptzip = new LeitorZipTransparencia(file);
            ptzip.IgnoreFirstLine = !incluirHeader;
            return ptzip.ReadLines();
        }
    }
}
