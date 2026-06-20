using AgenciaViagens.Interfaces;
using AgenciaViagens.Models;
using MySql.Data.MySqlClient;

namespace AgenciaViagens.Repository
{
    // Implementação do repositório de compras utilizando ADO.NET com MySQL
    public class CompraRepositorio : ICompraRepositorio
    {
        private readonly string _connectionString;

        // Construtor que lê a string de conexão do arquivo appsettings.json
        public CompraRepositorio(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("Conexao")!;
        }

        // Insere um registro de compra no banco de dados
        public void Comprar(int usuarioId, int vooId)
        {
            // Abre conexão e inicia o comando de inserção direta no banco
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();

                var sql = "INSERT INTO Compra (UsuarioId, VooId) VALUES (@usuarioId, @vooId)";
                var cmd = new MySqlCommand(sql, conn);
                
                // Vincula parâmetros seguros contra SQL Injection
                cmd.Parameters.AddWithValue("@usuarioId", usuarioId);
                cmd.Parameters.AddWithValue("@vooId", vooId);
                
                // Executa o comando de inserção no banco
                cmd.ExecuteNonQuery();
            }
        }

        // Recupera o histórico de compras do usuário realizando JOIN com a tabela de Voos
        public IEnumerable<CompraModel> ListarCompras(int usuarioId)
        {
            // Abre conexão segura com o banco de dados
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            // SQL Query com relacionamento interno (INNER JOIN) para carregar os dados do voo correspondente
            var sql = @"
                SELECT 
                    c.Id AS CompraId, c.UsuarioId, c.VooId, c.DataCompra,
                    v.Origem, v.OrigemUf, v.Destino, v.DestinoUf, v.DataViagem, v.Preco
                FROM Compra c
                INNER JOIN Voo v ON c.VooId = v.Id
                WHERE c.UsuarioId = @usuarioId
                ORDER BY c.DataCompra DESC";
            
            var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@usuarioId", usuarioId);

            var lista = new List<CompraModel>();
            
            // Lê cada linha retornada mapeando para o modelo complexo CompraModel e VooModel
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new CompraModel
                {
                    Id = Convert.ToInt32(reader["CompraId"]),
                    UsuarioId = Convert.ToInt32(reader["UsuarioId"]),
                    VooId = Convert.ToInt32(reader["VooId"]),
                    DataCompra = DateOnly.FromDateTime(Convert.ToDateTime(reader["DataCompra"])),
                    Voo = new VooModel
                    {
                        Id = Convert.ToInt32(reader["VooId"]),
                        Origem = reader["Origem"].ToString()!,
                        OrigemUf = reader["OrigemUf"].ToString()!,
                        Destino = reader["Destino"].ToString()!,
                        DestinoUf = reader["DestinoUf"].ToString()!,
                        DataViagem = Convert.ToDateTime(reader["DataViagem"]),
                        Preco = Convert.ToDecimal(reader["Preco"])
                    }
                });
            }
            
            // Retorna o histórico de compras populado
            return lista;
        }
    }
}
