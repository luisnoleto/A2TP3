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
        /// <summary>
        /// Busca seus empréstimos.
        /// </summary>
        /// <remarks>Como usuario logado você pode buscar todos os emprestimos que fez (apenas os do seu login irão aparecer)!</remarks>
        [Authorize]
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
        /// <summary>
        /// Busca um empréstimo pelo ID.
        /// </summary>
        /// <remarks>Como usuario logado você pode buscar um emprestimo seu pelo Id.</remarks>
        [Authorize]
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


        // POST: api/Emprestimos
        /// <summary>
        /// Faz um empréstimo.
        /// </summary>
        /// <remarks>Como usuario logado você pode fazer um emprestimo de livros, basta passar os Ids dos livros que deseja alugar! (É preciso cadastrar um livro antes)</remarks>
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


    }
}

