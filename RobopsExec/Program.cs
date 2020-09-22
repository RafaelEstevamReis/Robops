using System;
using System.Linq;
using Robos.Spiders.Camara.Leg.Cota;

namespace RobosExec
{
    class Program
    {
        static void Main(string[] args)
        {
            var spiders = Enumerable.Range(4, 7).Select(mes => new SpiderCotaParlamentar(2020, mes));

            spiders.AsParallel().All(spider => { spider.Executar(); return true; });

            //var spider = new SpiderCotaParlamentar(2020, 08);
            //spider.Executar();

            Console.WriteLine("Fim");

            //var despesas = spider.ListaDespesas.ToArray();
            //despesas = despesas;
        }
    }
}
