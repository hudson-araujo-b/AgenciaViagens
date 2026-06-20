using AgenciaViagens.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgenciaViagens.Controllers
{
    // Controladora de compras (exige autenticação do usuário)
    [Authorize]
    public class CompraController : Controller
    {
        private readonly ICompraRepositorio _compraRepositorio;

        // Construtor com injeção do repositório correspondente
        public CompraController(ICompraRepositorio compraRepositorio)
        {
            _compraRepositorio = compraRepositorio;
        }

        // Exibe o histórico de compras de passagens do usuário autenticado
        [HttpGet]
        public IActionResult Index()
        {
            // Obtém o Id do usuário autenticado a partir dos claims da sessão
            int usuarioId = int.Parse(User.FindFirst("Id")!.Value);
            
            // Busca a lista de compras vinculadas ao usuário no banco
            var compras = _compraRepositorio.ListarCompras(usuarioId);
            
            // Renderiza a view com a listagem de compras encontradas
            return View(compras);
        }
    }
}
