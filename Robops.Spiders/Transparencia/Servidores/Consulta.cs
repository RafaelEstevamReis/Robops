using System;
using RafaelEstevam.Simple.Spider.Helper;
using Robops.Lib.Transparencia.Servidores;
using Simple.Sqlite;

namespace Robops.Spiders.Transparencia.Servidores
{
    public class Consulta
    {
        public static void run()
        {
            var db = new NoSqliteStorage("robops_servidores.db");
            var cfg = new ConfigurationDB("robops_servidores.db");

            int comecarPagina = cfg.ReadConfig("LastPage","", 1440);
            // catalogar todos os servidores
            for (int i = comecarPagina; ; i++)
            {
                var resultado = FetchHelper.FetchResourceJson<ConsultaServidores>(buildJsonUri(i));

                Console.WriteLine($"{DateTime.Now.ToLongTimeString()} Pagina {i} - {resultado.data.Length} lidos");
                db.Store(resultado.data, d => d.id);
                cfg.SetConfig("LastPage", "", i);

                //foreach (var d in resultado.data)
                //{
                //    db.Store(d.id, d);
                //}

                if (resultado.data.Length < 50) 
                    break; // terminou
            }

        }

        private static Uri buildJsonUri(int pagina)
        {
            int tamanho = 50;
            int offSet = pagina * tamanho;
            return new Uri($"http://www.portaltransparencia.gov.br/servidores/consulta/resultado?paginacaoSimples=false&tamanhoPagina={tamanho}&offset={offSet}&direcaoOrdenacao=asc&colunaOrdenacao=nome&colunasSelecionadas=detalhar%2Ctipo%2Ccpf%2Cnome%2CorgaoServidorExercicio%2CorgaoServidorLotacao%2Cmatricula%2CtipoVinculo%2Cfuncao%2CorgaoSuperiorServidorLotacao%2CorgaoSuperiorServidorExercicio%2CunidadeOrganizacionalServidorLotacao%2CunidadeOrganizacionalServidorExercicio%2Ccargo%2Catividade%2Clicenca&_=1611368959485");
        }
    }
}
