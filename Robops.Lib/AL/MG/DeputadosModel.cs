using System.Xml.Serialization;

namespace Robops.Lib.AL.MG
{
    [XmlRoot("listaDeputado")]
    public class ListaDeputado
    {
        [XmlElement("deputado")]
        public DeputadosModel[] deputados { get; set; }
    }
    public class DeputadosModel
    {
        public int id { get; set; }
        public string nome { get; set; }
        public string partido { get; set; }
    }

}
