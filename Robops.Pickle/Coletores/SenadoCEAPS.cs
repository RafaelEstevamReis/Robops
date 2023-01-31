using Robops.Pickle.Interfaces;
using Robops.Pickle.Models;

namespace Robops.Pickle.Coletores
{
    public class SenadoCEAPS : IColeta
    {
        private ConfiguracaoColeta cfg;

        public string NomeCasa => "Senado";

        public void Setup(ConfiguracaoColeta configuracao)
        {
            cfg = configuracao;
        }

        public void ColetarDados()
        {
            //throw new NotImplementedException();
        }

    }
}
