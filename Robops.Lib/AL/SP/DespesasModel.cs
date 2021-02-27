namespace Robops.Lib.AL.SP
{
    [System.Xml.Serialization.XmlRoot("despesas", Namespace = "", IsNullable = false)]
    public class ArquivoDespesas
    {
        [System.Xml.Serialization.XmlElement("despesa")]
        public DespesasModel[] Despesa { get; set; }
    }
    public class DespesasModel
    {
        public string Deputado { get; set; }
        public string Matricula { get; set; }
        public int Ano { get; set; }
        public int Mes { get; set; }
        public string Tipo { get; set; }
        public string Fornecedor { get; set; }
        public string CNPJ { get; set; }
        public decimal Valor { get; set; }
    }

}
