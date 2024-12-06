using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace A2TP3.Models
{
    public class Usuario
    {


        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Nome { get; set; }

        public string Login { get; set; }

        public string Password { get; set; }

        public virtual List<Emprestimo>? EmprestimosUsuario{ get; set; }
    }
}
