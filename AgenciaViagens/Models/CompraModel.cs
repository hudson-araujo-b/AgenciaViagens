using System;

namespace AgenciaViagens.Models
{
    public class CompraModel
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int VooId { get; set; }
        public DateOnly DataCompra { get; set; }

        public UsuarioModel? Usuario { get; set; }
        public VooModel? Voo { get; set; }
    }
}
