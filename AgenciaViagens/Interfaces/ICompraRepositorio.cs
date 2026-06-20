using System.Collections.Generic;
using AgenciaViagens.Models;

namespace AgenciaViagens.Interfaces
{
    // Definição das operações relacionadas às compras de passagens
    public interface ICompraRepositorio
    {
        // Realiza o processo de compra de uma passagem associando o usuário ao voo
        public void Comprar(int usuarioId, int vooId);

        // Lista todo o histórico de compras de um determinado usuário
        public IEnumerable<CompraModel> ListarCompras(int usuarioId);
    }
}
