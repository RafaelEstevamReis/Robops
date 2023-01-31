using Robops.Pickle.Interfaces;
using Robops.Pickle.Models;
using System;

namespace Robops.Pickle.Coletores
{
    public class CamaraCEAP : IColeta
    {
        private ConfiguracaoColeta cfg;
        public string NomeCasa => "Camara Federal";

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
