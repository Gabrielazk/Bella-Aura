using Microsoft.EntityFrameworkCore;
using loja.Models;

namespace loja.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Isso vai criar a tabela de Usuários no seu MySQL
        public DbSet<Usuario> Usuarios { get; set; }
    }
}