namespace Robops.Lib.Transparencia.Servidores
{
    public class ServidoresCadastroModel
    {
        public int ID_ServidorPortal { get; set; }
        public string Nome { get; set; }
        public string CPF_6D { get; set; }
        public string Matricula_3D { get; set; }
        public string DocumentoIngressoServicoPublico { get; set; }

        public int CodigoOrgaoSuperiorLotacao { get; set; }
        public int CodigoOrgaoLotacao { get; set; }
        public int TipoVinculo { get; set; }
        public bool ServidorCadastroMilitar { get; set; }
    }
}
