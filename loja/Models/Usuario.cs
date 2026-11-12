using System.ComponentModel.DataAnnotations;

namespace loja.Models
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Nome { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Senha { get; set; }

        [Required]
        public string Cep { get; set; }

        [Required]
        public string Numero { get; set; }

        [Required]
        public string Endereco { get; set; }

        [Required]
        public string Bairro { get; set; }

        [Required]
        public string CidadeEstado { get; set; }
    }
}