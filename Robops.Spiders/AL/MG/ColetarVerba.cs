using System;
using System.IO;
using RafaelEstevam.Simple.Spider.Helper;
using Robops.Lib.AL.MG;

namespace Robops.Spiders.AL.MG
{
    public class ColetarVerba
    {
        public static void run(int ano)
        {
            //if (ano != 2020) return;

            var deputados = FetchHelper.FetchResourceXml<ListaDeputado>(new Uri("http://dadosabertos.almg.gov.br/ws/deputados/em_exercicio"), enableCaching: true);

            //using var fs = File.Open($"almg_gastos_{ano}.csv", FileMode.Append, FileAccess.Write);
            using (var fs = File.Open($"almg_gastos.csv", FileMode.Append, FileAccess.Write))
            {
                using var sw = new StreamWriter(fs);

                foreach (var d in deputados.deputados)
                {
                    for (int mes = 1; mes <= 12; mes++)
                    {
                        // http://dadosabertos.almg.gov.br/ws/prestacao_contas/verbas_indenizatorias/deputados/7753/2012/1
                        //string url = $"http://dadosabertos.almg.gov.br/ws/prestacao_contas/verbas_indenizatorias/legislatura_atual/deputados/{d.id}/{ano}/{mes}";
                        string url = $"http://dadosabertos.almg.gov.br/ws/prestacao_contas/verbas_indenizatorias/deputados/{d.id}/{ano}/{mes}";

                        ListaResumoVerba gastos;
                        try
                        {
                            gastos = FetchHelper.FetchResourceXml<ListaResumoVerba>(new Uri(url), enableCaching: true);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            System.Threading.Thread.Sleep(2000); // delay for avoid 429
                            continue;
                        }
                        //System.Threading.Thread.Sleep(1000); // 1s delay for avoid 429

                        if (gastos.resumoVerba == null)
                        {
                            Console.WriteLine("NLL");
                            continue;
                        }

                        foreach (var despesa in gastos.resumoVerba)
                        {
                            foreach (var conta in despesa.listaDetalheVerba)
                            {
                                sw.WriteLine($"{d.nome};{ano};{mes};{despesa.descTipoDespesa};{conta.dataEmissao:yyyy-MM-dd};{conta.cpfCnpj};{conta.valorDespesa};{conta.valorReembolsado};{conta.nomeEmitente}");
                            }
                        }
                        sw.Flush();
                    }
                }
            }
            Console.WriteLine("Fim ANO");
        }
    }
}
