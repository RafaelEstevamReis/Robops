using System;
using System.Diagnostics.CodeAnalysis;

namespace Robops.Lib.Senado.Leg
{
    public class Folha : IEquatable<Folha>
    {
        public int CodigoFuncionario { get; set; }
        public DateTime Referencia { get; set; }

        public decimal ValorNaoIdentificado { get; set; }
        public bool CarregouDados { get; set; }

        public string TipoFolha { get; set; }

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
        public decimal PSSS { get; set; }
        public decimal Faltas { get; set; }

        public decimal RemuneracaoAposDescontosObrigatorios { get; set; }

        public decimal Diarias { get; set; }
        public decimal Auxilio_Transporte_Repasse { get; set; }
        public decimal Auxilio_Transporte { get; set; }
        public decimal Auxilio_Alimentacao { get; set; }
        public decimal OutrasVantagensIndenizatorias { get; set; }

        public decimal AdicionalInsalubridade { get; set; }
        public decimal Ferias { get; set; }
        public decimal AdicionalNoturno { get; set; }

        public bool Equals([AllowNull] Folha other)
        {
            return CodigoFuncionario == other.CodigoFuncionario
                && Referencia == other.Referencia;
        }
    }
}
