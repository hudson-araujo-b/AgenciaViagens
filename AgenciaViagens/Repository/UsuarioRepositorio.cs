using AgenciaViagens.Interfaces;
using AgenciaViagens.Models;
using MySql.Data.MySqlClient;

namespace AgenciaViagens.Repository
{
    // Implementação do repositório de usuários utilizando ADO.NET, MySQL e BCrypt para segurança
    public class UsuarioRepositorio : IUsuarioRepositorio
    {
        private readonly string _connectionString;
        public UsuarioRepositorio(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("Conexao")!;
        }

        // Cadastra uma nova conta criptografando a senha em Hash com BCrypt
        public void CriarConta(UsuarioModel usuario)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                string senhaHash = BCrypt.Net.BCrypt.HashPassword(usuario.SenhaHash);

                var sql = "INSERT INTO Usuario(Nome, Email, SenhaHash, NivelAcesso) VALUES (@nome,@email,@senha,@nivel)";
                var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@nome", usuario.Nome);
                cmd.Parameters.AddWithValue("@email", usuario.Email);
                cmd.Parameters.AddWithValue("@senha", senhaHash);
                cmd.Parameters.AddWithValue("@nivel", "Usuário");
                cmd.ExecuteNonQuery();
            }
        }

        // Valida o login do usuário comparando o hash de senha salvo com a senha fornecida
        public UsuarioModel? Logar(string Email, string Senha)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            var sql = " SELECT * FROM Usuario WHERE Email= @email ";
            var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@email", Email);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                string SenhaBanco = reader["SenhaHash"].ToString()!;

                // Verifica se a senha corresponde ao hash gerado pelo BCrypt
                if (BCrypt.Net.BCrypt.Verify(Senha, SenhaBanco))
                {
                    return new UsuarioModel
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Nome = reader["Nome"].ToString()!,
                        Email = reader["Email"].ToString()!,
                        NivelAcesso = reader["NivelAcesso"].ToString()!
                    };
                }
            }
            return null;
        }

        // Recupera todas as informações de cadastro do usuário pelo seu Id
        public UsuarioModel? ObterPorId(int id)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            var sql = "SELECT * FROM Usuario WHERE Id = @id";
            var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new UsuarioModel
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Nome = reader["Nome"].ToString()!,
                    Email = reader["Email"].ToString()!,
                    NivelAcesso = reader["NivelAcesso"].ToString()!,
                    Telefone = reader["Telefone"] != DBNull.Value ? reader["Telefone"].ToString() : null,
                    Cpf = reader["Cpf"] != DBNull.Value ? reader["Cpf"].ToString() : null,
                    Cep = reader["Cep"] != DBNull.Value ? reader["Cep"].ToString() : null,
                    DataNascimento = reader["DataNascimento"] != DBNull.Value ? Convert.ToDateTime(reader["DataNascimento"]) : null
                };
            }
            return null;
        }

        // Atualiza os dados de cadastro no banco, lidando com valores nulos opcionais
        public void Atualizar(UsuarioModel usuario)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                var sql = "UPDATE Usuario SET Nome = @nome, Email = @email, Telefone = @telefone, Cpf = @cpf, Cep = @cep, DataNascimento = @dataNasc WHERE Id = @id";
                var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@nome", usuario.Nome);
                cmd.Parameters.AddWithValue("@email", usuario.Email);
                cmd.Parameters.AddWithValue("@telefone", (object?)usuario.Telefone ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@cpf", (object?)usuario.Cpf ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@cep", (object?)usuario.Cep ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@dataNasc", (object?)usuario.DataNascimento ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@id", usuario.Id);
                cmd.ExecuteNonQuery();
            }
        }

        // Deleta o usuário e suas compras de forma transacional (tudo ou nada)
        public void Deletar(int id)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        // Excluir histórico de compras associado ao usuário
                        var deleteComprasSql = "DELETE FROM Compra WHERE UsuarioId = @usuarioId";
                        using (var cmdCompras = new MySqlCommand(deleteComprasSql, conn, trans))
                        {
                            cmdCompras.Parameters.AddWithValue("@usuarioId", id);
                            cmdCompras.ExecuteNonQuery();
                        }

                        // Excluir conta de usuário principal
                        var deleteUsuarioSql = "DELETE FROM Usuario WHERE Id = @usuarioId";
                        using (var cmdUsuario = new MySqlCommand(deleteUsuarioSql, conn, trans))
                        {
                            cmdUsuario.Parameters.AddWithValue("@usuarioId", id);
                            cmdUsuario.ExecuteNonQuery();
                        }

                        // Salva permanentemente as exclusões no banco
                        trans.Commit();
                    }
                    catch
                    {
                        // Desfaz as exclusões em caso de erro para manter o banco consistente
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}
