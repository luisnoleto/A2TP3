using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace A2TP3.Models
{
    public class Livro
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Titulo { get; set; }
        public string Autor { get; set; }
        public string Editora { get; set; }
        public string ISBN { get; set; }
        public string AnoPublicacao { get; set; }

        [Required]
        public int CategoriaId { get; set; }

        // Navegação para Categoria (não usar [JsonIgnore])
        public virtual Categoria Categoria { get; set; }

        public decimal Valor { get; set; }
        public bool isDeleted { get; set; } = false;
    }
}
