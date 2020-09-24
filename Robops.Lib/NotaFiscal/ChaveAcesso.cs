using System;
using System.Linq;

namespace Robops.Lib.NotaFiscal
{
    public class ChaveAcesso
    {
        public static readonly int AnoCorrenteYY = DateTime.Now.Year % 100;
        // Lista de códigos do IBGe
        public static readonly string[] UFs = {
            "11", "12", "13", "14", "15", "16", "17", // Região Norte
            "21", "22", "23", "24", "25", "26", "27", "28", "29", // Região Nordeste
            "31", "32", "33", "35", // Região Sudeste
            "41", "42", "43",       // Região Sul
            "50", "51", "52", "53"  // Região Centro-Oeste
        };

        public enum ModeloNota
        {
            NFe = 55,
            NFCe = 65,
            SAT = 59,
            Desconhecido = -1,
        }
        #region Geral
        public string Chave { get; }

        public string sUF { get; }
        public string sDataEmissao { get; }
        public string sCNPJ { get; }
        public string sModelo { get; }
        public ModeloNota Modelo { get; }

        public string sCodigoNumericoAleatorio { get; }
        public string sDV { get; }
        #endregion

        #region NFe / NFCe
        public string sSerie { get; }
        public string sNF { get; }
        public string sTipoEmissao { get; }
        #endregion

        #region SAT
        public string sNumeroSerieSAT { get; }
        public string sCF { get; }
        #endregion

        public ChaveAcesso(string Chave)
        {
            if (Chave.Length != 44) throw new ArgumentException("A chave deve ter 44 caracteres");
            if (!Chave.Any(c => char.IsNumber(c))) throw new ArgumentException("A chave deve ser numérica");

            this.Chave = Chave;

            this.sUF = Chave[0..2];
            this.sDataEmissao = Chave[2..6];
            this.sCNPJ = Chave[6..20];
            this.sModelo = Chave[20..22];

            this.Modelo = sModelo switch
            {
                "55" => ModeloNota.NFe,
                "59" => ModeloNota.SAT,
                "65" => ModeloNota.NFCe,
                _ => ModeloNota.Desconhecido
            };

            this.sDV = Chave[43..44];

            if (Modelo == ModeloNota.NFe || Modelo == ModeloNota.NFCe)
            {
                this.sSerie = Chave[22..25];
                this.sNF = Chave[25..34];
                this.sTipoEmissao = Chave[34..35];
                this.sCodigoNumericoAleatorio = Chave[35..43];
            }
            else if (Modelo == ModeloNota.SAT)
            {
                this.sNumeroSerieSAT = Chave[22..31];
                this.sCF = Chave[31..37];
                this.sCodigoNumericoAleatorio = Chave[37..43];
            }
        }

        /// <summary>
        /// Chamada leve para verificar se uma sequência é uma chave válida
        /// </summary>
        /// <param name="Chave">Sequência de 44 números a ser validada</param>
        public static bool EhChaveValida(string Chave)
        {
            // As verificações serão feitas em ordem de Menor processamento para Maior processamento
            // Assim chaves inválidas são descartadas o mais rápido possível
            //  Com meu AMDRyzen 3200, este teste validou 100M de chaves corretas em 45 segundos
            //   100M / ~45s => 2.237.712 chaves/seg
            //  Os testes foram com chaves corretas (passa por todas as etapas)
            //   com otimização no JIT e com o GC limpo

            /* ---------------------------------------------------------------------- */

            //  0             1           2           3         4
            //  01  2345  67890123456789  01  234567890123456789012  3
            // [UF][YYMM][CNPJ          ][TP][VARIA                ][D]
            if (Chave.Length != 44) return false;
            if (!Chave.Any(c => char.IsNumber(c))) return false;

            // Checa o Mês da data
            // Mês só pode começar com '0' ou '1'
            if (Chave[6] != '0' && Chave[6] != '1') return false;

            // Checa Tipo de Nota
            string modelo = Chave[20..22];
            if (modelo != "55"
                && modelo != "59"
                && modelo != "65")
            {
                return false;
            }
            // Checa o Ano da data
            int ano = int.Parse(Chave[2..4]);
            if (ano < AnoCorrenteYY) return false;

            // Checa UF
            if (!UFs.Contains(Chave[0..2])) return false;

            // Acabaram todos os testes rápidos
            // Agora sobraram os lentos, calcular DVs
            // Checa se DV do CNPJ bate
            if (!DigitosVerificadores.EhCNPJValido(Chave[6..20])) return false;

            // Checa se DV da chave de acesso bate
            if (CalculaDigitoChaveAcesso(Chave) != Chave[43]) return false;

            // Nada impede
            return true;
        }

        public static char CalculaDigitoChaveAcesso(string ChaveAcesso)
        {
            if (ChaveAcesso.Length == 44) ChaveAcesso = ChaveAcesso[0..43];

            int acumulador = 0;
            int fator = 2;
            for (int i = 42; i >= 0; i--)
            {
                acumulador += fator * (ChaveAcesso[i] - '0');

                fator++;
                if (fator > 9) fator = 2;
            }

            var resto = acumulador % 11;
            if (resto <= 1) return '0'; // 0 ou 1, retorna zero

            int dv = 11 - resto;

            return (char)('0' + dv);
        }
    }
}
