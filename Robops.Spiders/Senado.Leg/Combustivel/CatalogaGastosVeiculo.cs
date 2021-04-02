using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using RafaelEstevam.Simple.Spider.Helper;
using Robops.Lib;

namespace Robops.Spiders.Senado.Leg.Combustivel
{
    public class CatalogaGastosVeiculo
    {
        public static void run()
        {
            List<VeiculoModel> lst = new List<VeiculoModel>();
            FastCsv parser = new FastCsv()
            {
                Encoding = Encoding.UTF7,
                SupportQuotedLineBreaks = true,
            };
            // ler cada arquivo
            foreach (var arq in Directory.GetFiles(@"N:\Organizando\Dados Crawlers\Senado\Abastecimentos", "*.csv"))
            {
                var fi = new FileInfo(arq);
                var nameParts = fi.Name[..^4].Split('_');
                DateTime dt = new DateTime(int.Parse(nameParts[0]), int.Parse(nameParts[1]), 1);
                var lines = parser.ReadDelimiter(File.Open(arq, FileMode.Open)).Skip(1).ToArray();

                var first = lines[0]
                                 .Select(o => o.ToUpper())
                                 .ToList();

                // 2019
                //        0             1       2       3       4       5         6        7       8      9
                // "PLACA\nVINCULADA";MODELO;SENADOR;Condutor;Placa;COMBUSTÍVEL;Valor;Km Rodados;Litros;Média Km/litro
                // VINCULADA;MODELO;SENADOR;CONDUTOR;PLACA;COMBUSTIVEL;VALOR;KM RODADOS;LITROS;MÉDIA KM / LITRO
                // 2020
                //     0        1       2       3       4       5         6        7       8      9
                // VINCULADA;MODELO;GABINETE;CONDUTOR;PLACA;COMBUSTIVEL;VALOR;KM RODADOS;LITROS;MÉDIA KM / LITRO
                // 2021
                //     0        1       2       3          4        5       6        7       8
                // VINCULADA;MODELO;GABINETE;CONDUTOR;COMBUSTIVEL;VALOR;KM RODADOS;LITROS;MÉDIA KM / LITRO

                int idxlitros = first.IndexOf("LITROS");
                int idxValor = idxlitros - 1;
                int idxKm = idxlitros + 1;

                foreach (var line in lines.Skip(1))
                {
                    if (line[0].Trim() == "") continue;
                    if (line[1].Trim() == "") continue;
                    VeiculoModel veic = new VeiculoModel()
                    {
                        Data = dt,
                        Vinculo = line[0],
                        Placa = line[0],
                        Gabinete = line[2],
                        Condutor = line[3],
                        Valor = line[idxValor].ParseDecimal(),
                        KM = line[idxKm].ParseDecimal(),
                        Litros = line[idxlitros].ParseDecimal(),
                    };

                    if (dt.Year != 2021) veic.Placa = line[4];
                    if (veic.Vinculo.ToLower().StartsWith("xxxx")) veic.Vinculo = veic.Placa;

                    lst.Add(veic);
                }
            }
            lst = lst;

            // agrupados por ano
            var agrupadoAno = lst.GroupBy(v => v.Data.Year)
                              .Select(g => new
                              {
                                  ano = g.Key,
                                  itens = g.ToArray()
                              })
                              .ToArray();
            foreach (var grupo in agrupadoAno)
            {
                int ano = grupo.ano;
                // Agrupado no ano
                var veiculosAnual = grupo.itens
                       .GroupBy(v => v.Vinculo)
                       .Select(g => new VeiculoModel()
                       {
                           Vinculo = g.Key,
                           Gabinete = g.First().Gabinete,
                           Placa = g.First().Placa,
                           Valor = g.Sum(o => o.Valor),
                           KM = g.Sum(o => o.KM),
                           Litros = g.Sum(o => o.Litros),
                       })
                       .ToArray();
                VeiculoModel.Exporta($"ano_somado_{ano}.csv", veiculosAnual);

                // Rankings
                VeiculoModel.Exporta($"ano_{ano}.csv", grupo.itens.Select(v =>
                {
                    var veic = v;
                    v.Gabinete = $"{v.Data:MMM-yyyy} {v.Gabinete}";
                    return v;
                }));
            }



        }


        public class VeiculoModel
        {
            public DateTime Data { get; set; }
            public string Vinculo { get; set; }
            public string Gabinete { get; set; }
            public string Condutor { get; set; }
            public string Placa { get; set; }
            public decimal Valor { get; set; }
            public decimal KM { get; set; }
            public decimal Litros { get; set; }

            public static void Exporta(string file, IEnumerable<VeiculoModel> lista)
            {
                if (File.Exists(file)) File.Delete(file);
                File.WriteAllLines(file,
                                   new string[] { "Vinculo;Gabinete;Valor;KM;Litros" }
                                   .Union(lista.Select(v => $"{v.Vinculo};{v.Gabinete.RemoveAcentos()};R$ {v.Valor:N2};{v.KM:N2};{v.Litros:N2}")));
            }
        }
    }
}
