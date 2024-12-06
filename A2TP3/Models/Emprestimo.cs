
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace A2TP3.Models
{
    public class Emprestimo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Display(Name = "Data de Empréstimo")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DataEmprestimo { get; set; }

        [Display(Name = "Data de Devolução")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DataDevolucao { get; set; }
        [Display(Name = "Valor do Emprestimo")]
        public decimal ValorTotal { get; set; }

        public virtual List<EmprestimoLivro> EmprestimoLivros { get; set; }

    }

}