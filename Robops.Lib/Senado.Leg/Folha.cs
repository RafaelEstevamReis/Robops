using System;

namespace Robops.Lib.Senado.Leg
{
    public class Folha
    {
        public int CodigoFuncionario { get; set; }
        public DateTime Referencia { get; set; }

        public decimal RemuneracaoBasica { get; set; }
        public decimal VantagensPessoais { get; set; }

        public decimal FuncaoComissionada { get; set; }
        public decimal AntecipacaoGratificacaoNatalina { get; set; }
        public decimal HorasExtras { get; set; }
        public decimal OutrasRemuneracoesEventuais { get; set; }

        public decimal AbonoPermanencia { get; set; }

        public decimal ReversaoTextoConstitucional { get; set; }
        public decimal ImpostoRenda { get; set; }
        public decimal INSS { get; set; }
        public decimal Faltas { get; set; }


        public decimal RemuneracaoAposDescontosObrigatorios { get; set; }

        public decimal Diarias { get; set; }
        public decimal Auxilio_Alimentacao { get; set; }
        public decimal OutrasVantagensIndenizatorias { get; set; }
    }
}
