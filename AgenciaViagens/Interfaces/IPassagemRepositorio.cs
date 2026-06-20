using System.Collections.Generic;
using AgenciaViagens.Models;

namespace AgenciaViagens.Interfaces
{
    // Definição das operações relacionadas às passagens e voos
    public interface IPassagemRepositorio
    {
        // Obtém a listagem completa de todos os voos cadastrados no banco
        public IEnumerable<VooModel> ListarPassagens();
    }
}
