using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using A2TP3.Models;
using A2TP3.Persistence;
using Microsoft.AspNetCore.Authorization;

namespace A2TP3.Controllers
{
    [Route("api/livros")]
    [ApiController]
    public class LivrosController : ControllerBase
    {
        private readonly A2TP3Context _context;

        public LivrosController(A2TP3Context context)
        {
            _context = context;
        }

        // GET: api/Livros
        /// <summary>
        /// Retorna todos os livros cadastrados.
        /// </summary>
        /// <remarks>Como usuario logado você pode ver todos os livros cadastrados no sistema!</remarks>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Livro>>> GetLivros()
        {
            // Inclui a categoria no resultado
            return await _context.Livros.Include(l => l.Categoria).ToListAsync();
        }

        // GET: api/Livros/5
        /// <summary>
        /// Retorna um livro pelo Id.
        /// </summary>
        /// <remarks>Como usuario logado você pesquisar um livro previamente cadastrado pelo Id!</remarks>
        [HttpGet("{id}")]
        public async Task<ActionResult<Livro>> GetLivro(int id)
        {
            var livro = await _context.Livros.FindAsync(id);

            if (livro == null)
            {
                return NotFound();
            }

            return livro;
        }

        // PUT: api/Livros/5
        /// <summary>
        /// Realiza a atualização de um livro.
        /// </summary>
        /// <remarks>Como usuario logado você pode atualizar um livro já cadastrado no sistema, utilizando o Id!</remarks>
        /// 
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLivro(int id, Livro livro)
        {
            if (id != livro.Id)
            {
                return BadRequest();
            }

            _context.Entry(livro).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LivroExists(id))
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

        // POST: api/Livros
        /// <summary>
        /// Cadastra um novo livro no sistema.
        /// </summary>
        /// <remarks>Como usuario logado você pode cadastrar um novo livro no sistema, basta preencher os campos obrigatórios e colocar apenas o CategoriaId da categoria que você cadastrou anteriormente no sistema!</remarks>

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Livro>> PostLivro(Livro livro)
        {
            livro.Id = 0; // Garante que o EF gere o Id

            var categoria = await _context.Categoria.FindAsync(livro.CategoriaId);
            if (categoria == null)
            {
                return BadRequest("Categoria inexistente.");
            }

            livro.Categoria = categoria; // Atribui a categoria encontrada
            _context.Livros.Add(livro);
            await _context.SaveChangesAsync();

            // Retorna o livro criado, agora com categoria carregada
            return CreatedAtAction("GetLivro", new { id = livro.Id }, livro);
        }

        // DELETE: api/Livros/5
        /// <summary>
        /// Delete um livro do sistema.
        /// </summary>
        /// <remarks>Como usuario logado você pode deletar um livro do sistema, basta passar o Id do livro que deseja deletar!</remarks>
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLivro(int id)
        {
            var livro = await _context.Livros.FindAsync(id);
            if (livro == null)
            {
                return NotFound();
            }

            _context.Livros.Remove(livro);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LivroExists(int id)
        {
            return _context.Livros.Any(e => e.Id == id);
        }
    }
}
