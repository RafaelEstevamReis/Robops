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

        long key = 0;
        public long Key()
        {
            if (NIS == null) return 0;
            if (key == 0) key = long.Parse(NIS);
            return key;
        }

        public override string ToString()
        {
            return $"{UF} {Enquadramento} {CPF_6D} {Nome} | Resp:{NIS_Responsavel}";
        }
    }
}
