using System;

namespace Robops.Lib.Camara.Leg.API
{
    public partial class Deputado
    {
        public xmlDados dados { get; set; }
        public xmlLinks[] links { get; set; }
    }

    public partial class xmlDados
    {
        public uint id { get; set; }
        public string uri { get; set; }
        public string nomeCivil { get; set; }
        public xmlDadosUltimoStatus ultimoStatus { get; set; }
        public string cpf { get; set; }
        public string sexo { get; set; }
        public string urlWebsite { get; set; }

        [System.Xml.Serialization.XmlArrayItemAttribute("redeSocial", IsNullable = false)]
        public string[] redeSocial { get; set; }
        public DateTime dataNascimento { get; set; }
        public DateTime? dataFalecimento { get; set; }
        public string ufNascimento { get; set; }
        public string municipioNascimento { get; set; }
        public string escolaridade { get; set; }
    }

    public partial class xmlDadosUltimoStatus
    {
        public uint id { get; set; }
        public string uri { get; set; }
        public string nome { get; set; }
        public string siglaPartido { get; set; }
        public string uriPartido { get; set; }
        public string siglaUf { get; set; }
        public byte idLegislatura { get; set; }
        public string urlFoto { get; set; }
        public string email { get; set; }
        public string data { get; set; }
        public string nomeEleitoral { get; set; }
        public xmlDadosUltimoStatusGabinete gabinete { get; set; }
        public string situacao { get; set; }
        public string condicaoEleitoral { get; set; }
        public object descricaoStatus { get; set; }
    }

    public partial class xmlDadosUltimoStatusGabinete
    {
        public string nome { get; set; }
        public string predio { get; set; }
        public string sala { get; set; }
        public string andar { get; set; }
        public string telefone { get; set; }
        public string email { get; set; }
    }

    public partial class xmlLinks
    {
        public string rel { get; set; }
        public string href { get; set; }
    }
}
