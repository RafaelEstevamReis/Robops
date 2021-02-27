using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using RafaelEstevam.Simple.Spider.Helper;

namespace Robops.Spiders.AL.SP
{
    public class DadosAbertos_DespesasXML
    {
        // https://www.al.sp.gov.br/repositorioDados/docs/deputados/despesas_gabinetes.pdf

        public static void run()
        {
            string path = @"N:\AL.SP\despesas_gabinetes.xml";

            var valores = XmlSerializerHelper.DeserializeFromFile<Lib.AL.SP.ArquivoDespesas>(path);

            var fs = File.OpenWrite(@"N:\AL.SP\despesas_gabinetes.csv.gz");
            var sw = new StreamWriter(fs, Encoding.UTF8);
            foreach (var v in valores.Despesa)
            {
                string valor = v.Valor.ToString(System.Globalization.CultureInfo.InvariantCulture);
                sw.WriteLine($"{v.Ano};{v.Mes};{v.Matricula};\"{v.Deputado}\";\"{v.Tipo}\";{valor};{v.CNPJ};\"{v.Fornecedor}\"");
            }

        }
    }
}
