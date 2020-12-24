namespace Robops.Lib.Senado.Leg
{
    public class Senador
    {
        public int CodigoSenador { get; set; }
        public string NomeCivil { get; set; }
        public string Nascimento { get; set; }
        public string Naturalidade { get; set; }
        public string LocalGabinete { get; set; }

        public FuncionarioGabinete[] Gabinete;

        public override string ToString()
        {
            return $"{CodigoSenador} {NomeCivil}";
        }
    }
}
