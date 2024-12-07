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
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly A2TP3Context _context;

        public UsuariosController(A2TP3Context context)
        {
            _context = context;
        }

        // GET: api/Usuarios
        /// <summary>
        /// Retorna os usuarios cadastrados.
        /// </summary>
        /// <remarks>Como usuario logado você pode ver todos os usuarios cadastrados no sistema!</remarks>
        /// 
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuario()
        {
            return await _context.Usuario.ToListAsync();
        }

        // GET: api/Usuarios/5
        /// <summary>
        /// Retorna um usuario pelo seu ID.
        /// </summary>
        /// <remarks>Como usuario logado você pode buscar um usuario pelo Id.</remarks>
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> GetUsuario(int id)
        {
            var usuario = await _context.Usuario.FindAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            return usuario;
        }

        // PUT: api/Usuarios/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        /// <summary>
        /// Atualiza um usuario já cadastrado.
        /// </summary>
        /// <remarks>Como usuario logado você pode alterar um usuario já cadastrado, através do Id.</remarks>
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuario(int id, Usuario usuario)
        {
            if (id != usuario.Id)
            {
                return BadRequest();
            }

            _context.Entry(usuario).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExists(id))
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

        // POST: api/Usuarios
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        /// <summary>
        /// Cadastre-se no sistema.
        /// </summary>
        /// <remarks>Se você ainda não tem uma conta, pode se cadastrar aqui, para ter acesso as outras funcionalidades do sistema!</remarks>
        [HttpPost]
        public async Task<ActionResult<Usuario>> PostUsuario(UsuarioDTO usuarioDto)

        {
            var usuario = new Usuario
            {
                Nome = usuarioDto.Nome,
                Login = usuarioDto.Login,
                Password = usuarioDto.Password 
            };

            _context.Usuario.Add(usuario);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUsuario", new { id = usuario.Id }, usuario);
        }

        // DELETE: api/Usuarios/5
        /// <summary>
        /// Delete um usuario pelo seu ID.
        /// </summary>
        /// <remarks>Como usuario logado você pode deletar um usuario pelo Id.</remarks>
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            var usuario = await _context.Usuario.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            _context.Usuario.Remove(usuario);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuario.Any(e => e.Id == id);
        }
    }
}
