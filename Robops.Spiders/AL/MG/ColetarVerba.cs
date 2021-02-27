using System;
using RafaelEstevam.Simple.Spider.Helper;
using Robops.Lib.AL.MG;

namespace Robops.Spiders.AL.MG
{
    public class ColetarVerba
    {
        public static void run(int ano)
        {
            var deputados = FetchHelper.FetchResourceXml<ListaDeputado>(new Uri("http://dadosabertos.almg.gov.br/ws/deputados/em_exercicio"), enableCaching: true);
            deputados = deputados;

            foreach (var d in deputados.deputados)
            {
                for (int mes = 1; mes <= 12; mes++)
                {
                    string url = $"http://dadosabertos.almg.gov.br/ws/prestacao_contas/verbas_indenizatorias/legislatura_atual/deputados/{d.id}/{ano}/{mes}";
                    var gastos = FetchHelper.FetchResourceXml<ListaResumoVerba>(new Uri(url), enableCaching: true);

                    //System.Threading.Thread.Sleep(5000);
                }
            }

        }
    }
}
