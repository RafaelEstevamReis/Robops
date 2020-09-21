using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Net.RafaelEstevam.Spider;
using Net.RafaelEstevam.Spider.Wrappers.HTML;

namespace Robos.Spiders.Camara.Leg.Cota
{
    public class SpiderCotaParlamentar
    {
        static readonly string urlBase = "https://www.camara.leg.br/cota-parlamentar/sumarizado?nuDeputadoId={0}&dataInicio={1}/{2}&dataFim={1}/{2}&despesa=&nomeHospede=&nomePassageiro=&nomeFornecedor=&cnpjFornecedor=&numDocumento=&sguf=&filtroNivel1=1&filtroNivel2=2&filtroNivel3=3";

        public int Ano { get; }
        public int Mes { get; }

        public SpiderCotaParlamentar(int Ano, int Mes)
        {
            this.Ano = Ano;
            this.Mes = Mes;
        }

        public void Executar()
        {
            var init = InitializationParams
                .Default002() // Usar configs padrão 
                .SetConfig(c => c.Set_DownloadDelay(2000) // Aguardar 2s entre requisições
                                 .Disable_AutoAnchorsLinks()); // Não sair navegando
                        
            var spider = new SimpleSpider("cota_camara", new Uri("https://www.camara.leg.br"), init);
            // Obter todos os palamentares
            spider.AddPage(new Uri("https://www.camara.leg.br/cota-parlamentar/index.jsp"), spider.BaseUri);
            // Obter páginas
            spider.FetchCompleted += Spider_FetchCompleted;
            // mandar ver ...
            spider.Execute();
        }

        private void Spider_FetchCompleted(object Sender, FetchCompleteEventArgs args)
        {
            if (args.Link == "https://www.camara.leg.br/cota-parlamentar/index.jsp")
            {
                // coletar todos parlamentares
                Tag tag = new Tag(args.GetDocument());
                var lista = tag.SelectTags("//ul[@id=\"listaDeputados\"]//span");

                var deputados = lista
                    .Select(o => new
                    {
                        Codigo = o.Id,
                        Nome = o.InnerText.Trim()
                    })
                    .Select(dados => montaLinkDeputado(dados.Codigo, Mes, Ano));

                (Sender as SimpleSpider).AddPages(deputados, args.Link);
            }
            else if (args.Link.ToString().Contains("sumarizado?"))
            {
            }
            else if (args.Link.ToString().Contains("analitico?"))
            {
            }
            else if (args.Link.ToString().Contains("documento?"))
            {
            }
        }

        private static Uri montaLinkDeputado(string codigo, int mes, int ano) 
        {
            return new Uri(string.Format(urlBase, codigo, mes, ano));
        }
    }
}
