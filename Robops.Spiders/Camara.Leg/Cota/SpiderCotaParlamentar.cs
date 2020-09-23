using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using Net.RafaelEstevam.Spider;
using Net.RafaelEstevam.Spider.Wrappers.HTML;
using Robops.Lib;
using Robops.Lib.Camara.Leg;
using Robops.Lib.Camara.Leg.Cota;
using Robops.Spiders.Extensions;

namespace Robops.Spiders.Camara.Leg.Cota
{
    public class SpiderCotaParlamentar
    {
        static readonly string urlBase = "https://www.camara.leg.br/cota-parlamentar/sumarizado?nuDeputadoId={0}&dataInicio={1}/{2}&dataFim={1}/{2}&despesa=&nomeHospede=&nomePassageiro=&nomeFornecedor=&cnpjFornecedor=&numDocumento=&sguf=&filtroNivel1=1&filtroNivel2=2&filtroNivel3=3";

        public int Ano { get; }
        public int Mes { get; }

        public List<Deputado> ListaDeputados = new List<Deputado>();
        public List<Despesa> ListaDespesas = new List<Despesa>();

        public SpiderCotaParlamentar(int Ano, int Mes)
        {
            this.Ano = Ano;
            this.Mes = Mes;
        }

        List<Uri> listaNotasAcessar = new List<Uri>();
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
            spider.Execute();
        }
        private void spider_ShouldFetch(object Sender, ShouldFetchEventArgs args)
        {
            // O site da câmara retorna um erro 500 para os registros vazios
            // para fins de exemplo, vou só pular a maioria aqui
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

            if (IDs.Any(id => args.Link.ToString().Contains($"={id}&"))) args.Cancel = true;
        }

