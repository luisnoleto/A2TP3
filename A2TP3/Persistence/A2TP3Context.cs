using Microsoft.EntityFrameworkCore;
using A2TP3.Models;

namespace A2TP3.Persistence
{
    public class A2TP3Context : DbContext
    {
        public DbSet<Livro> Livros { get; set; }
        public DbSet<Categoria> Categoria { get; set; }

        public DbSet<EmprestimoLivro> EmprestimoLivros { get; set; }

        public DbSet<Emprestimo> Emprestimos { get; set; }

        public A2TP3Context(DbContextOptions<A2TP3Context> options)
            : base(options)
        {
        }
    }
}
