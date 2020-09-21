using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Net.RafaelEstevam.Spider;
using Net.RafaelEstevam.Spider.Wrappers.HTML;
using Robos.Spiders.Extensions;

namespace Robos.Spiders.Camara.Leg.Cota
{
    public class SpiderCotaParlamentar
    {
        static readonly string urlBase = "https://www.camara.leg.br/cota-parlamentar/sumarizado?nuDeputadoId={0}&dataInicio={1}/{2}&dataFim={1}/{2}&despesa=&nomeHospede=&nomePassageiro=&nomeFornecedor=&cnpjFornecedor=&numDocumento=&sguf=&filtroNivel1=1&filtroNivel2=2&filtroNivel3=3";

        CancellationTokenSource cancel;

        public int Ano { get; }
        public int Mes { get; }

        public SpiderCotaParlamentar(int Ano, int Mes)
        {
            this.Ano = Ano;
            this.Mes = Mes;
            cancel = new CancellationTokenSource();

            //Console.CancelKeyPress += (s, e) =>
            //{
            //    cancel.Cancel(); // Interrompe a Spider
            //};
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
            // Ignorar alguns endereços
            spider.ShouldFetch += spider_ShouldFetch;
            // mandar ver ...
            spider.Execute(cancel.Token);
        }
        private void spider_ShouldFetch(object Sender, ShouldFetchEventArgs args)
        {
            var IDs = new string[] { "3070", "3005",  "958", "2377", // Lista geral (ano?)
                                     "2338", "1549", "3358", "3437",
                                     "3367", "3036", "2979",  "999",
                                     "3463", "3255", "2250", "3456",
                                     "1829", "2924", "3462", "3394",
                                     "2311", "1649", "3357",
                                     // Mes específico
                                     "1571", "3269", "3324", "3052",
                                     "3220", "2934", "3061", "1893",
                                     "2919", "1344", "3471", "1627",
                                     "80",   "3464", "3362", "3142",
                                     "2317", "3438", "3081", "3472",
                                     "3455", "1161", "2902", "2277",
                                     "2475", "3066", "3417", "3162",
                                     "1946", "2233", "3268"
            };

            foreach (var id in IDs)
            {
                if (args.Link.ToString().Contains($"={id}&"))
                {
                    args.Cancel = true;
                }
            }
        }

        private void Spider_FetchCompleted(object Sender, FetchCompleteEventArgs args)
        {
            var spider = Sender as SimpleSpider;
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

                spider.AddPages(deputados, args.Link);
            }
            else if (args.Link.ToString().Contains("sumarizado?"))
            {
                carregaSumarizado(spider, args);
            }
            else if (args.Link.ToString().Contains("analitico?"))
            {
                carregaAnalitico(spider, args);
            }
            else if (args.Link.ToString().Contains("documento?"))
            {
                carregaDocumento(spider, args);
            }
            else { }
        }

        private void carregaSumarizado(SimpleSpider spider, FetchCompleteEventArgs args)
        {
            var tabela = new Tag(args.GetDocument()).SelectTag("//table");
            var linhas = tabela.SelectTags(".//tr")
                .Skip(1) // Ignora o Header
                .SkipLast(1); // Ignora o total

            var dados = linhas
                .Select(tr => tr.SelectTag<Anchor>());
            spider.AddPages(dados, args.Link);
        }
        private void carregaAnalitico(SimpleSpider spider, FetchCompleteEventArgs args)
        {
            var linhas = new Tag(args.GetDocument())
                .SelectTags("//table/tbody/tr")
                .SkipLast(1); // Ignora o total

            foreach (var linha in linhas)
            {
                var lnk = linha.SelectTag<Anchor>(".//a");
                if (!lnk.Href.Contains("/documento?nuDeputadoId")) continue; // é externo
                spider.AddPage(lnk, args.Link);
            }
        }
        private void carregaDocumento(SimpleSpider spider, FetchCompleteEventArgs args)
        {
            
        }


        private static Uri montaLinkDeputado(string codigo, int mes, int ano) 
        {
            return new Uri(string.Format(urlBase, codigo, mes, ano));
        }
    }
}
