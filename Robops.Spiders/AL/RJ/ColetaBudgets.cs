using RafaelEstevam.Simple.Spider.Helper;
using Robops.Lib.AL.RJ.ApiModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Robops.Spiders.AL.RJ
{
    public class ColetaBudgets
    {
        public static void GeraArquivoBudget(string arquivo)
        {
            var parlamentares = ColetarParlamentar.ObterParlamentares();
            using var sw = new StreamWriter(arquivo, false, Encoding.UTF8);
            sw.WriteLine($@"Parlamentar;Partido;Ano;Mês;Objeto;Data;Valor;Documento;Fornecedor");
            foreach (var p in parlamentares)
            {
                var budget = ColetaBudget(p.id);

                foreach (var meses in budget)
                {
                    foreach (var e in meses.entries)
                    {
                        sw.WriteLine($@"""{p.name}"";""{p.party.name}"";{meses.year};{meses.month};""{e.@object}"";{e.date:yyyy/MM/dd};{e.value.ToString("N2", System.Globalization.CultureInfo.InvariantCulture)};""{e.provider.cpf_cnpj}"";""{e.provider.name}""");
                    }
                }
                sw.Flush();
            }
        }
        public static IEnumerable<BudgetInfo.DataReferencia> ColetaBudget(int idParlamentar)
        {
            var uri = new Uri($"https://docigp.alerj.rj.gov.br/api/v1/congressmen/{idParlamentar}/budgets");
            var registros = FetchHelper.FetchResourceJson<BudgetInfo>(uri);

            foreach (var d in registros.data) yield return d;

            while (registros.current_page != registros.last_page)
            {
                registros = FetchHelper.FetchResourceJson<BudgetInfo>(new Uri(registros.next_page_url));
                foreach (var d in registros.data) yield return d;
            }
        }
    }
}
