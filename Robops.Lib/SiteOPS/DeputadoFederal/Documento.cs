using System;
using System.Collections.Generic;
using System.Text;

namespace Robops.Lib.SiteOPS.DeputadoFederal
{
    public class Documento
    {
        public int id_cf_despesa { get; set; }
        public int id_documento { get; set; }
        public string numero_documento { get; set; }
        public string tipo_documento { get; set; }
        public string data_emissao { get; set; }
        public string valor_documento { get; set; }
        public string valor_glosa { get; set; }
        public string valor_liquido { get; set; }
        public string valor_restituicao { get; set; }
        public object parcela { get; set; }
        public object nome_passageiro { get; set; }
        public object trecho_viagem { get; set; }
        public int ano { get; set; }
        public int mes { get; set; }
        public int ano_mes { get; set; }
        public int id_cf_despesa_tipo { get; set; }
        public string descricao_despesa { get; set; }
        public int id_cf_deputado { get; set; }
        public int id_deputado { get; set; }
        public string nome_parlamentar { get; set; }
        public string sigla_estado { get; set; }
        public string sigla_partido { get; set; }
        public int id_fornecedor { get; set; }
        public string cnpj_cpf { get; set; }
        public string nome_fornecedor { get; set; }
        public string competencia { get; set; }
        public int link { get; set; }
        public string url_documento { get; set; }
    }

}
