using System;
using System.Collections.Generic;
using System.Text;

namespace Robops.Lib.Transparencia.AuxilioEmergencial
{
    public class AuxilioModel
    {
        public string NIS { get; set; }
        public string UF { get; set; }
        public int Municipio { get; set; }
        public string CPF_6D { get; set; }
        public string Nome { get; set; }
        public string NIS_Responsavel { get; set; }
        public string CPF_6D_Responsavel { get; set; }
        public string Nome_Responsavel { get; set; }
        public string Enquadramento { get; set; }

        public string Key()
        {
            return $"{Municipio}{NIS}{CPF_6D}{Nome.Split(' ')[0]}";
        }
    }
}
