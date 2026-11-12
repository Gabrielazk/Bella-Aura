using System.ComponentModel.DataAnnotations;

namespace BellaAura.Models
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; } 

        [Required]
        public string Nome { get; set; } = string.Empty; 

        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Senha { get; set; } = string.Empty;

        [Required]
        public string Cep { get; set; } = string.Empty;

        [Required]
        public string Numero { get; set; } = string.Empty;

        [Required]
        public string Endereco { get; set; } = string.Empty;

        [Required]
        public string Bairro { get; set; } = string.Empty;

        [Required]
        public string CidadeEstado { get; set; } = string.Empty;
    }
}