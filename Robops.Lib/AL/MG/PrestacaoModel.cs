using System;
using System.Xml.Serialization;

namespace Robops.Lib.AL.MG
{
    [System.Xml.Serialization.XmlRoot("listaResumoVerba", Namespace = "", IsNullable = false)]
    public class ListaResumoVerba
    {
        [XmlElement("resumoVerba")]
        public ListaResumoVerbaResumoVerba[] resumoVerba { get; set; }
    }

    public partial class ListaResumoVerbaResumoVerba
    {
        public int idDeputado { get; set; }
        public DateTime dataReferencia { get; set; }
        public string valor { get; set; }
        public string descTipoDespesa { get; set; }

        [XmlArray("listaDetalheVerba", IsNullable = false)]
        [XmlArrayItem("detalheVerba", IsNullable = false)]
        public ListaResumoVerbaResumoVerbaDetalheVerba[] listaDetalheVerba { get; set; }
    }

    public partial class ListaResumoVerbaResumoVerbaDetalheVerba
    {
        public int id { get; set; }
        public int idDeputado { get; set; }
        public DateTime dataReferencia { get; set; }
        public string valorReembolsado { get; set; }
        public DateTime dataEmissao { get; set; }
        public ulong cpfCnpj { get; set; }
        public string valorDespesa { get; set; }
        public string nomeEmitente { get; set; }
        public string descDocumento { get; set; }
        public byte codTipoDespesa { get; set; }
        public string descTipoDespesa { get; set; }
    }


}
