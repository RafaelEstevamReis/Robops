using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using RafaelEstevam.Simple.Spider.Helper;
using Robops.Lib.Camara.Leg.API;
using Robops.Spiders.Camara.Leg.Cota;

namespace RobopsExec
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Lendo deputados");

            comparaListasNomes(File.ReadAllLines(@"B:\Temp\Deputados ALMT.csv"),
                               File.ReadAllLines(@"B:\Temp\lotacionograma.csv"));           

            Console.WriteLine("Fim");
        }

        private static void comparaListasNomes(string[] Lista1, string[] Lista2)
        {
            var l1 = Lista1.Where(l => !string.IsNullOrWhiteSpace(l))
                           .OrderBy(o => o);
            var l2 = Lista2.Where(l => !string.IsNullOrWhiteSpace(l))
                           .OrderBy(o => o);

            // caso tenha CamelCase, pode ser que sejam localizados nomes grudados

            // Passe 1, acha primeiro nome
            var primeirosNomes = Lista1.Select(l => l.Split(' ')[0].ToLower())
                                       .Intersect(
                                            Lista2.Select(l => l.Split(' ')[0].ToLower())
                                       )
                                       .ToArray();

            primeirosNomes = primeirosNomes;
        }
        private static void cruzaDeputadosCandidatos()
        {
            // Coletado com coletarDeputadosApi()
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
                    Console.WriteLine($"{deputado.dados.id:000000} {deputado.dados.cpf} {deputado.dados.nomeCivil} -> {line[12]}/{line[14]} [{line[28]}]");

                    string url = $"https://www.camara.leg.br/cota-parlamentar/sumarizado?nuDeputadoId={deputado.dados.id}&dataInicio=8/2020&dataFim=10/2020&despesa=&nomeHospede=&nomePassageiro=&nomeFornecedor=&cnpjFornecedor=&numDocumento=&sguf=&filtroNivel1=1&filtroNivel2=2&filtroNivel3=3";
                    var document = FetchHelper.FetchResourceDocument(new Uri(url), enableCaching: true);
                }
            }
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
