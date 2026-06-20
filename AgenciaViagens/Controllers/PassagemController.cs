using AgenciaViagens.Interfaces;
using AgenciaViagens.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Linq;

namespace AgenciaViagens.Controllers
{
    // Controladora de listagem e compra direta de passagens
    public class PassagemController : Controller
    {
        private readonly IPassagemRepositorio _passagemRepositorio;
        private readonly ICompraRepositorio _compraRepositorio;

        // Construtor com injeção dos repositórios de passagens e compras
        public PassagemController(IPassagemRepositorio passagemRepositorio, ICompraRepositorio compraRepositorio)
        {
            _passagemRepositorio = passagemRepositorio;
            _compraRepositorio = compraRepositorio;
        }

        // Lista todos os voos cadastrados que ainda possuem passagens disponíveis
        [HttpGet]
        public IActionResult Index()
        {
            // Filtra os voos obtendo apenas os que possuem passagens disponíveis acima de zero
            var voos = _passagemRepositorio.ListarPassagens().Where(v => v.PassagensDisponiveis > 0);
            
            // Retorna os voos para renderização na vitrine de passagens
            return View(voos);
        }

        // Processa a compra de uma passagem (requer autenticação manual)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Comprar(int vooId)
        {
            // Se o usuário não estiver logado, redireciona para a view que solicita autenticação
            if (User.Identity?.IsAuthenticated != true || !int.TryParse(User.FindFirst("Id")?.Value, out int usuarioId))
            {
                return View("RequererLogin");
            }

            // Obtém as informações do voo selecionado pelo usuário
            var voo = _passagemRepositorio.ListarPassagens().FirstOrDefault(v => v.Id == vooId);

            try
            {
                // Verifica se o voo existe e se ainda restam passagens disponíveis
                if (voo == null || voo.PassagensDisponiveis <= 0)
                {
                    return View("CompraErro");
                }

                // Efetua a compra inserindo o registro de compra no banco de dados
                _compraRepositorio.Comprar(usuarioId, vooId);

                // Busca as informações do voo com o número de vagas já atualizado para mostrar ao usuário
                var vooAtualizado = _passagemRepositorio.ListarPassagens().FirstOrDefault(v => v.Id == vooId);
                
                // Exibe a tela de sucesso da compra com os dados atualizados
                return View("CompraSucesso", vooAtualizado ?? voo);
            }
            catch (System.Exception)
            {
                // Qualquer erro não tratado no banco redireciona para a tela de falha
                return View("CompraErro");
            }
        }

        // Rota global de visualização de exceções do sistema
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            // Retorna o identificador único da requisição para rastreabilidade de erros
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
