using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization; // Importante para o [Authorize]
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using A2TP3.Models;
using A2TP3.Persistence;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace A2TP3.Controllers
{
    [Route("api/emprestimos")]
    [ApiController]
    [Authorize] // Garante que todos os métodos requerem usuário autenticado
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
            // Obter ID do usuário logado
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized("Não foi possível obter o userId.");
            }

            var userId = int.Parse(userIdClaim.Value);

            // Retornar apenas os empréstimos do usuário logado
            return await _context.Emprestimos
                .Include(e => e.EmprestimoLivros)
                .ThenInclude(el => el.Livro)
                .ThenInclude(l => l.Categoria)
                .Where(e => e.UsuarioId == userId)
                .ToListAsync();
        }

        // GET: api/Emprestimos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Emprestimo>> GetEmprestimo(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized("Não foi possível obter o userId.");
            }

            var userId = int.Parse(userIdClaim.Value);

            var emprestimo = await _context.Emprestimos
                .Include(e => e.EmprestimoLivros)
                .ThenInclude(el => el.Livro)
                .ThenInclude(l => l.Categoria)
                .FirstOrDefaultAsync(e => e.Id == id && e.UsuarioId == userId);

            if (emprestimo == null)
            {
                return NotFound();
            }

            return emprestimo;
        }

        // PUT: api/Emprestimos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmprestimo(int id, Emprestimo emprestimo)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized("Não foi possível obter o userId.");
            }

            var userId = int.Parse(userIdClaim.Value);

            if (id != emprestimo.Id)
            {
                return BadRequest();
            }

            // Garante que só pode atualizar se for dono do empréstimo
            var emprestimoExistente = await _context.Emprestimos.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id && e.UsuarioId == userId);
            if (emprestimoExistente == null)
            {
                return NotFound("Empréstimo não encontrado ou não pertence a você.");
            }

            emprestimo.UsuarioId = userId; // Garante que o dono não mude
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
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Emprestimo>> PostEmprestimo(EmprestimoDto dto)
        {
            var allClaims = User.Claims.ToList();
            if (!allClaims.Any())
            {
                return Unauthorized("Nenhuma claim encontrada no usuário. O token foi realmente validado?");
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized("Não foi possível obter o userId.");
            }

            var userId = int.Parse(userIdClaim.Value);

            var emprestimo = new Emprestimo
            {
                EmprestimoLivros = new List<EmprestimoLivro>(),
                UsuarioId = userId, // Associa o empréstimo ao usuário logado
                DataEmprestimo = DateTime.Now,
                DataDevolucao = DateTime.Now.AddDays(7)
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

                var emprestimoLivro = new EmprestimoLivro
                {
                    Emprestimo = emprestimo,
                    LivroId = livro.Id
                };

                emprestimo.EmprestimoLivros.Add(emprestimoLivro);
                valorTotal += livro.Valor;
            }

            emprestimo.ValorTotal = valorTotal;

            _context.Emprestimos.Add(emprestimo);
            await _context.SaveChangesAsync();

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
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized("Não foi possível obter o userId.");
            }

            var userId = int.Parse(userIdClaim.Value);

            var emprestimo = await _context.Emprestimos.FirstOrDefaultAsync(e => e.Id == id && e.UsuarioId == userId);
            if (emprestimo == null)
            {
                return NotFound("Empréstimo não encontrado ou não pertence a você.");
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
