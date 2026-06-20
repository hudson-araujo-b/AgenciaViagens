using System.ComponentModel.DataAnnotations;

namespace AgenciaViagens.Models
{
    public class ConfiguracoesViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório.")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "O e-mail informado não é válido.")]
        public string Email { get; set; } = string.Empty;

        public string? Telefone { get; set; }
        public string? Cpf { get; set; }
        public string? Cep { get; set; }
        public DateTime? DataNascimento { get; set; }
    }
}
