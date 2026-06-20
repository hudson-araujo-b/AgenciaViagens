using AgenciaViagens.Interfaces;
using AgenciaViagens.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AgenciaViagens.Controllers
{
    // Controladora para autenticação, cadastro e configurações de conta do usuário
    public class UsuarioController : Controller
    {
        private readonly IUsuarioRepositorio _usuarioRepositorio;

        // Construtor com injeção do repositório de usuários
        public UsuarioController(IUsuarioRepositorio usuarioRepositorio)
        {
            _usuarioRepositorio = usuarioRepositorio;
        }

        // Exibe a tela de login
        [HttpGet]
        public IActionResult Login() => View();

        // Processa as credenciais de login e cria a sessão em cookie se válidas
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel usuario)
        {
            // Valida as anotações do modelo antes de prosseguir
            if (!ModelState.IsValid) return View(usuario);
            
            // Tenta validar o e-mail e a senha informados no banco de dados
            var usuarioVar = _usuarioRepositorio.Logar(usuario.Email, usuario.Senha);

            if (usuarioVar != null)
            {
                // Configura as claims com os dados de identificação do usuário
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, usuarioVar.Nome),
                    new Claim(ClaimTypes.Email, usuarioVar.Email),
                    new Claim("NivelAcesso", usuarioVar.NivelAcesso),
                    new Claim("Id" , usuarioVar.Id.ToString())
                };

                // Cria a identidade principal da autenticação baseada em Cookie
                var identidade = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                // Efetua a gravação do cookie de autenticação no navegador do cliente
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(identidade),
                    new AuthenticationProperties { IsPersistent = false });

                // Redireciona o usuário logado para a vitrine de passagens
                return RedirectToAction("Index", "Passagem");
            }

            // Exibe mensagem de erro na tela se a credencial for inválida
            ModelState.AddModelError(string.Empty, "E-mail ou senha incorretos.");
            return View(usuario);
        }

        // Executa o logout e apaga a sessão em cookie
        public async Task<IActionResult> Sair()
        {
            // Limpa o cookie de autenticação do navegador
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            
            // Redireciona o usuário para a página inicial
            return RedirectToAction("Index", "Passagem");
        }

        // Exibe a tela de cadastro de nova conta
        [HttpGet]
        public IActionResult CriarConta() => View();

        // Processa o cadastro de novos usuários capturando erros de e-mails duplicados
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CriarConta(UsuarioModel usuario)
        {
            // Valida as anotações do modelo
            if (!ModelState.IsValid) return View(usuario);

            try
            {
                // Tenta inserir a conta no banco através do repositório
                _usuarioRepositorio.CriarConta(usuario);
                
                // Define mensagem temporária de sucesso e envia para tela de login
                TempData["MensagemSucesso"] = "Sua conta foi criada com sucesso! Faça login.";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                // Se o e-mail já estiver cadastrado no banco, retorna erro específico
                if (ex.Message.Contains("Duplicate entry") || ex.Message.Contains("key 'Email'") || (ex.InnerException != null && ex.InnerException.Message.Contains("Duplicate entry")))
                {
                    ModelState.AddModelError("Email", "Este e-mail já está cadastrado.");
                }
                else
                {
                    // Erros gerais do banco de dados retornam mensagem amigável genérica
                    ModelState.AddModelError(string.Empty, "Ocorreu um erro ao criar a conta. Tente novamente.");
                }
                return View(usuario);
            }
        }

        // Exibe os dados cadastrados da conta do usuário atual
        [HttpGet]
        public IActionResult Configuracoes()
        {
            // Bloqueia acesso caso o usuário não esteja logado
            if (User.Identity?.IsAuthenticated != true)
            {
                return RedirectToAction("Login");
            }

            // Recupera a Claim contendo o Id do usuário atual
            var idClaim = User.FindFirst("Id")?.Value;
            if (!int.TryParse(idClaim, out int usuarioId))
            {
                return RedirectToAction("Login");
            }

            // Busca os dados cadastrais do usuário no banco
            var usuario = _usuarioRepositorio.ObterPorId(usuarioId);
            if (usuario == null)
            {
                return RedirectToAction("Login");
            }

            // Mapeia o modelo de dados para o modelo específico da View (ViewModel)
            var model = new ConfiguracoesViewModel
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Telefone = usuario.Telefone,
                Cpf = usuario.Cpf,
                Cep = usuario.Cep,
                DataNascimento = usuario.DataNascimento
            };

            return View(model);
        }

        // Processa a atualização de dados cadastrais e recria o cookie com os dados novos
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Configuracoes(ConfiguracoesViewModel model)
        {
            // Bloqueia requisições não autenticadas
            if (User.Identity?.IsAuthenticated != true)
            {
                return RedirectToAction("Login");
            }

            // Valida os campos obrigatórios e formatos do formulário
            if (!ModelState.IsValid) return View(model);

            try
            {
                // Instancia o modelo de usuário mapeando as alterações recebidas
                var usuario = new UsuarioModel
                {
                    Id = model.Id,
                    Nome = model.Nome,
                    Email = model.Email,
                    Telefone = model.Telefone,
                    Cpf = model.Cpf,
                    Cep = model.Cep,
                    DataNascimento = model.DataNascimento
                };

                // Atualiza o registro correspondente no banco
                _usuarioRepositorio.Atualizar(usuario);

                // Obtém o Id e o Nível de acesso existentes para preservar os claims
                var idClaim = User.FindFirst("Id")?.Value;
                var nivelAcesso = User.FindFirst("NivelAcesso")?.Value ?? "Usuário";

                // Recria a lista de claims com os dados atualizados
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, model.Nome),
                    new Claim(ClaimTypes.Email, model.Email),
                    new Claim("NivelAcesso", nivelAcesso),
                    new Claim("Id", idClaim ?? model.Id.ToString())
                };

                var identidade = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                // Grava novamente a sessão de autenticação com as informações atualizadas
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(identidade),
                    new AuthenticationProperties { IsPersistent = false });

                TempData["MensagemSucesso"] = "Dados atualizados com sucesso!";
                return RedirectToAction("Configuracoes");
            }
            catch (Exception ex)
            {
                // Impede o e-mail de ser atualizado para um e-mail em uso por outra pessoa
                if (ex.Message.Contains("Duplicate entry") || ex.Message.Contains("key 'Email'") || (ex.InnerException != null && ex.InnerException.Message.Contains("Duplicate entry")))
                {
                    ModelState.AddModelError("Email", "Este e-mail já está em uso por outra conta.");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Ocorreu um erro ao atualizar os dados. Tente novamente.");
                }
                return View(model);
            }
        }

        // Remove a conta do usuário, apaga a sessão e redireciona à listagem inicial
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletarConta()
        {
            // Bloqueia exclusões não autorizadas
            if (User.Identity?.IsAuthenticated != true)
            {
                return RedirectToAction("Login");
            }

            // Exclui o usuário do banco através da Claim ID
            var idClaim = User.FindFirst("Id")?.Value;
            if (int.TryParse(idClaim, out int usuarioId))
            {
                _usuarioRepositorio.Deletar(usuarioId);
            }

            // Exclui o cookie de login do navegador do usuário
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            
            // Retorna flag temporária de exclusão e redireciona para a vitrine
            TempData["ContaExcluida"] = true;
            return RedirectToAction("Index", "Passagem");
        }
    }
}
