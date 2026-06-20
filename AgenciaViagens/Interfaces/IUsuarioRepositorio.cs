using AgenciaViagens.Models;

namespace AgenciaViagens.Interfaces
{
    // Definição das operações CRUD e autenticação de usuários
    public interface IUsuarioRepositorio
    {
        // Realiza o cadastro de uma nova conta de usuário
        public void CriarConta(UsuarioModel usuario);

        // Valida as credenciais de login e retorna os dados do usuário em caso de sucesso
        public UsuarioModel? Logar(string Email, string Senha);

        // Busca um usuário a partir do seu identificador numérico único (Id)
        public UsuarioModel? ObterPorId(int id);

        // Atualiza os dados de cadastro de um usuário existente
        public void Atualizar(UsuarioModel usuario);

        // Remove permanentemente o registro de um usuário e suas informações
        public void Deletar(int id);
    }
}
