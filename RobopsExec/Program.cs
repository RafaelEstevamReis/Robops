using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Net.RafaelEstevam.Spider.Helper;
using Newtonsoft.Json;
using Robops.Lib.Camara.Leg.API;
using Robops.Spiders.Camara.Leg.Cota;

namespace RobopsExec
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Lendo deputados");

            var deputados = Directory
                .GetFiles(@"B:\Temp\api\deputados\", "*.json")
                .Select(f => JsonConvert.DeserializeObject<Deputado>(File.ReadAllText(f)))
                .ToDictionary(o => o.dados.cpf);

            // http://agencia.tse.jus.br/estatistica/sead/odsele/consulta_cand/consulta_cand_2020.zip -> Brasil.csv
            var arquivo = @"B:\Temp\consulta_cand_2020_BRASIL.csv";
            using var fs = File.OpenRead(arquivo);
            var lines = CSVHelper.FileSplit(new StreamReader(fs, Encoding.UTF7))
                                 .Skip(1);

            Console.WriteLine("Lendo candidatos");

            foreach (var line in lines)
            {
                // [20] CPF
                var cpf = line[20];

                if (deputados.ContainsKey(cpf))
                {
                    var deputado = deputados[cpf];
                    Console.WriteLine($"{deputado.dados.cpf} {deputado.dados.nomeCivil} -> {line[12]}/{line[14]} [{line[28]}]");
                }
            }

            Console.WriteLine("Fim");
        }

        private static void coletarDeputadosApi()
        {
            // https://dadosabertos.camara.leg.br/arquivos/deputados/csv/deputados.csv
            var arquivo = @"B:\Temp\deputados.csv";
            using var fs = File.OpenRead(arquivo);
            using var sr = new StreamReader(fs);
            var lines = CSVHelper.FileSplit(sr)
                                 .Skip(1);
            foreach (var l in lines)
            {
                var idLegislaturaFinal = l[3];

                if (idLegislaturaFinal != "56") continue;

                var uri = new Uri(l[0]);
                var txt = FetchHelper.FetchResourceText(uri, enableCaching: true);
                File.WriteAllText($@"B:\Temp\api\deputados\{uri.Segments[^1]}.json", txt);
            }
        }

        static void coletarTodos()
        {
            var spider = new SpiderCotaParlamentar(2020, 8);
            spider.Executar();

            var despesas = spider.ListaDespesas.ToArray();
            Console.WriteLine($"Coletado: {despesas.Sum(d => d.ValorDespesa):C2} em {despesas.Length} despesas");
            var deputados = spider.ListaDeputados.ToArray();
            Console.WriteLine($" em {deputados.Length} deputados");
        }
    }
}
