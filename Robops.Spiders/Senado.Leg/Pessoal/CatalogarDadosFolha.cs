using System;
using System.Collections.Generic;
using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using RafaelEstevam.WebDriverController.Lib;
using RafaelEstevam.WebDriverController.Lib.Actions;

namespace Robops.Spiders.Senado.Leg.Pessoal
{
    public class CatalogarDadosFolha
    {
        static void a(int[] cods, int mes, int ano)
        {

            using IWebDriver driver = new ChromeDriver();
            var ctr = new WDController(driver);

            string valDate = $"01/{mes}/{ano}";

            ctr.Do(new Redirect(buildStartUrl(cods[0])));
            while (ctr.ElementExists(By.ClassName("box_texto")))
            {
                Console.WriteLine("Definindo data");
                ctr.Interact(By.Name("fmes"), (d, e) =>
                {
                    var select = new OpenQA.Selenium.Support.UI.SelectElement(e);
                    select.SelectByValue(valDate);
                });

                Console.WriteLine("Aguardando captcha");
                ctr.WaitUntil_UserNavigate();
            }

            foreach (var c in cods)
            {
                ctr.Do(new Redirect(buildUrl(c)))
                   .InspectIf((ctr) =>
                   {
                       return ctr.FirstElementOrDefault(By.ClassName("rodape_tabela_resposta")) != null;
                   },
                   (wc, wd) =>
                   {
                   })
                   .Sleep(500);
            }

        }


        static  string buildStartUrl(int funcionario)
        {
            return $"http://www.senado.leg.br/transparencia/rh/servidores/detalhe.asp?fcodigo={funcionario}";
        }
        static string buildUrl(int funcionario)
        {
            return $"http://www.senado.leg.br/transparencia/rh/servidores/remuneracao.asp?fcodigo={funcionario}&fvinculo=";
        }

    }
}
