using System;
using Robos.Spiders.Camara.Leg.Cota;

namespace RobosExec
{
    class Program
    {
        static void Main(string[] args)
        {
            var spider = new SpiderCotaParlamentar(2020, 08);
            spider.Executar();
        }
    }
}
