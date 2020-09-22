using System;
using System.Linq;
using Robops.Spiders.Camara.Leg.Cota;

namespace RobopsExec
{
    class Program
    {
        static void Main(string[] args)
        {
            var spider = new SpiderCotaParlamentar(2020, 8);
            spider.Executar();

            var despesas = spider.ListaDespesas.ToArray();
            Console.WriteLine($"Coletado: {despesas.Sum(d => d.ValorDespesa):C2} em {despesas.Length} despesas");
            var deputados = spider.ListaDeputados.ToArray();
            Console.WriteLine($" em {deputados.Length} deputados");

            Console.WriteLine("Fim");
        }
    }
}