        private void Spider_FetchCompleted(object Sender, FetchCompleteEventArgs args)
        {
            var spider = Sender as SimpleSpider;
            if (args.Link == "https://www.camara.leg.br/cota-parlamentar/index.jsp")
            {
                // coletar todos parlamentares
                Tag tag = new Tag(args.GetDocument());
                var lista = tag.SelectTags("//ul[@id=\"listaDeputados\"]//span");

                foreach (var Mes in Enumerable.Range(1, 8))
                {
                    var deputados = lista
                        .Select(o => new
                        {
                            Codigo = o.Id,
                            Nome = o.InnerText.Trim()
                        })
                        .Select(dados => montaLinkDeputado(dados.Codigo, Mes, Ano));
                    spider.AddPages(deputados, args.Link);
                }
            }
            else if (args.Link.ToString().Contains("sumarizado?"))
            {
                processaSumarizado(spider, args);
            }
            else if (args.Link.ToString().Contains("analitico?"))
            {
                processaAnalitico(spider, args);
            }
            else if (args.Link.ToString().Contains("documento?"))
            {
                //processaDocumento(spider, args);
            }
            else if (args.Link.ToString().Contains("/nota-fiscal-eletronica?"))
            {
                processaNF(spider, args);
            }
            else { }
        }
        private void processaSumarizado(SimpleSpider spider, FetchCompleteEventArgs args)
        {
            // cataloga Deputado
            var tag = new Tag(args.GetDocument());

            var id = args.Link.Uri.Query
                .Split('&')[0] // primeiro bloco
                .Split('=')[1] // após o igual
                .ToInt();

            var h3 = tag.SelectTag("//h3[@class=\"header\"]");
            ListaDeputados.Add(new Deputado()
            {
                Id = id,
                Nome = h3.SelectTag<Anchor>().InnerText.Trim(),
                PartidoLideranca = h3.Node.ChildNodes[2].InnerText.Trim()
            });

            // Carrega despesas
            var linhas = tag.SelectTags("//table//tr")
                .Skip(1) // Ignora o Header
                .SkipLast(1); // Ignora o total

            var dados = linhas
                .Select(tr => tr.SelectTag<Anchor>());
            spider.AddPages(dados, args.Link);
        }
        private void processaAnalitico(SimpleSpider spider, FetchCompleteEventArgs args)
        {
            var linhas = new Tag(args.GetDocument())
                .SelectTags("//table/tbody/tr")
                .SkipLast(1); // Ignora o total

            foreach (var linha in linhas)
            {
                var lnk = linha.SelectTag<Anchor>(".//a");
                if (lnk.Href.Contains("/documento?nuDeputadoId"))
                {
                    spider.AddPage(lnk, args.Link);
                }
                // https://www.camara.leg.br/cota-parlamentar/nota-fiscal-eletronica?ideDocumentoFiscal=0000000
                if (lnk.Href.Contains("/nota-fiscal-eletronica?"))
                {
                    spider.AddPage(lnk, args.Link);
                }

            }
        }
        private void processaDocumento(SimpleSpider spider, FetchCompleteEventArgs args)
        {
            int idDeputado = args.Link.Uri.Query
                   .Split('&')[0]
                   .Split('=')[1]
                   .ToInt();

            var hObj = args.GetHObject();
            var ULs = hObj["ul > .listaDefinicao"].ToArray();

            var ulDados = ULs[1];
            var ulFornecedor = ULs[2];
            var ulValorDespesa = ULs[3];
            var ulValores2 = ULs[4];

            int codigoDespesa = args.Link.Uri.Query
                .Split('&')[3]
                .Split('=')[1]
                .ToInt();
            //string nomeDepsesa = ulDados["span"][1].Trim();

            string numero = ulDados["span"][5].Trim();
            string dtEmissao = ulDados["span"][7].Trim();
            string competencia = ulDados["span"][9].Trim();

            string fornecedorNome = ulFornecedor["span"][1].Trim();
            string fornecedorCnpj = ulFornecedor["span"][5].Trim();

            string valorDespesa = ulValorDespesa["span"][0].Trim();
            string deducoes = ulValores2["span"][0].Trim();
            string glosas = ulValores2["span"][1].Trim();
            string restituicoes = ulValores2["span"][2].Trim();
            string reembolso = ulValorDespesa["span"][5].Trim();

            ListaDespesas.Add(new Despesa()
            {
                IdDeputado = idDeputado,
                TipoDespesa = (TiposDespesa)codigoDespesa,
                Numero = numero,

                DocumentoFornecedor = fornecedorCnpj,
                NomeFornecedor = WebUtility.HtmlDecode(fornecedorNome),

                DataEmissao = LocalizationHelper.ParseDatetime(dtEmissao),
                DataCompetencia = LocalizationHelper.ParseDatetime("01/" + competencia),

                ValorDespesa = converteValor(valorDespesa),
                Deducoes = converteValor(deducoes),
                Glosas = converteValor(glosas),
                Restituicoes = converteValor(restituicoes),
                Reembolso = converteValor(reembolso)
            });
        }

        private void processaNF(SimpleSpider spider, FetchCompleteEventArgs args)
        {
            if (args.Html.Contains("<!--NFE-API-->"))
            {
            }
            else
            {      
                // Ainda não há o que fazer
                return;
            }

            var html = args.Html.Substring(args.Html.IndexOf("<!--NFE-API-->"));

            if (html.Contains(".location"))
            {
            }
            else
            {
                // Ainda não há o que fazer
                return;
            }

            html = html.Substring(html.IndexOf("http"));
            var url = html.Substring(0, html.IndexOf("\""));
            listaNotasAcessar.Add(new Uri(url));
        }

        private static decimal converteValor(string valor)
        {
            if (valor.Contains(":")) valor = valor.Split(':')[1];
            valor = valor.Trim();

            if (valor[0] == '(') valor = valor.Replace("(", "").Replace(")", "");

            return LocalizationHelper.ParseDecimal(valor);
        }

        private static Uri montaLinkDeputado(string codigo, int mes, int ano)
        {
            return new Uri(string.Format(urlBase, codigo, mes, ano));
        }
    }
}
