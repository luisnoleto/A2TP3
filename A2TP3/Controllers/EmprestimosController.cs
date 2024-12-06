using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using A2TP3.Models;
using A2TP3.Persistence;

namespace A2TP3.Controllers
{
    [Route("api/emprestimos")]
    [ApiController]
    public class EmprestimosController : ControllerBase
    {
        private readonly A2TP3Context _context;

        public EmprestimosController(A2TP3Context context)
        {
            _context = context;
        }

        // GET: api/Emprestimos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Emprestimo>>> GetEmprestimos()
        {
            return await _context.Emprestimos.ToListAsync();
        }

        // GET: api/Emprestimos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Emprestimo>> GetEmprestimo(int id)
        {
            var emprestimo = await _context.Emprestimos.FindAsync(id);

            if (emprestimo == null)
            {
                return NotFound();
            }

            return emprestimo;
        }

        // PUT: api/Emprestimos/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmprestimo(int id, Emprestimo emprestimo)
        {
            if (id != emprestimo.Id)
            {
                return BadRequest();
            }

            _context.Entry(emprestimo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmprestimoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Emprestimos
        [HttpPost]
        public async Task<ActionResult<Emprestimo>> PostEmprestimo(EmprestimoDto dto)
        {
            // Criar o objeto Emprestimo vazio, datas vindas do DTO
            var emprestimo = new Emprestimo
            {
                EmprestimoLivros = new List<EmprestimoLivro>()
            };

            decimal valorTotal = 0;

            // Para cada livroId no DTO, buscar o livro no banco
            foreach (var livroId in dto.LivroIds)
            {
                var livro = await _context.Livros.FindAsync(livroId);
                if (livro == null)
                {
                    return BadRequest($"Livro com ID {livroId} não encontrado.");
                }

                // Criar EmprestimoLivro
                var emprestimoLivro = new EmprestimoLivro
                {
                    Emprestimo = emprestimo,
                    LivroId = livro.Id
                };

                emprestimo.EmprestimoLivros.Add(emprestimoLivro);

                // Somar valor do livro ao valor total do empréstimo
                valorTotal += livro.Valor;
            }

            // Atribuir valor total calculado
            emprestimo.ValorTotal = valorTotal;
            emprestimo.DataEmprestimo = DateTime.Now;
            emprestimo.DataDevolucao = DateTime.Now.AddDays(7);

            // Adicionar e salvar no contexto
            _context.Emprestimos.Add(emprestimo);
            await _context.SaveChangesAsync();

            // Carregar dados completos (incluindo livros) para retornar ao cliente, se desejado
            var emprestimoCompleto = await _context.Emprestimos
                .Include(e => e.EmprestimoLivros)
                .ThenInclude(el => el.Livro)
                .ThenInclude(l => l.Categoria)
                .FirstOrDefaultAsync(e => e.Id == emprestimo.Id);

            return CreatedAtAction("GetEmprestimo", new { id = emprestimo.Id }, emprestimoCompleto);
        }


        // DELETE: api/Emprestimos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmprestimo(int id)
        {
            var emprestimo = await _context.Emprestimos.FindAsync(id);
            if (emprestimo == null)
            {
                return NotFound();
            }

            _context.Emprestimos.Remove(emprestimo);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EmprestimoExists(int id)
        {
            return _context.Emprestimos.Any(e => e.Id == id);
        }
    }
}
