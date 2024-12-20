﻿using A2TP3.DTO;
using A2TP3.Models;
using A2TP3.Persistence;
using A2TP3.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace A2TP3.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly A2TP3Context _context;
        private readonly ITokenService _tokenService;

        public AuthController(A2TP3Context context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }
        /// <summary>
        /// Faz login no sistema.
        /// </summary>
        /// <remarks>Se você já tiver criado uma conta no endpoint de usuarios, pode fazer login aqui, para ter acesso as outras funcionalidades do sistema! Lembre-se de digitar Bearer antes de colcoar o token sem aspas no campo</remarks>
        /// <response code="200">Retorna o token de acesso.</response>
        ///
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
        {
            var usuario = await _context.Usuario
                .FirstOrDefaultAsync(u => u.Login == loginDto.Login);

            if (usuario == null)
                return Unauthorized("Usuário ou senha inválidos.");

            // Aqui você deveria verificar a senha com hashing e etc.
            if (usuario.Password != loginDto.Password)
                return Unauthorized("Usuário ou senha inválidos.");

            var token = _tokenService.GenerateToken(usuario);
            return Ok(new { Token = token });
        }
    }
}
