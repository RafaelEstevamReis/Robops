namespace Robops.Lib
{
    public class DigitosVerificadores
    {
        // Vários cálculos de dígito podem ser simplificados pois a maioria usa a mesma técnica
        // A princípio vou implementar cada um da forma direta por restrição de tempo

        public static bool EhCPFValido(string Documento)
        {
            int soma, i, resultado;
            string numeros, digitos;

            if (Documento.Length < 11)
                return false;

            if (EhRepeticao(Documento)) return false;

            numeros = Documento.Substring(0, 9);
            digitos = Documento.Substring(9);
            soma = 0;
            for (i = 10; i > 1; i--)
                soma += (numeros[10 - i] - '0') * i;
            resultado = soma % 11;
            if (resultado < 2)
                resultado = 0;
            else
                resultado = 11 - resultado;

            if (resultado != (digitos[0] - '0'))
                return false;
            numeros = Documento.Substring(0, 10);
            soma = 0;
            for (i = 11; i > 1; i--)
                soma += (numeros[11 - i] - '0') * i;
            resultado = soma % 11 < 2 ? 0 : 11 - soma % 11;
            if (resultado != (digitos[1] - '0'))
                return false;
            return true;
        }

        public static bool EhCNPJValido(string Documento)
        {
            int soma, i, resultado, tamanho, pos;
            string numeros, digitos;

            if (Documento.Length != 14)
                return false;

            if (EhRepeticao(Documento)) return false;

            tamanho = Documento.Length - 2;
            numeros = Documento.Substring(0, tamanho);
            digitos = Documento.Substring(tamanho);
            soma = 0;
            pos = tamanho - 7;
            for (i = tamanho; i >= 1; i--)
            {
                soma += (numeros[tamanho - i] - '0') * pos--;
                if (pos < 2)
                    pos = 9;
            }
            resultado = soma % 11 < 2 ? 0 : 11 - soma % 11;
            if (resultado != (digitos[0] - '0'))
                return false;
            tamanho = tamanho + 1;
            numeros = Documento.Substring(0, tamanho);
            soma = 0;
            pos = tamanho - 7;
            for (i = tamanho; i >= 1; i--)
            {
                soma += (numeros[tamanho - i] - '0') * pos--;
                if (pos < 2)
                    pos = 9;
            }
            resultado = soma % 11 < 2 ? 0 : 11 - soma % 11;
            if (resultado != (digitos[1] - '0'))
                return false;
            return true;
        }
        public static bool EhRepeticao(string Texto)
        {
            if (Texto == null) return false;
            if (Texto.Length < 2) return false;

            for (int i = 1; i < Texto.Length; i++)
            {
                if (Texto[0] != Texto[i]) return false;
            }
            return true;
        }
    }
}
