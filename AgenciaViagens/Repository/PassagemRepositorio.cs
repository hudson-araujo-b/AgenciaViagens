using AgenciaViagens.Interfaces;
using AgenciaViagens.Models;
using MySql.Data.MySqlClient;

namespace AgenciaViagens.Repository
{
    // Implementação do repositório de voos e passagens com MySQL
    public class PassagemRepositorio : IPassagemRepositorio
    {
        private readonly string _connectionString;

        // Construtor que lê a string de conexão do arquivo appsettings.json
        public PassagemRepositorio(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("Conexao")!;
        }

        // Obtém voos cadastrados calculando as vagas restantes subtraindo as passagens compradas
        public IEnumerable<VooModel> ListarPassagens()
        {
            // Inicializa e abre a conexão com o banco MySQL
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            // SQL Query com Subquery para calcular a quantidade de assentos disponíveis dinamicamente
            var sql = "SELECT Id, Origem, OrigemUf, Destino, DestinoUf, DataViagem, Preco, (NumPassagens - (SELECT COUNT(*) FROM Compra WHERE VooId = Voo.Id)) AS PassagensDisponiveis FROM Voo";
            var cmd = new MySqlCommand(sql, conn);

            // Cria uma lista vazia para armazenar as passagens encontradas
            var lista = new List<VooModel>();
            
            // Executa o comando de leitura e itera sobre as linhas do resultado
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                // Converte e adiciona os dados lidos para o modelo de domínio do Voo
                lista.Add(new VooModel
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Origem = reader["Origem"].ToString()!,
                    OrigemUf = reader["OrigemUf"].ToString()!,
                    Destino = reader["Destino"].ToString()!,
                    DestinoUf = reader["DestinoUf"].ToString()!,
                    DataViagem = Convert.ToDateTime(reader["DataViagem"]),
                    Preco = Convert.ToDecimal(reader["Preco"]),
                    PassagensDisponiveis = Convert.ToInt32(reader["PassagensDisponiveis"])
                });
            }
            
            // Retorna a lista populada de voos
            return lista;
        }
    }
}
