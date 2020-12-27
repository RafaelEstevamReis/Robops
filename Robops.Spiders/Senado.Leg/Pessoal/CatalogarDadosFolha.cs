using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using RafaelEstevam.Simple.Spider.Helper;
using RafaelEstevam.Simple.Spider.Wrappers;
using RafaelEstevam.WebDriverController;
using RafaelEstevam.WebDriverController.Actions;
using Robops.Lib;
using Robops.Lib.Senado.Leg;

namespace Robops.Spiders.Senado.Leg.Pessoal
{
    public class CatalogarDadosFolha
    {
        public static Folha[] CarregarFolhaFuncionarios(int[] cods, int mes, int ano)
        {
            // inicializa
            var valDate = $"01/{mes:00}/{ano}";
            var lstfolha = new List<Folha>();
            using IWebDriver driver = new ChromeDriver();
            var ctr = new Controller(driver);

            // navega pro captcha e aguarda o preenchimento do primeiro
            ctr.Do(new Redirect(buildStartUrl(cods[0])));
            while (!ctr.ElementExists(By.ClassName("box_texto")))
            {
                Console.WriteLine("Definindo data");
                ctr.Interact(By.Name("fmes"), (d, e) =>
                {
                    var select = new OpenQA.Selenium.Support.UI.SelectElement(e);
                    select.SelectByValue(valDate);
                });

                Console.WriteLine("Aguardando captcha");
                //ctr.FindElement(By.TagName("h4"))
                ctr.WaitUntil_UserNavigate();
            }
            // reutiliza a sessão para consultar todos os outros
            foreach (var c in cods)
            {
                processaCodigo(lstfolha, ctr, c, new DateTime(ano, mes, 1));
            }

            return lstfolha.ToArray();
        }

        private static void processaCodigo(List<Folha> lstfolha, Controller ctr, int cod, DateTime periodo)
        {
            ctr.Do(new Redirect(buildUrl(cod)))
               // processa a resposta
               .InspectIf(
                   // somente se há dados
                   (ctr) =>
                   {
                       return ctr.FirstElementOrDefault(By.ClassName("rodape_tabela_resposta")) != null;
                   },
                   // havendo dados...
                   (ctr) =>
                   {
                       var doc = HtmlParseHelper.ParseHtmlDocument(ctr.PageSource);
                       var hObj = new HObject(doc);

                       // //section/div/div/div/
                       var painelDados = hObj["section > div > div > div"];

                       var folha = preencheFolha(hObj);
                       folha.CodigoFuncionario = cod;
                       folha.Referencia = periodo;

                       lstfolha.Add(folha);
                   })
               // respira ...
               .Sleep(100);
        }

        private static Folha preencheFolha(HObject hObj)
        {
            Folha folha = new Folha()
            {
                CarregouDados = true,
            };

            var trs = hObj["tr"];
            foreach (var linha in trs)
            {
                var valores = linha["td"].TrimAll();
                if (valores.Length < 2) continue;

                var nome = valores[0].ToLower()
                                     .Replace("&nbsp;","")
                                     .Trim();
                var valor = valores[1].ToDecimal(0);

                switch (nome)
                {
                    case "tipo da folha":
                        folha.TipoFolha = valores[1].Trim();
                        break;
                    case "": // ??
                        folha.ValorNaoIdentificado += valor;
                        break;

                    case "estrutura remuneratória básica":
                        folha.RemuneracaoBasica = valor;
                        break;
                    case "vantagens pessoais":
                        folha.VantagensPessoais = valor;
                        break;

                    case "vantagens eventuais":
                        // totais do bloco de baixo
                        break;
                    case "função comissionada":
                        folha.FuncaoComissionada = valor;
                        break;
                    case "antecipação e gratificação natalina":
                        folha.AntecipacaoGratificacaoNatalina = valor;
                        break;
                    case "horas extras":
                        folha.HorasExtras = valor;
                        break;
                    case "outras remunerações eventuais/provisórias":
                        folha.OutrasRemuneracoesEventuais = valor;
                        break;

                    case "abono de permanência":
                        folha.AbonoPermanencia = valor;
                        break;

                    case "descontos obrigatórios":
                        // totais do bloco de baixo
                        break;
                    case "reversão do teto constitucional":
                        folha.ReversaoTextoConstitucional = valor;
                        break;
                    case "imposto de renda":
                        folha.ImpostoRenda = valor;
                        break;
                    case "inss":
                    case "inss grat natalina":
                        folha.INSS = valor;
                        break;
                    case "psss":
                    case "psss (lei 12.618/12)":
                        folha.PSSS = valor;
                        break;
                    case "faltas":
                        folha.Faltas = valor;
                        break;

                    case "remuneração após descontos obrigatórios":
                        folha.RemuneracaoAposDescontosObrigatorios = valor;
                        break;

                    case "vantagens indenizatórias e compensatórias":
                        // totais do bloco de baixo
                        break;
                    case "diárias":
                        folha.RemuneracaoAposDescontosObrigatorios = valor;
                        break;
                    case "auxílios":
                        // totais do bloco de baixo
                        break;
                    case "auxílio-transporte - repasse":
                        folha.Auxilio_Transporte_Repasse = valor;
                        break;
                    case "auxílio-transporte":
                        folha.Auxilio_Transporte = valor;
                        break;
                    case "auxílio-alimentação":
                        folha.Auxilio_Alimentacao = valor;
                        break;

                    case "outras vantagens indenizatórias":
                        folha.OutrasVantagensIndenizatorias = valor;
                        break;
                    case "adicional de insalubridade":
                        folha.AdicionalInsalubridade += valor;
                        break;
                    case "férias indenizadas":
                    case "adicional de férias indenizadas":
                    case "férias indenizadas (proporcionais)":
                    case "adicional de férias proporcionais indenizadas":
                        folha.Ferias += valor;
                        break;
                    case "adicional noturno em serviço extraordinário":
                    case "adicional noturno":
                        folha.AdicionalNoturno += valor;
                        break;
                    case "auxílio-transporte estágio":
                        folha.ValorNaoIdentificado += valor;
                        break;

                    default:
                        folha.ValorNaoIdentificado += valor;
                        break;
                }
            }

            return folha;
        }

        static string buildStartUrl(int funcionario)
        {
            return $"http://www.senado.leg.br/transparencia/rh/servidores/detalhe.asp?fcodigo={funcionario}";
        }
        static string buildUrl(int funcionario)
        {
            return $"http://www.senado.leg.br/transparencia/rh/servidores/remuneracao.asp?fcodigo={funcionario}&fvinculo=";
        }

    }
}
