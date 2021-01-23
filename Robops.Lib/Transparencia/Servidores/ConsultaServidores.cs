using System;
using System.Collections.Generic;
using System.Text;

namespace Robops.Lib.Transparencia.Servidores
{
    public class ConsultaServidores
    {
        public int draw { get; set; }
        public int recordsTotal { get; set; }
        public int recordsFiltered { get; set; }
        public ConsultaServidoresDados[] data { get; set; }
    }

    public class ConsultaServidoresDados
    {
        public string idComFlagExisteDetalhamentoServidor { get; set; }
        public string id
        {
            get { return idComFlagExisteDetalhamentoServidor.Split('_')[0]; }
            set {; }
        }

        public string tipo { get; set; }
        public string cpf { get; set; }
        public string nome { get; set; }
        public string orgaoSuperiorServidorLotacao { get; set; }
        public string orgaoServidorLotacao { get; set; }
        public string orgaoSuperiorServidorExercicio { get; set; }
        public string orgaoServidorExercicio { get; set; }
        public string unidadeOrganizacionalServidorLotacao { get; set; }
        public string unidadeOrganizacionalServidorExercicio { get; set; }
        public string matricula { get; set; }
        public string tipoVinculo { get; set; }
        public string cargo { get; set; }
        public string atividade { get; set; }
        public string funcao { get; set; }
        public string licenca { get; set; }
    }

}
