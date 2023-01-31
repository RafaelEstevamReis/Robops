using Robops.Pickle.Models;

namespace Robops.Pickle.Interfaces
{
    public interface IColeta
    {
        public string NomeCasa { get; }

        void Setup(ConfiguracaoColeta configuracao);
        public void ColetarDados();
    }
}
