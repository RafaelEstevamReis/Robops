using System;

namespace Robops.Lib.Camara.Leg.Cota
{
    public class Despesa
    {
        public int IdDeputado { get; set; }

        public TiposDespesa TipoDespesa { get; set; }
        public string Numero { get; set; }
        public DateTime DataEmissao { get; set; }
        public DateTime DataCompetencia { get; set; }

        public string DocumentoFornecedor { get; set; }
        public string NomeFornecedor { get; set; }

        public decimal ValorDespesa { get; set; }
        public decimal Deducoes { get; set; }
        public decimal Glosas { get; set; }
        public decimal Restituicoes { get; set; }
        public decimal Reembolso { get; set; }

    }
}
