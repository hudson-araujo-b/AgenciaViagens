using System;

namespace AgenciaViagens.Models
{
    public class VooModel
    {
        public int Id { get; set; }
        public string Origem { get; set; } = string.Empty;
        public string OrigemUf { get; set; } = string.Empty;
        public string Destino { get; set; } = string.Empty;
        public string DestinoUf { get; set; } = string.Empty;
        public DateTime DataViagem { get; set; }
        public decimal Preco { get; set; }
        public int PassagensDisponiveis { get; set; }
    }
}
