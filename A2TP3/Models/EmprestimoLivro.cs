using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace A2TP3.Models
{
    public class EmprestimoLivro
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int EmprestimoId { get; set; }

        [JsonIgnore]
        public Emprestimo? Emprestimo { get; set; }

        public int LivroId { get; set; }
        public Livro? Livro { get; set; }
    }
}
