using System;
using System.Collections.Generic;
using System.Linq;
using RafaelEstevam.Simple.Spider;
using RafaelEstevam.Simple.Spider.Helper;
using RafaelEstevam.Simple.Spider.Wrappers.HTML;

namespace Robops.Spiders.Senado.Leg.Pessoal
{
    public class CatalogarFuncionariosSenadores
    {
        List<Lib.Senado.Leg.Senador> senadores;

        public void Catalogar(int ano)
        {
            senadores = new List<Lib.Senado.Leg.Senador>();

            // obter listagem
            var page = FetchHelper.FetchResourceDocument(new Uri("https://www25.senado.leg.br/web/transparencia/sen/"), 
                                                         enableCaching: true);
            var select = new Select(page.DocumentNode.SelectSingleNode("//select"));
            var idsSenadores = select.GetItems()
                                     .Select(id => id.Value.Trim())
                                     .Where(id => !string.IsNullOrEmpty(id));

            // caminhar pelo site do senado obtendo os demais dados
            var init = InitializationParams
                .Default002()
                .SetConfig(c => c.Disable_AutoAnchorsLinks());
            var spider = new SimpleSpider("Senado.Leg",
                                          new System.Uri("https://www6g.senado.leg.br/"),
                                          init);

            spider.FetchCompleted += Spider_FetchCompleted;

            foreach (var i in idsSenadores)
            {
                spider.AddPage(montarUriSenador(ano, i),
                               spider.BaseUri);
            }

            spider.Execute();

        }

        private Uri montarUriSenador(int ano, string id)
        {
            return new Uri($"https://www6g.senado.leg.br/transparencia/sen/{id}/pessoal/?local=gabinete&ano={ano}&vinculo=TODOS#conteudo_transparencia");
        }

        private void Spider_FetchCompleted(object Sender, FetchCompleteEventArgs args)
        {
            var senador = new Lib.Senado.Leg.Senador()
            {
                CodigoSenador = ConversionHelper.ToInt(args.Link.Uri.Segments[3].Replace("/", ""), 0),
            };
            
            var hObj = args.GetHObject();

            var dl_dd = hObj["dd"].ToArray();
            senador.NomeCivil = dl_dd[0].Trim();
            senador.Nascimento = dl_dd[1].Trim();
            senador.Naturalidade = dl_dd[2].Trim();
            senador.LocalGabinete = dl_dd[3].Trim();

            // funcionários
            var funcionarios = new List<Lib.Senado.Leg.FuncionarioGabinete>();
            var todosTrs = hObj["tr"];
            foreach (var linha in todosTrs)
            {
                var tds = linha["td"];
                if (tds.IsEmpty()) // é header da tabela
                {
                    continue;
                }

                var id = ConversionHelper.ToInt(tds[0]["a"].GetHrefValue().Split('=')[1], 0);
                var func = new Lib.Senado.Leg.FuncionarioGabinete()
                {
                    Senador = senador.CodigoSenador,
                    CodigoFuncionario = id,
                    Nome = tds[0]["span"].GetValue(),
                    Funcao = tds[1].GetValue(),
                    NomeFuncao = tds[2].GetValue(),
                };

                if(funcionarios.Any( o => o.CodigoFuncionario == id)) continue;

                funcionarios.Add(func);
            }
            senador.Gabinete = funcionarios.ToArray();

            senadores.Add(senador);
        }
    }
}
