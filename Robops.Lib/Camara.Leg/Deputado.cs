using System;
using Simple.Sqlite.Attributes;

namespace Robops.Lib.Camara.Leg
{
    public class Deputado
    {
        [Unique]
        public int Id { get; set; }
        public string Nome { get; set; }
        public string NomeCivil { get; set; }
        public string PartidoLideranca { get; set; }
        public string EMail { get; set; }
        public string Naturalidade { get; set; }
        public string Naturalidade_UF { get; set; }
        public string Telefone { get; set; }
        public string Endereco { get; set; }
        public DateTime DataNascimento { get; set; }
    }
}
