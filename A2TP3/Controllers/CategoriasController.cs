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
    [Route("api/categorias")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        private readonly A2TP3Context _context;

        public CategoriasController(A2TP3Context context)
        {
            _context = context;
        }

        // GET: api/Categorias
        /// <summary>
        /// Busca todas as categorias já cadastradas no sistema
        /// </summary>
        /// <remarks>Se você como usuario logado adicionou alguma categoria ao sistema, ele aparecerá aqui, caso não, retorna uma lista vazia!</remarks>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Categoria>>> GetCategoria()
        {
            return await _context.Categoria.ToListAsync();
        }

        /// <summary>
        /// Mostra uma categoria pelo seu ID.
        /// </summary>
        /// <remarks>
        /// Pode usar o id de uma categoria para buscar informações sobre ela.
        /// </remarks>
        /// <response code="200">Retorna a categoria solicitada.</response>
        /// <response code="404">Categoria não encontrada.</response>
        [HttpGet("{id}")]
        public async Task<ActionResult<Categoria>> GetCategoria(int id)
        {
            var categoria = await _context.Categoria.FindAsync(id);

            if (categoria == null)
            {
                return NotFound();
            }

            return categoria;
        }

        // PUT: api/Categorias/5
        ///<summary>
        ///Atualiza uma categoria já cadastrada.
        ///</summary>
        ///<remarks>
        ///Um usuario logado pode alterar uma categoria já cadastrada, através do Id.
        ///</remarks>
        /// <response code="200">Atualizada a categoria desejada.</response>
        /// <response code="401">Não autorizado, faça login.</response>
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategoria(int id, Categoria categoria)
        {
            if (id != categoria.Id)
            {
                return BadRequest();
            }

            _context.Entry(categoria).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoriaExists(id))
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

        // POST: api/Categorias
        /// <summary>
        /// Cadastra uma nova categoria no sistema.
        /// </summary>
        /// <remarks>
        ///  Um usuario logado pode adicionar uma nova categoria ao sistema, colocando apenas o nome.
        ///  </remarks>
        /// <response code="200">Cadastra a categoria.</response>
        /// <response code="401">Não autorizado, faça login.</response>
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Categoria>> PostCategoria(Categoria categoria)
        {
            // Garantir que o Id seja 0,oforçar geração pelo EF
            categoria.Id = 0;

            _context.Categoria.Add(categoria);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCategoria", new { id = categoria.Id }, categoria);
        }


        // DELETE: api/Categorias/5
        /// <summary>
        /// Deleta uma categoria do sistema.
        /// </summary>
        /// <remarks>
        ///  Um usuario logado pode deletar uma categoria do sistema, através do Id.
        ///  </remarks>
        /// <response code="200">Categoria deletada.</response>
        /// <response code="404">Categoria não encontrada.</response>
        /// <response code="401">Não autorizado, faça login.</response>
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategoria(int id)
        {
            var categoria = await _context.Categoria.FindAsync(id);
            if (categoria == null)
            {
                return NotFound();
            }

            _context.Categoria.Remove(categoria);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CategoriaExists(int id)
        {
            return _context.Categoria.Any(e => e.Id == id);
        }
    }
}
