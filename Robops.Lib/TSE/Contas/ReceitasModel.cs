namespace Robops.Lib.TSE.Contas
{
    public class ReceitasModel
    {
        public enum FormasPagamento
        {
            Outros = 0,
            Dinheiro = 1,
            Cheque = 2,
            Deposito = 3,
            Transferencia = 4,
            Boleto = 5,
            CDebito = 6,
            CCredito = 7,
            Estimado = 8,
        }

        public int Ano { get; set; }
        public int CodigoEleicao { get; set; }
        public string UF { get; set; }
        public int NumeroCandidato { get; set; }
        public string DocumentoDoador { get; set; }
        public string NomeDoadorRFB { get; set; }
        public decimal Valor { get; set; }
        public FormasPagamento FormaPagamento { get; set; } 
    }
}
