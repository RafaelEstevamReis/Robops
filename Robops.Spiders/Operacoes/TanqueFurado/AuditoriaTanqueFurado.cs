using System;
using System.Linq;
using OpenQA.Selenium;
using RafaelEstevam.Simple.Spider.Wrappers;
using RafaelEstevam.WebDriverController;
using RafaelEstevam.WebDriverController.Actions;
using RafaelEstevam.WebDriverController.Interaction;
using Robops.Lib.NotaFiscal;

namespace Robops.Spiders.Operacoes.TanqueFurado
{
    public class AuditoriaTanqueFurado
    {
        public static void run(Controller ctrl)
        {
            //5220 0907 5982 8500 0153 5500 1000 0224 2512 6059 6180 
            while (true)
            {
                Console.WriteLine("Digite a chave de acesso:");
                string chave = ChaveAcesso.LimparChaveAcesso(Console.ReadLine());

                if (!ChaveAcesso.EhChaveValida(chave))
                {
                    Console.WriteLine("A chave é inválida, verifique e digite novamente");
                    continue;
                }

                //var nfces = processaNFe(ctrl, chave);

                var nfces = new string[] {
"52200907598285000153650010002265121702609901",
"52200907598285000153650010002265131307114358",
"52200907598285000153650010002265141718349755",
"52200907598285000153650010002265161231939671",
"52200907598285000153650010002265171546975698",
"52200907598285000153650010002265191656861004",
"52200907598285000153650010002265201598827245",
"52200907598285000153650010002265211915361868",
"52200907598285000153650010002265221173653321",
"52200907598285000153650010002265231277514538",
"52200907598285000153650010002265541319758975",
"52200907598285000153650010002265551485681194",
"52200907598285000153650010002266141372826785",
"52200907598285000153650010002266191745822034",
"52200907598285000153650010002266201162392670",
"52200907598285000153650010002266221385328950",
"52200907598285000153650010002266241050293564",
"52200907598285000153650010002266261522521470",
"52200907598285000153650010002266271099713852"
                };

                processaNFCes(ctrl, nfces);
              
            }
        }

        private static string[] processaNFe(Controller ctrl, string chave)
        {
            ctrl.GoTo("http://www.nfe.fazenda.gov.br/")
                .WaitUntil_Exists(By.LinkText("Serviços"))
                .HoverOn(By.LinkText("Serviços"))
                .ClickOn(By.LinkText("Consultar NF-e"));
            // wait
            ctrl.Interact(By.Id("ctl00_ContentPlaceHolder1_txtChaveAcessoResumo"),
                (c, e) => e.SendKeys(chave));

            Console.WriteLine("Aguardando usuário resolver o captcha");
            ctrl.WaitUntil_UserNavigate();
            ctrl.WaitUntil_Exists(By.Id("ctl00_ContentPlaceHolder1_divBotoesConsulta"));

            retryIfFail(() =>
            {
                // tela da nota
                // clica nos produtos e coleta
                ctrl.ClickOn(By.Id("tab_3"));
                // coleta produtos
                ctrl.Inspect(lerProdutosNFe);
            });


            string[] chaves = new string[0];
            retryIfFail(() =>
            {
                // clica nas inf. add. e coleta
                ctrl.ClickOn(By.Id("tab_7"));
                ctrl.Inspect((c, h) => { chaves = lerReferenciasNFe(c, h); });
            });

            if (chaves != null)
            {
                foreach (var c in chaves) Console.WriteLine(c);
            }

            return chaves;
        }
        private static void processaNFCes(Controller ctrl, string[] chavesReferenciadasCatalogadas)
        {
            // processa NFCe
            foreach (var c in chavesReferenciadasCatalogadas)
            {
                ctrl.GoTo($"http://nfe.sefaz.go.gov.br/nfeweb/sites/nfce/render/xml-consulta-completa?g-recaptcha-response=03AGdBq27XLoZP63KxfaVJo0X5bbIkOifk_n01UOI7jQr6jXEY3WLN9uWRNl5kNiRrPbVlf7QpBG7VvdDqEAnc_cJ6nktGliaATJW9BvfVrDFbOLuyaGB6BzbeVRd1NIyJ1X-txU4CauxyBPSkNeMSgZa9tPs2eOmb9wIHX4oUFGiXk3hzruDXu0D9V88h52-IvXV4q9A4dOSkvQsbQ3ye5aowTa_4cTpcxIExA0V5VCOAEYSeeuvSDNOs0ogu-Wn8hCr5CY4m-gx1bAFyjxIufDgK4KNlA49oZH1r4iJmzDUh1xqy5vIKHV64gOvopemg6Xs6kJA-VYES9k2d7dF4xGuiXqejTw5AKxL8J3LXLtpTZR_6jpIzr3dAxcxfKo2LTHYCQ1qN-jwNG7HwUxZcWX8q-H9bm7VtpPYzEgt3SSNWYp7lowa7cLTxd1kcEYDjOoWPFsP5cV3fXsrYHqbysR5gCI8U7lYSEBARcYo-YN34vNr-FlPmfOFfWHkRH4qmH-nsxEsjj8Zjk2wI5V9Xh9DXXD_0YxoyzQ&chaveAcesso={c}");
                retryIfFail(() => processaNFCeIndividual(ctrl, c));
            }
        }

        private static void processaNFCeIndividual(Controller ctrl, string c)
        {
           
        }

        private static void retryIfFail(Action p)
        {
            while (true)
            {
                try
                {
                    p();
                    return;
                }
                catch (Exception)
                { }
            }
        }

        private static void lerProdutosNFe(Controller obj, HtmlAgilityPack.HtmlDocument doc)
        {
            var itens = doc.DocumentNode.SelectNodes("//*[@id='Prod']/fieldset/div/table[@class='toggle box']");
            foreach (var table in itens)
            {
                var hObj = new HObject(table);
                var campos = hObj["span"].TrimAll();

                string prodId = campos[0];
                string prodName = campos[1];
                string prodQtd = campos[2];
                string prodUn = campos[3];
                string prodTotal = campos[4];
            }

        }
        private static string[] lerReferenciasNFe(Controller obj, HtmlAgilityPack.HtmlDocument doc)
        {
            var allInfAddSpans = doc.DocumentNode.SelectNodes("//*[@id='Inf']//span");
            string[] chaves = allInfAddSpans.Select(o => ChaveAcesso.LimparChaveAcesso(o.InnerText))
                                            .Where(t => ChaveAcesso.EhChaveValida(t))
                                            .ToArray();
            return chaves;

        }

    }
}
