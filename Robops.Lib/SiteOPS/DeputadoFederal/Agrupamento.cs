using System;
using System.Collections.Generic;
using System.Text;

namespace Robops.Lib.SiteOPS.DeputadoFederal
{
    public class Agrupamento
    {
        public int draw { get; set; }
        public string valorTotal { get; set; }
        public int recordsTotal { get; set; }
        public int recordsFiltered { get; set; }
        public Dados[] data { get; set; }

        public class Dados
        {
            public int id_cf_despesa { get; set; }
            public string data_emissao { get; set; }
            public int id_fornecedor { get; set; }
            public string cnpj_cpf { get; set; }
            public string nome_fornecedor { get; set; }
            public string sigla_estado_fornecedor { get; set; }
            public int id_cf_deputado { get; set; }
            public string nome_parlamentar { get; set; }
            public string numero_documento { get; set; }
            public string trecho_viagem { get; set; }
            public string valor_liquido { get; set; }
        }
    }

    public class FiltroAgrupamento
    {
        public int draw { get; set; }
        public Order[] order { get; set; }
        public int start { get; set; }
        public int length { get; set; }
        public Filters filters { get; set; }

        public class Filters
        {
            public string Agrupamento { get; set; }
            public string Periodo { get; set; }
            public string Fornecedor { get; set; }
        }
        public class Order
        {
            public int column { get; set; }
            public string dir { get; set; }
        }

        public static FiltroAgrupamento AgruparFornecedor(int idPeriodo, int idFornecedor, int start, int len)
        {
            // {"draw":1,"order":[{"column":0,"dir":"asc"}],"start":0,"length":100,"filters":{"Agrupamento":"6","Periodo":"9","Fornecedor":"28252"}}
            return new FiltroAgrupamento()
            {
                draw = 1,
                order = new Order[1]
                {
                    new Order() { column = 0, dir = "asc" },
                },
                start = start,
                length = len,
                filters = new Filters()
                {
                    Agrupamento = "6",
                    Periodo = idPeriodo.ToString(),
                    Fornecedor = idFornecedor.ToString(),
                }
            };

        }
    }
}
